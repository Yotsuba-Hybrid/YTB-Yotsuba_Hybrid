using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YotsubaEngine.YTBContentBuilder.Models
{
    /// <summary>
    /// Generates a static model registry class by scanning the Assets folder for 3D model files.
    /// Genera una clase de registro de modelos estático escaneando la carpeta Assets en busca de archivos de modelos 3D.
    /// </summary>
    public static class ModelRegistryGenerator
    {
        /// <summary>
        /// 3D model file extensions supported by MonoGame Content Pipeline.
        /// Extensiones de archivos de modelos 3D soportadas por MonoGame Content Pipeline.
        /// </summary>
        private static readonly string[] ModelExtensions = { ".fbx", ".obj", ".dae", ".x" };

        /// <summary>
        /// Generates the model registry class by scanning the source path for 3D model files.
        /// Genera la clase de registro de modelos escaneando la ruta de origen en busca de archivos de modelos 3D.
        /// </summary>
        /// <param name="sourcePath">Path to the Assets folder to scan.</param>
        /// <param name="outputPath">Path where the generated file will be written (without extension).</param>
        public static void GenerateRegistry(string sourcePath, string outputPath)
        {
            //Console.WriteLine($"[ModelRegistry] Scanning for 3D model files in: {sourcePath}");

            var models = new List<(string Name, string RelativePath)>();

            // Scan for 3D model files
            foreach (var extension in ModelExtensions)
            {
                string pattern = "*" + extension;
                string[] files;

                try
                {
                    files = Directory.GetFiles(sourcePath, pattern, SearchOption.AllDirectories);
                }
                catch (DirectoryNotFoundException)
                {
                    //Console.WriteLine($"[ModelRegistry] Directory not found: {sourcePath}");
                    continue;
                }

                var seenPaths = new HashSet<string>();
                foreach (var file in files)
                {
                    // Skip bin/obj folders - check both relative path parts and full path segments
                    string normalizedPath = file.Replace('\\', '/');
                    if (normalizedPath.Contains("/bin/") || normalizedPath.Contains("/obj/") ||
                        normalizedPath.StartsWith("bin/") || normalizedPath.StartsWith("obj/"))
                    {
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(file);

                    // Calculate relative path from Assets folder (for content loading)
                    string relativePath = GetRelativePathForContent(file, sourcePath);

                    // Use HashSet for O(1) duplicate detection
                    if (seenPaths.Add(relativePath))
                    {
                        models.Add((fileName, relativePath));
                        //Console.WriteLine($"[ModelRegistry] Model found: {fileName} ({relativePath})");
                    }
                }
            }

            WriteRegistryFile(models, outputPath);
        }

        /// <summary>
        /// Gets the relative path suitable for Content.Load calls.
        /// Obtiene la ruta relativa adecuada para llamadas a Content.Load.
        /// </summary>
        private static string GetRelativePathForContent(string fullPath, string assetsBasePath)
        {
            // Get path relative to the Assets folder
            string relativePath = Path.GetRelativePath(assetsBasePath, fullPath);

            // Remove the file extension (Content.Load doesn't use extensions)
            relativePath = Path.ChangeExtension(relativePath, null);

            // Normalize path separators to forward slashes for cross-platform compatibility
            relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');

            return relativePath;
        }

        /// <summary>
        /// Converts a name to a valid C# identifier.
        /// Convierte un nombre a un identificador C# válido.
        /// </summary>
        private static string ToValidIdentifier(string name)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];

                if (i == 0)
                {
                    // First character must be letter or underscore
                    if (char.IsLetter(c) || c == '_')
                        sb.Append(c);
                    else if (char.IsDigit(c))
                        sb.Append('_').Append(c);
                    else
                        sb.Append('_');
                }
                else
                {
                    // Subsequent characters can be letters, digits, or underscores
                    if (char.IsLetterOrDigit(c) || c == '_')
                        sb.Append(c);
                    else
                        sb.Append('_');
                }
            }

            return sb.Length > 0 ? sb.ToString() : "_Model";
        }

        private static void WriteRegistryFile(
            List<(string Name, string RelativePath)> models,
            string outputPath)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("// This file was generated by ModelRegistryGenerator.");
            sb.AppendLine("// Do not modify this file manually.");
            sb.AppendLine("// Este archivo fue generado por ModelRegistryGenerator.");
            sb.AppendLine("// No modifiques este archivo manualmente.");
            sb.AppendLine("// </auto-generated>");
            sb.AppendLine();
            sb.AppendLine("using Microsoft.Xna.Framework.Graphics;");
            sb.AppendLine("using YotsubaEngine.Core.System.YotsubaEngineCore;");
            sb.AppendLine("using YotsubaEngine.Graphics;");
            sb.AppendLine();
            sb.AppendLine("namespace YotsubaEngine.Graphics");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Auto-generated class containing all discovered 3D model assets.");
            sb.AppendLine("    /// Clase auto-generada que contiene todos los modelos 3D descubiertos.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class ModelRegistry : IModelRegistry");
            sb.AppendLine("    {");

            // Generate constructor that registers all models
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Initializes the model registry and registers all discovered models.");
            sb.AppendLine("        /// Inicializa el registro de modelos y registra todos los modelos descubiertos.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        public ModelRegistry()");
            sb.AppendLine("        {");
            sb.AppendLine("            // Register all discovered models / Registrar todos los modelos descubiertos");

            foreach (var model in models)
            {
                sb.AppendLine($"            RegisterModel(\"{model.RelativePath}\");");
            }

            sb.AppendLine("        }");
            sb.AppendLine();

            // Generate static accessors for each model with lazy loading
            if (models.Count > 0)
            {
                sb.AppendLine("        /// <summary>");
                sb.AppendLine("        /// Static accessors for 3D models (cached on first access).");
                sb.AppendLine("        /// Accesos estáticos para modelos 3D (cacheados en primer acceso).");
                sb.AppendLine("        /// </summary>");
                sb.AppendLine("        public static class Models");
                sb.AppendLine("        {");

                foreach (var model in models)
                {
                    string identifier = ToValidIdentifier(model.Name);
                    sb.AppendLine($"            private static Model _{identifier};");
                    sb.AppendLine($"            /// <summary>3D Model: {model.Name} ({model.RelativePath})</summary>");
                    sb.AppendLine($"            public static Model {identifier} => _{identifier} ??= YTBGlobalState.ContentManager.Load<Model>(\"{model.RelativePath}\");");
                    sb.AppendLine();
                }

                sb.AppendLine("        }");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            // Ensure directory exists for output
            var dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string fullOutputPath = outputPath + ".cs";
            File.WriteAllText(fullOutputPath, sb.ToString());
            //Console.WriteLine($"[ModelRegistry] Registry generated at: {fullOutputPath}");
            //Console.WriteLine($"[ModelRegistry] Found {models.Count} 3D models.");
        }
    }
}
