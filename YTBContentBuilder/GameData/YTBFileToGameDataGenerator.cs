using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YotsubaEngine.YTBContentBuilder.GameData
{
    /// <summary>
    /// Genera <c>YTBFileToGameData.Generated.cs</c> a partir del análisis (regex-based) de los archivos
    /// fuente del engine, buscando <c>[UIComponent]</c> y <c>[UIComponentValue]</c>.
    /// Esto produce código estático (sin reflexión) compatible con AOT/Release/Debug.
    /// </summary>
    public static class YTBFileToGameDataGenerator
    {
        // ---- Modelo interno ----

        private sealed class UIComponentInfo
        {
            public string TypeName = "";
            public string SerializableName = "";
            public List<UIComponentValueInfo> Members = new();
            public bool HasManualConvertTo;
            public bool IsClass;
            public bool HasParameterlessCtor;
        }

        private sealed class UIComponentValueInfo
        {
            public string MemberName = "";
            public string MemberTypeRaw = "";
            public string SerializableName = "";
            public string? ParseConverter;
            public string InactiveValue = "";
            public string DefaultValue = "";
        }

        // ---- Regex ----

        private static readonly Regex _uiComponentRegex =
            new(@"\[(?:YotsubaEngine\.Attributes\.)?UIComponent\(\s*""(?<visible>[^""]*)""\s*,\s*(?:nameof\((?<sn1>\w+)\)|""(?<sn2>[^""]*)"")\s*\)\]\s*(?:public\s+)?(?:partial\s+)?(?<kind>struct|class)\s+(?<type>\w+)",
                RegexOptions.Compiled);

        private static readonly Regex _uiComponentValueRegex =
            new(@"\[(?:YotsubaEngine\.Attributes\.)?UIComponentValue\((?<args>[^\]]+)\)\]\s*public\s+(?<membertype>[\w\.\<\>\?\[\]]+)\s+(?<member>\w+)",
            RegexOptions.Compiled);

        private static readonly Regex _convertToRegex =
            new(@"ConvertTo(?<name>\w+)\s*\(\s*YTBComponents", RegexOptions.Compiled);

        /// <summary>
        /// Entry point: escanea el directorio del engine y produce el archivo Generated.cs.
        /// </summary>
        public static void Generate(string engineSourcePath, string outputFilePath)
        {
            if (!Directory.Exists(engineSourcePath))
            {
                Console.WriteLine($"[YTBFileToGameDataGenerator] Engine source path not found: {engineSourcePath}");
                return;
            }

            var componentsDir = Path.Combine(engineSourcePath, "Core", "Component");
            if (!Directory.Exists(componentsDir))
            {
                Console.WriteLine($"[YTBFileToGameDataGenerator] Components dir not found: {componentsDir}");
                return;
            }

            var components = ScanComponents(componentsDir);

            var ytbFileToGameDataPath = FindYtbFileToGameDataCs(engineSourcePath);
            HashSet<string> manualConverters = ytbFileToGameDataPath != null
                ? FindManualConverters(ytbFileToGameDataPath)
                : new HashSet<string>();

            foreach (var c in components)
                c.HasManualConvertTo = manualConverters.Contains(c.TypeName);

            var output = WriteGenerated(components);

            Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath)!);
            File.WriteAllText(outputFilePath, output, Encoding.UTF8);
            Console.WriteLine($"[YTBFileToGameDataGenerator] Wrote {components.Count} components → {outputFilePath}");
        }

        private static List<UIComponentInfo> ScanComponents(string componentsDir)
        {
            var result = new List<UIComponentInfo>();
            foreach (var file in Directory.GetFiles(componentsDir, "*.cs", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".UI.cs", StringComparison.OrdinalIgnoreCase)) continue;
                string content = File.ReadAllText(file);

                var compMatch = _uiComponentRegex.Match(content);
                if (!compMatch.Success) continue;

                var info = new UIComponentInfo
                {
                    TypeName = compMatch.Groups["type"].Value,
                    SerializableName = compMatch.Groups["sn1"].Success
                        ? compMatch.Groups["sn1"].Value
                        : compMatch.Groups["sn2"].Value,
                    IsClass = compMatch.Groups["kind"].Value == "class"
                };

                // Si es class, buscar constructor parameterless explícito.
                if (info.IsClass)
                {
                    var ctorRegex = new Regex(@"public\s+" + Regex.Escape(info.TypeName) + @"\s*\(\s*\)");
                    info.HasParameterlessCtor = ctorRegex.IsMatch(content);
                }
                else
                {
                    // Structs siempre tienen un default ctor implícito.
                    info.HasParameterlessCtor = true;
                }

                // Encuentra todos los [UIComponentValue] del archivo
                foreach (Match m in _uiComponentValueRegex.Matches(content))
                {
                    var args = m.Groups["args"].Value;
                    var member = new UIComponentValueInfo
                    {
                        MemberName = m.Groups["member"].Value,
                        MemberTypeRaw = m.Groups["membertype"].Value.Trim()
                    };
                    member.SerializableName = ExtractSerializableName(args, member.MemberName);
                    member.ParseConverter = ExtractNamedArg(args, "ValueConverterForParse");
                    member.InactiveValue = ExtractNamedStringArg(args, "inactiveValue") ?? "";
                    member.DefaultValue = ExtractNamedStringArg(args, "defaultValue") ?? "";
                    info.Members.Add(member);
                }

                result.Add(info);
            }
            return result;
        }

        private static string ExtractSerializableName(string args, string memberFallback)
        {
            var parts = SplitTopLevel(args);
            if (parts.Count < 2) return memberFallback;
            var second = parts[1].Trim();
            var nameofMatch = Regex.Match(second, @"^nameof\(\s*(\w+)\s*\)$");
            if (nameofMatch.Success) return nameofMatch.Groups[1].Value;
            if (second.StartsWith("\"") && second.EndsWith("\""))
                return second.Substring(1, second.Length - 2);
            return memberFallback;
        }

        private static string? ExtractNamedArg(string args, string argName)
        {
            var m = Regex.Match(args, argName + @"\s*[:=]\s*(?:nameof\(\s*(\w+)\s*\)|""([^""]*)"")");
            if (!m.Success) return null;
            return m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
        }

        /// <summary>
        /// Extrae un argumento nombrado cuyo valor SIEMPRE es string literal (no nameof). Más permisivo:
        /// acepta secuencias de escape y string vacío.
        /// </summary>
        private static string? ExtractNamedStringArg(string args, string argName)
        {
            var m = Regex.Match(args, argName + @"\s*[:=]\s*""((?:[^""\\]|\\.)*)""");
            if (!m.Success) return null;
            return Regex.Unescape(m.Groups[1].Value);
        }

        private static List<string> SplitTopLevel(string args)
        {
            var result = new List<string>();
            int depth = 0;
            int start = 0;
            bool inString = false;
            for (int i = 0; i < args.Length; i++)
            {
                char ch = args[i];
                if (ch == '"' && (i == 0 || args[i - 1] != '\\')) inString = !inString;
                if (inString) continue;
                if (ch == '(' || ch == '<' || ch == '[') depth++;
                else if (ch == ')' || ch == '>' || ch == ']') depth--;
                else if (ch == ',' && depth == 0)
                {
                    result.Add(args.Substring(start, i - start));
                    start = i + 1;
                }
            }
            result.Add(args.Substring(start));
            return result;
        }

        private static string? FindYtbFileToGameDataCs(string engineSourcePath)
        {
            var matches = Directory.GetFiles(engineSourcePath, "YTBFileToGameData.cs", SearchOption.AllDirectories);
            return matches.FirstOrDefault();
        }

        private static HashSet<string> FindManualConverters(string path)
        {
            var result = new HashSet<string>(StringComparer.Ordinal);
            string content = File.ReadAllText(path);

            // Heurística: buscar "case nameof(TypeName):" en el switch principal.
            var caseRegex = new Regex(@"case\s+nameof\(\s*(\w+)\s*\)\s*:");
            foreach (Match m in caseRegex.Matches(content))
            {
                result.Add(m.Groups[1].Value);
            }
            return result;
        }

        private static string WriteGenerated(List<UIComponentInfo> components)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("// Este archivo es regenerado por YTBContentBuilder.YTBFileToGameDataGenerator");
            sb.AppendLine("// a partir de los atributos [UIComponent]/[UIComponentValue] del engine. NO EDITAR.");
            sb.AppendLine("// </auto-generated>");
            sb.AppendLine("#nullable enable");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Globalization;");
            sb.AppendLine("using Microsoft.Xna.Framework;");
            sb.AppendLine("using Microsoft.Xna.Framework.Graphics;");
            sb.AppendLine("using Microsoft.Xna.Framework.Input;");
            sb.AppendLine("using YotsubaEngine.Core.Component.C_2D;");
            sb.AppendLine("using YotsubaEngine.Core.Component.C_3D;");
            sb.AppendLine("using YotsubaEngine.Core.Component.C_AGNOSTIC;");
            sb.AppendLine("using YotsubaEngine.Physics;");
            sb.AppendLine("using YotsubaEngine.Physics.RigidBody;");
            sb.AppendLine("using YotsubaEngine.Core.YotsubaGame;");
            sb.AppendLine();
            sb.AppendLine("namespace YotsubaEngine.ActionFiles.YTB_Files");
            sb.AppendLine("{");
            sb.AppendLine(" public partial class YTBFileToGameData");
            sb.AppendLine(" {");

            // Helpers reutilizables
            sb.AppendLine(@" private static bool _G_TryParseVector2(string raw, out Vector2 v)
{
    v = Vector2.Zero;
    if (string.IsNullOrEmpty(raw)) return false;
    var parts = raw.Split(',');
    if (parts.Length < 2) return false;
    if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
        && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
    { v = new Vector2(x, y); return true; }
    return false;
}

private static bool _G_TryParseVector3(string raw, out Vector3 v)
{
    v = Vector3.Zero;
    if (string.IsNullOrEmpty(raw)) return false;
    var parts = raw.Split(',');
    if (parts.Length < 3) return false;
    if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
        && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
        && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
    { v = new Vector3(x, y, z); return true; }
    return false;
}

private static bool _G_TryParseRectangle(string raw, out Rectangle r)
{
    r = default;
    if (string.IsNullOrEmpty(raw)) return false;
    var parts = raw.Split(',');
    if (parts.Length < 4) return false;
    if (int.TryParse(parts[0], out var x) && int.TryParse(parts[1], out var y)
        && int.TryParse(parts[2], out var w) && int.TryParse(parts[3], out var h))
    { r = new Rectangle(x, y, w, h); return true; }
    return false;
}

private static bool _G_ShouldSkip(string[]? exclude, string name)
{
    if (exclude == null || exclude.Length == 0) return false;
    for (int i = 0; i < exclude.Length; i++) if (exclude[i] == name) return true;
    return false;
}
");

            // Métodos Parse{Type}_Generated — skipping classes without parameterless ctor
            foreach (var c in components)
            {
                if (c.IsClass && !c.HasParameterlessCtor)
                {
                    sb.AppendLine($"        // SKIPPED: {c.TypeName} is a class without a parameterless constructor.");
                    sb.AppendLine($"        // Use the manual ConvertTo* method in YTBFileToGameData.cs.");
                    sb.AppendLine();
                    continue;
                }

                sb.AppendLine($"        internal static {c.TypeName} Parse{c.TypeName}_Generated(");
                sb.AppendLine("            YTBComponents comp, string sceneName, string entityName, string[]? exclude = null)");
                sb.AppendLine("        {");
                sb.AppendLine($"            var result = new {c.TypeName}();");
                sb.AppendLine("            foreach (var prop in comp.Propiedades)");
                sb.AppendLine("            {");
                sb.AppendLine("                if (_G_ShouldSkip(exclude, prop.Item1)) continue;");
                sb.AppendLine("                switch (prop.Item1)");
                sb.AppendLine("                {");
                foreach (var m in c.Members)
                {
                    sb.AppendLine($" internal static {c.TypeName} Parse{c.TypeName}_Generated(");
                    sb.AppendLine(" EntityManager entityManager, YTBComponents comp, string sceneName, string entityName, string[]? exclude = null)");
                    sb.AppendLine(" {");
                    sb.AppendLine(" Vector3 _initialPosition = Vector3.Zero;");
                    sb.AppendLine(" float _angleView = 45f;");
                    sb.AppendLine(" float _nearRender = 0.1f;");
                    sb.AppendLine(" float _farRender = 1000f;");
                    sb.AppendLine(" string _entityName = \"\";");
                    sb.AppendLine(" Vector3 _offsetCamera = new Vector3(0, 50, -100);");
                    sb.AppendLine(" foreach (var prop in comp.Propiedades)");
                    sb.AppendLine(" {");
                    sb.AppendLine(" if (_G_ShouldSkip(exclude, prop.Item1)) continue;");
                    sb.AppendLine(" switch (prop.Item1)");
                    sb.AppendLine(" {");
                    foreach (var m in c.Members)
                    {
                        sb.AppendLine($" case \"{m.SerializableName}\":");
                        sb.AppendLine(" {");
                        sb.AppendLine($" {EmitParseForTemp(m)}");
                        sb.AppendLine(" break;");
                        sb.AppendLine(" }");
                    }
                    sb.AppendLine(" }");
                    sb.AppendLine(" }");
                    sb.AppendLine(" var result = new CameraComponent3D(entityManager, _initialPosition, _angleView, _nearRender, _farRender);");
                    sb.AppendLine(" result.EntityName = _entityName;");
                    sb.AppendLine(" result.OffsetCamera = _offsetCamera;");
                    sb.AppendLine(" return result;");
                    sb.AppendLine(" }");
                }
                else
                {
                    sb.AppendLine($" internal static {c.TypeName} Parse{c.TypeName}_Generated(");
                    sb.AppendLine(" YTBComponents comp, string sceneName, string entityName, string[]? exclude = null)");
                    sb.AppendLine(" {");
                    sb.AppendLine($" var result = new {c.TypeName}();");
                    sb.AppendLine(" foreach (var prop in comp.Propiedades)");
                    sb.AppendLine(" {");
                    sb.AppendLine(" if (_G_ShouldSkip(exclude, prop.Item1)) continue;");
                    sb.AppendLine(" switch (prop.Item1)");
                    sb.AppendLine(" {");
                    foreach (var m in c.Members)
                    {
                        sb.AppendLine($" case \"{m.SerializableName}\":");
                        sb.AppendLine(" {");
                        sb.AppendLine($" {EmitParse(m)}");
                        sb.AppendLine(" break;");
                        sb.AppendLine(" }");
                    }
                    sb.AppendLine(" }");
                    sb.AppendLine(" }");
                    sb.AppendLine(" return result;");
                    sb.AppendLine(" }");
                }

                sb.AppendLine();
            }

            // _inactiveValuesByComponent + IsAllInactive
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Mapa generado en build-time con los valores 'inactivos' por componente y propiedad.");
            sb.AppendLine("        /// Usado por <see cref=\"IsAllInactive\"/> para decidir si un componente del .ytb debe saltarse.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        internal static readonly Dictionary<string, Dictionary<string, string>> _inactiveValuesByComponent =");
            sb.AppendLine("            new(StringComparer.Ordinal)");
            sb.AppendLine("        {");
            foreach (var c in components)
            {
                sb.AppendLine($"            [\"{c.SerializableName}\"] = new(StringComparer.Ordinal)");
                sb.AppendLine("            {");
                foreach (var m in c.Members)
                {
                    var inactiveLiteral = m.InactiveValue
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r")
                        .Replace("\t", "\\t");
                    sb.AppendLine($"                [\"{m.SerializableName}\"] = \"{inactiveLiteral}\",");
                }
                sb.AppendLine("            },");
            }
            sb.AppendLine("        };");
            sb.AppendLine();

            sb.AppendLine(@"        /// <summary>
        /// Devuelve true si TODAS las propiedades de <paramref name=""comp""/> coinciden con su valor inactivo
        /// declarado por <c>[UIComponentValue.inactiveValue]</c>. Usado por el loop de carga para saltarse
        /// componentes vacíos (reemplaza el viejo filtro contra EntityYTBXmlTemplate.GenerateNew()).
        /// </summary>
        internal static bool IsAllInactive(YTBComponents comp)
        {
            if (comp == null) return false;
            if (!_inactiveValuesByComponent.TryGetValue(comp.ComponentName, out var map)) return false;
            if (comp.Propiedades == null || comp.Propiedades.Count == 0) return true;
            foreach (var prop in comp.Propiedades)
            {
                if (!map.TryGetValue(prop.Item1, out var inactive)) return false;
                if (prop.Item2 != inactive) return false;
            }
            return true;
        }
");

            // _defaultValuesByComponent
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Mapa generado en build-time con los valores 'por defecto' por componente y propiedad.");
            sb.AppendLine("        /// Usado por <c>EntityYTBXmlTemplate</c> como façade sobre el sistema de atributos.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        internal static readonly Dictionary<string, Dictionary<string, string>> _defaultValuesByComponent =");
            sb.AppendLine("            new(StringComparer.Ordinal)");
            sb.AppendLine("        {");
            foreach (var c in components)
            {
                sb.AppendLine($"            [\"{c.SerializableName}\"] = new(StringComparer.Ordinal)");
                sb.AppendLine("            {");
                foreach (var m in c.Members)
                {
                    var defaultLiteral = m.DefaultValue
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r")
                        .Replace("\t", "\\t");
                    sb.AppendLine($"                [\"{m.SerializableName}\"] = \"{defaultLiteral}\",");
                }
                sb.AppendLine("            },");
            }
            sb.AppendLine("        };");
            sb.AppendLine();

            sb.AppendLine("        internal static readonly Dictionary<string, Action<YotsubaEngine.Core.Entity.Yotsuba, YTBComponents, YotsubaEngine.Core.YotsubaGame.Scene, string, string>>?");
            sb.AppendLine("            _autoGeneratedComponents = new(StringComparer.Ordinal)");
            sb.AppendLine("        {");
            foreach (var c in components.Where(x => !x.HasManualConvertTo && !(x.IsClass && !x.HasParameterlessCtor)))
            {
                sb.AppendLine($"            // [\"{c.SerializableName}\"] = (entity, comp, scene, sn, en) => {{ /* TODO: registrar AddX en EntityManager para {c.TypeName} */ }},");
            }
            sb.AppendLine(" };");

            sb.AppendLine(" }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string EmitParse(UIComponentValueInfo m)
        {
            if (!string.IsNullOrEmpty(m.ParseConverter))
            {
                return $"result.{m.MemberName} = ({StripQualifier(m.MemberTypeRaw)}){m.ParseConverter}(prop.Item2);";
            }

            string t = m.MemberTypeRaw;
            return t switch
            {
                "string" => $"result.{m.MemberName} = prop.Item2;",
                "bool" => $"if (bool.TryParse(prop.Item2, out var v)) result.{m.MemberName} = v;",
                "int" => $"if (int.TryParse(prop.Item2, out var v)) result.{m.MemberName} = v;",
                "float" => $"if (float.TryParse(prop.Item2, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)) result.{m.MemberName} = v;",
                "Vector2" => $"if (_G_TryParseVector2(prop.Item2, out var v)) result.{m.MemberName} = v;",
                "Vector3" => $"if (_G_TryParseVector3(prop.Item2, out var v)) result.{m.MemberName} = v;",
                "Rectangle" => $"if (_G_TryParseRectangle(prop.Item2, out var v)) result.{m.MemberName} = v;",
                "Color" => $"if (NamedColors.TryGetValue(prop.Item2, out var v)) result.{m.MemberName} = v;",
                _ =>
                     $"if (Enum.TryParse<{StripQualifier(t)}>(prop.Item2, true, out var v)) result.{m.MemberName} = v;"
            };
        }

        private static string StripQualifier(string typeName)
        {
            int lastDot = typeName.LastIndexOf('.');
            return lastDot >= 0 ? typeName.Substring(lastDot + 1) : typeName;
        }
    }
}
