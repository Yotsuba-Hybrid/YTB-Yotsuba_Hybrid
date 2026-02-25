using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YotsubaEngine.YTBContentBuilder.Audio
{
    /// <summary>
    /// Generates a static audio registry class by scanning the Assets folder for audio files.
    /// Genera una clase de registro de audio estático escaneando la carpeta Assets en busca de archivos de audio.
    /// </summary>
    public static class AudioRegistryGenerator
    {
        /// <summary>
        /// Audio file extensions that are supported by MonoGame.
        /// Extensiones de archivos de audio soportadas por MonoGame.
        /// </summary>
        private static readonly string[] SoundEffectExtensions = { ".wav", ".ogg" };
        private static readonly string[] SongExtensions = { ".mp3", ".ogg", ".wma" };
        private static readonly string[] AllAudioExtensions = { ".wav", ".mp3", ".ogg", ".wma" };

        /// <summary>
        /// Generates the audio registry partial class by scanning the source path for audio files.
        /// Genera la clase parcial del registro de audio escaneando la ruta de origen en busca de archivos de audio.
        /// </summary>
        /// <param name="sourcePath">Path to the Assets folder to scan.</param>
        /// <param name="outputPath">Path where the generated file will be written (without extension).</param>
        public static void GenerateRegistry(string sourcePath, string outputPath)
        {
            //Console.WriteLine($"[AudioRegistry] Scanning for audio files in: {sourcePath}");

            var soundEffects = new List<(string Name, string RelativePath)>();
            var songs = new List<(string Name, string RelativePath)>();

            // Scan for audio files
            foreach (var extension in AllAudioExtensions)
            {
                string pattern = "*" + extension;
                string[] files;
                
                try
                {
                    files = Directory.GetFiles(sourcePath, pattern, SearchOption.AllDirectories);
                }
                catch (DirectoryNotFoundException)
                {
                    //Console.WriteLine($"[AudioRegistry] Directory not found: {sourcePath}");
                    continue;
                }

                foreach (var file in files)
                {
                    // Skip bin/obj folders
                    if (file.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar) ||
                        file.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
                    {
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string fileExtension = Path.GetExtension(file).ToLowerInvariant();

                    // Calculate relative path from Assets folder (for content loading)
                    string relativePath = GetRelativePathForContent(file, sourcePath);

                    // Determine if it's a sound effect or song based on extension and folder structure
                    bool isSong = IsSongFile(file, fileExtension);

                    if (isSong)
                    {
                        if (!songs.Exists(s => s.Name == fileName))
                        {
                            songs.Add((fileName, relativePath));
                            //Console.WriteLine($"[AudioRegistry] Song found: {fileName} ({relativePath})");
                        }
                    }
                    else
                    {
                        if (!soundEffects.Exists(s => s.Name == fileName))
                        {
                            soundEffects.Add((fileName, relativePath));
                            //Console.WriteLine($"[AudioRegistry] SoundEffect found: {fileName} ({relativePath})");
                        }
                    }
                }
            }

            WriteRegistryFile(soundEffects, songs, outputPath);
        }

        /// <summary>
        /// Determines if a file should be treated as a Song based on extension and folder structure.
        /// Determina si un archivo debe tratarse como una canción basándose en la extensión y estructura de carpetas.
        /// </summary>
        private static bool IsSongFile(string filePath, string extension)
        {
            // MP3 files are typically songs
            if (extension == ".mp3" || extension == ".wma")
                return true;

            // Check folder structure for hints
            string lowerPath = filePath.ToLowerInvariant();
            if (lowerPath.Contains("music") || lowerPath.Contains("songs") || lowerPath.Contains("soundtrack") || lowerPath.Contains("bgm"))
                return true;

            // OGG can be either, but if in specific folders, treat as sound effect
            if (lowerPath.Contains("sfx") || lowerPath.Contains("sounds") || lowerPath.Contains("effects"))
                return false;

            // WAV files are typically sound effects
            if (extension == ".wav")
                return false;

            // Default OGG to song if not in sound effect folders
            return extension == ".ogg";
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
            
            return sb.Length > 0 ? sb.ToString() : "_Audio";
        }

        private static void WriteRegistryFile(
            List<(string Name, string RelativePath)> soundEffects,
            List<(string Name, string RelativePath)> songs,
            string outputPath)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("// This file was generated by AudioRegistryGenerator.");
            sb.AppendLine("// Do not modify this file manually.");
            sb.AppendLine("// Este archivo fue generado por AudioRegistryGenerator.");
            sb.AppendLine("// No modifiques este archivo manualmente.");
            sb.AppendLine("// </auto-generated>");
            sb.AppendLine();
            sb.AppendLine("using Microsoft.Xna.Framework.Audio;");
            sb.AppendLine("using Microsoft.Xna.Framework.Media;");
            sb.AppendLine("using YotsubaEngine.Audio;");
            sb.AppendLine("using YotsubaEngine.Core.YotsubaGame;");
            sb.AppendLine("using YotsubaEngine.Core.System.YotsubaEngineCore;");
            sb.AppendLine();
            sb.AppendLine("namespace YotsubaEngine.Audio");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Auto-generated partial class containing all discovered audio assets.");
            sb.AppendLine("    /// Clase parcial auto-generada que contiene todos los activos de audio descubiertos.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public static partial class AudioAssets");
            sb.AppendLine("    {");

            // Generate static partial method implementation
            sb.AppendLine("        public static partial void InitializeAudioAssets();");
            sb.AppendLine("        public static partial void InitializeAudioAssets()");
            sb.AppendLine("        {");
            sb.AppendLine("            // Register sound effects / Registrar efectos de sonido");
            
            foreach (var sfx in soundEffects)
            {
                sb.AppendLine($"            IAudioRegistry.SoundEffects.TryAdd(\"{sfx.Name}\", () => YTBGlobalState.ContentManager.Load<SoundEffect>(\"{sfx.RelativePath}\"));");
            }
            
            sb.AppendLine();
            sb.AppendLine("            // Register songs / Registrar canciones");
            
            foreach (var song in songs)
            {
                sb.AppendLine($"            IAudioRegistry.Songs.TryAdd(\"{song.Name}\", () => YTBGlobalState.ContentManager.Load<Song>(\"{song.RelativePath}\"));");
            }
            
            sb.AppendLine("        }");
            sb.AppendLine();
            
            // Generate static accessor classes for easy access with lazy caching
            if (soundEffects.Count > 0)
            {
                sb.AppendLine("        /// <summary>");
                sb.AppendLine("        /// Static accessors for sound effects (cached on first access).");
                sb.AppendLine("        /// Accesos estáticos para efectos de sonido (cacheados en primer acceso).");
                sb.AppendLine("        /// </summary>");
                sb.AppendLine("        public static class SFX");
                sb.AppendLine("        {");
                
                foreach (var sfx in soundEffects)
                {
                    string identifier = ToValidIdentifier(sfx.Name);
                    sb.AppendLine($"            private static SoundEffect _{identifier};");
                    sb.AppendLine($"            /// <summary>Sound effect: {sfx.Name}</summary>");
                    sb.AppendLine($"            public static SoundEffect {identifier} => _{identifier} ??= YTBGlobalState.ContentManager.Load<SoundEffect>(\"{sfx.RelativePath}\");");
                }
                
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            
            if (songs.Count > 0)
            {
                sb.AppendLine("        /// <summary>");
                sb.AppendLine("        /// Static accessors for songs/music (cached on first access).");
                sb.AppendLine("        /// Accesos estáticos para canciones/música (cacheados en primer acceso).");
                sb.AppendLine("        /// </summary>");
                sb.AppendLine("        public static class Music");
                sb.AppendLine("        {");
                
                foreach (var song in songs)
                {
                    string identifier = ToValidIdentifier(song.Name);
                    sb.AppendLine($"            private static Song _{identifier};");
                    sb.AppendLine($"            /// <summary>Song: {song.Name}</summary>");
                    sb.AppendLine($"            public static Song {identifier} => _{identifier} ??= YTBGlobalState.ContentManager.Load<Song>(\"{song.RelativePath}\");");
                }
                
                sb.AppendLine("        }");
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");

            string fullOutputPath = outputPath + ".cs";
            File.WriteAllText(fullOutputPath, sb.ToString());
            //Console.WriteLine($"[AudioRegistry] Registry generated at: {fullOutputPath}");
            //Console.WriteLine($"[AudioRegistry] Found {soundEffects.Count} sound effects and {songs.Count} songs.");
        }
    }
}
