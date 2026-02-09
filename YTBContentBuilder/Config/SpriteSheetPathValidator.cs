using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace YotsubaEngine.YTBContentBuilder
{
    /// <summary>
    /// Validates sprite sheet XML imagepath entries against the Assets directory.
    /// Valida los imagepath de spritesheets contra la carpeta Assets.
    /// </summary>
    public static class SpriteSheetPathValidator
    {
        private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg" };

        public static void ValidateImagePaths(string assetsRoot)
        {
            if (string.IsNullOrWhiteSpace(assetsRoot) || !Directory.Exists(assetsRoot))
            {
                Console.WriteLine("[SpriteSheetValidator] Assets path inválido.");
                return;
            }

            foreach (var xmlFile in Directory.GetFiles(assetsRoot, "*.xml", SearchOption.AllDirectories))
            {
                try
                {
                    XDocument document = XDocument.Load(xmlFile);
                    XElement root = document.Root;
                    if (root == null || !string.Equals(root.Name.LocalName, "textureatlas", StringComparison.OrdinalIgnoreCase))
                        continue;

                    XAttribute imageAttr = root.Attribute("imagepath");
                    if (imageAttr == null || string.IsNullOrWhiteSpace(imageAttr.Value))
                        continue;

                    string originalPath = NormalizeRelativePath(imageAttr.Value);
                    string resolvedPath = ResolveImagePath(assetsRoot, xmlFile, originalPath);

                    if (string.IsNullOrWhiteSpace(resolvedPath))
                        continue;

                    if (!string.Equals(originalPath, resolvedPath, StringComparison.Ordinal))
                    {
                        imageAttr.Value = resolvedPath;
                        document.Save(xmlFile);
                        Console.WriteLine($"[SpriteSheetValidator] imagepath corregido: {xmlFile} -> {resolvedPath}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SpriteSheetValidator] Error en {xmlFile}: {ex.Message}");
                }
            }
        }

        private static string ResolveImagePath(string assetsRoot, string xmlFile, string imagePath)
        {
            string normalizedPath = StripAssetsPrefix(imagePath);

            string directMatch = FindExistingImagePath(assetsRoot, normalizedPath);
            if (!string.IsNullOrWhiteSpace(directMatch))
                return directMatch;

            string fileName = Path.GetFileName(normalizedPath);
            string xmlDirectory = Path.GetDirectoryName(xmlFile) ?? assetsRoot;

            string xmlFolderMatch = FindImageInDirectory(xmlDirectory, assetsRoot, fileName);
            if (!string.IsNullOrWhiteSpace(xmlFolderMatch))
                return xmlFolderMatch;

            return FindImageInAssets(assetsRoot, fileName);
        }

        private static string FindExistingImagePath(string assetsRoot, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            string normalized = NormalizeRelativePath(relativePath);

            if (Path.HasExtension(normalized))
            {
                string fullPath = Path.Combine(assetsRoot, normalized);
                if (File.Exists(fullPath))
                {
                    return NormalizeRelativePath(Path.ChangeExtension(normalized, null));
                }
            }

            foreach (var extension in ImageExtensions)
            {
                string candidate = normalized + extension;
                string fullPath = Path.Combine(assetsRoot, candidate);
                if (File.Exists(fullPath))
                {
                    return NormalizeRelativePath(Path.ChangeExtension(candidate, null));
                }
            }

            return string.Empty;
        }

        private static string FindImageInDirectory(string directory, string assetsRoot, string fileName)
        {
            foreach (var candidate in GetCandidateFileNames(fileName))
            {
                string fullPath = Path.Combine(directory, candidate);
                if (File.Exists(fullPath))
                {
                    string relative = Path.GetRelativePath(assetsRoot, fullPath);
                    return NormalizeRelativePath(Path.ChangeExtension(relative, null));
                }
            }

            return string.Empty;
        }

        private static string FindImageInAssets(string assetsRoot, string fileName)
        {
            foreach (var candidate in GetCandidateFileNames(fileName))
            {
                string[] matches = Directory.GetFiles(assetsRoot, candidate, SearchOption.AllDirectories);
                if (matches.Length > 0)
                {
                    string relative = Path.GetRelativePath(assetsRoot, matches[0]);
                    return NormalizeRelativePath(Path.ChangeExtension(relative, null));
                }
            }

            return string.Empty;
        }

        private static IEnumerable<string> GetCandidateFileNames(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                yield break;

            if (Path.HasExtension(fileName))
            {
                yield return fileName;
                yield break;
            }

            foreach (var extension in ImageExtensions)
            {
                yield return fileName + extension;
            }
        }

        private static string StripAssetsPrefix(string path)
        {
            string normalized = NormalizeRelativePath(path);

            if (normalized.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                return normalized["Assets/".Length..];
            }

            return normalized;
        }

        private static string NormalizeRelativePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            return path.Replace('\\', '/').TrimStart('/');
        }
    }
}
