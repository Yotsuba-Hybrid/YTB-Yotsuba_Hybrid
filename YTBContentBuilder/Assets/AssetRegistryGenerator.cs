using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YotsubaEngine.YTBContentBuilder.Assets
{
    public static class AssetRegistryGenerator
    {
        private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif" };
        private static readonly string[] FontExtensions = { ".spritefont" };

        public static void GenerateRegistry(string sourcePath, string outputPath)
        {
            Console.WriteLine($"[AssetRegistry] Scanning for assets in: {sourcePath}");

            var images = ScanFiles(sourcePath, ImageExtensions);
            var fonts = ScanFiles(sourcePath, FontExtensions);

            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine("namespace SandBoxGame.Core");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Auto-generated class listing all game assets (textures and fonts).");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public static class AssetRegister");
            sb.AppendLine("    {");

            sb.AppendLine("        public static readonly List<string> TextureAssets = new List<string>");
            sb.AppendLine("        {");
            foreach (var item in images) sb.AppendLine($"            \"{item}\",");
            sb.AppendLine("        };");

            sb.AppendLine();

            sb.AppendLine("        public static readonly List<string> FontAssets = new List<string>");
            sb.AppendLine("        {");
            foreach (var item in fonts) sb.AppendLine($"            \"{item}\",");
            sb.AppendLine("        };");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            // Ensure directory exists for output
            var dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(outputPath + ".cs", sb.ToString());
            Console.WriteLine($"[AssetRegistry] Generated {outputPath}.cs");
        }

        private static List<string> ScanFiles(string root, string[] extensions)
        {
            var results = new List<string>();
            if (!Directory.Exists(root)) return results;

            foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.AllDirectories))
            {
                if (extensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
                {
                    var rel = Path.GetRelativePath(root, file);
                    rel = Path.ChangeExtension(rel, null);
                    rel = rel.Replace("\\", "/");
                    results.Add(rel);
                }
            }
            results.Sort();
            return results;
        }
    }
}