using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.System.YotsubaEngineCore;

namespace YotsubaEngine.ActionFiles.XML_SpriteSheet_Files
{
    /// <summary>
    /// Builds and updates sprite sheet atlases and XML metadata (Windows-only due to System.Drawing).
    /// Construye y actualiza atlas de sprite sheets y metadatos XML (solo Windows por System.Drawing).
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class TexturePacker
    {
        /// <summary>
        /// Loads image metadata for the provided file paths.
        /// Carga metadatos de imagen para las rutas proporcionadas.
        /// </summary>
        public static IEnumerable<SpriteInfo> GetImages(params string[] images)
        {
            EnsureWindowsSupport();
            foreach (var path in images)
            {
                using var image = Image.FromFile(path);

                SpriteInfo info = new SpriteInfo()
                {
                    Height = image.Height,
                    Width = image.Width,
                    Path = path,
                    Name = Path.GetFileNameWithoutExtension(path)
                };

                yield return info;
            }
        }

        /// <summary>
        /// Calculates packed sprite positions and atlas size.
        /// Calcula posiciones de sprites y tamaño del atlas.
        /// </summary>
        public static void CalculatePositions(List<SpriteInfo> sprites, int maxAtlasWidth, out int finalAtlasWidth, out int finalAtlasHeight)
        {
            EnsureWindowsSupport();
            sprites.Sort((s1, s2) => s2.Height.CompareTo(s1.Height));

            int currentX = 0;
            int currentY = 0;
            int maxRowHeight = 0;

            finalAtlasWidth = 0;

            foreach (var sprite in sprites)
            {
                if (currentX + sprite.Width > maxAtlasWidth)
                {
                    currentX = 0;
                    currentY += maxRowHeight;
                    maxRowHeight = 0;
                }

                sprite.X = currentX;
                sprite.Y = currentY;

                currentX += sprite.Width;

                maxRowHeight = Math.Max(maxRowHeight, sprite.Height);

                finalAtlasWidth = Math.Max(finalAtlasWidth, currentX);
            }

            finalAtlasHeight = currentY + maxRowHeight;
        }

        /// <summary>
        /// Generates an atlas image from packed sprites.
        /// Genera una imagen de atlas a partir de sprites empaquetados.
        /// </summary>
        public static void GenerateAtlas(List<SpriteInfo> sprites, int width, int height, string outputPath)
        {
            EnsureWindowsSupport();
            using (var atlasBitmap = new Bitmap(width, height))
            {
                using (var g = System.Drawing.Graphics.FromImage(atlasBitmap))
                {
                    g.Clear(Color.Transparent);

                    // 3. Recorrer cada sprite info
                    foreach (var sprite in sprites)
                    {
                        // Cargar la imagen original desde el disco
                        // IMPORTANTE: Usamos 'using' para liberarla apenas la pintemos
                        using (var currentImage = Image.FromFile(sprite.Path))
                        {
                            g.DrawImage(currentImage, sprite.X, sprite.Y, sprite.Width, sprite.Height);
                        }
                    }
                }

                outputPath = outputPath.Replace("/",@"\\");
                atlasBitmap.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>
        /// Exports sprite data to the expected XML format.
        /// Exporta datos de sprites al formato XML esperado.
        /// </summary>
        public static void ExportXML(List<SpriteInfo> sprites, string atlasFileName, string xmlOutputPath, string imageName)
        {
            EnsureWindowsSupport();
            string animationContent = string.Empty;
            if (File.Exists(xmlOutputPath))
            {
                string xml = File.ReadAllText(xmlOutputPath);
                int animationIndex = xml.IndexOf("<animation", StringComparison.OrdinalIgnoreCase);
                if (animationIndex >= 0)
                {
                    animationContent = xml.Substring(animationIndex);
                }
            }

            string normalizedImageName = imageName.Replace('\\', '/');
            normalizedImageName = Path.ChangeExtension(normalizedImageName, null);

            XElement root = new XElement("textureatlas", new XAttribute("imagepath", normalizedImageName));

            foreach (var sprite in sprites)
            {
                XElement subtexture = new XElement("subtexture");
                subtexture.Add(new XAttribute("name", sprite.Name)); 
                subtexture.Add(new XAttribute("x", sprite.X)); 
                subtexture.Add(new XAttribute("y", sprite.Y));
                subtexture.Add(new XAttribute("width", sprite.Width)); 
                subtexture.Add(new XAttribute("height", sprite.Height)); 
                root.Add(subtexture);
            }

            XDocument doc = new XDocument(root);
            doc.Save(xmlOutputPath);
            if (!string.IsNullOrWhiteSpace(animationContent))
            {
                string newContent = File.ReadAllText(xmlOutputPath);
                newContent = newContent.Replace("</textureatlas>", "");
                string finalContent = newContent + animationContent;
                File.WriteAllText(xmlOutputPath, finalContent);
            }
        }

        /// <summary>
        /// Splits an atlas into individual sprite images.
        /// Separa un atlas en imágenes de sprites individuales.
        /// </summary>
        public static void UnpackAtlas(string xmlPath, string atlasImagePath, string outputFolder)
        {
            EnsureWindowsSupport();
            XDocument doc = XDocument.Load(xmlPath);
            atlasImagePath = atlasImagePath.Replace("/", "\\");
            if(!atlasImagePath.EndsWith(".png")) atlasImagePath += ".png";
            string imagePath = Path.Combine(YTBGlobalState.DevelopmentAssetsPath, atlasImagePath);
            using (var bigAtlas = new Bitmap(imagePath))
            {
                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                foreach (var element in doc.Descendants("subtexture"))
                {
                    // Leer atributos
                    string name = element.Attribute("name").Value;
                    int x = int.Parse(element.Attribute("x").Value);
                    int y = int.Parse(element.Attribute("y").Value);
                    int w = int.Parse(element.Attribute("width").Value);
                    int h = int.Parse(element.Attribute("height").Value);

                    Rectangle cropArea = new Rectangle(x, y, w, h);

                    using (var subImage = bigAtlas.Clone(cropArea, bigAtlas.PixelFormat))
                    {
                        string finalPath = Path.Combine(outputFolder, name + ".png");
                        subImage.Save(finalPath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }


        /// <summary>
        /// Updates an existing atlas by merging new images.
        /// Actualiza un atlas existente fusionando nuevas imágenes.
        /// </summary>
        public static void UpdateAtlas(string existingXmlPath, string[] newImagesPaths, int maxAtlasWidth = 2048)
        {
            EnsureWindowsSupport();
            // Definimos una carpeta temporal al lado del archivo XML original
            string tempFolder = Path.Combine(Path.GetDirectoryName(existingXmlPath), "Temp_Processing_Atlas");

            try
            {
                XDocument doc = XDocument.Load(existingXmlPath);

                // Normalizar imagepath leído del XML
                string relativePath = doc.Element("textureatlas").Attribute("imagepath").Value;
                relativePath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()).Trim();

                // Quitar cualquier sufijo .png para evitar duplicados
                if (relativePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    relativePath = relativePath.Substring(0, relativePath.Length - 4);

                string lastPath = existingXmlPath.Split(YTBFileToGameData.ContentManager.RootDirectory).LastOrDefault();

                // Si el último segmento path es vacío, significa que la carpeta ya estaba normalizada
                if (string.IsNullOrEmpty(lastPath) || lastPath == "\\" || lastPath == "/")
                    lastPath = YTBFileToGameData.ContentManager.RootDirectory;
                else
                {
                    if (lastPath.StartsWith("\\")) lastPath = lastPath.Substring(1);
                    if (lastPath.StartsWith("/")) lastPath = lastPath.Substring(1);
                }

                // Construir imagePath relativo y pasar a UnpackAtlas
                string imagePath = existingXmlPath.Replace(lastPath, relativePath);
                UnpackAtlas(existingXmlPath, imagePath, tempFolder);

                // Construir la ruta absoluta asegurando una sola extensión .png
                imagePath = Path.Combine(YTBGlobalState.DevelopmentAssetsPath, imagePath);
                imagePath = Path.ChangeExtension(imagePath, ".png");

                using (var fs = File.OpenRead(imagePath))
                {
                    using (var tmp = Image.FromStream(fs))
                    {
                        if(tmp.Width > maxAtlasWidth)
                        maxAtlasWidth = tmp.Width;
                    }
                }
                // PASO 2: FUSIONAR
                // Copiamos las imágenes NUEVAS a esa misma carpeta temporal
                foreach (var imgPath in newImagesPaths)
                {
                    string fileName = Path.GetFileName(imgPath);
                    string destPath = Path.Combine(tempFolder, fileName);

                    // 'true' permite sobrescribir si la imagen nueva tiene el mismo nombre que una vieja
                    File.Copy(imgPath, destPath, true);
                }

                // PASO 3: LECTURA TOTAL (GetImages)
                // Leemos TODOS los archivos (viejos + nuevos) de la carpeta temporal
                string[] allFiles = Directory.GetFiles(tempFolder, "*.png");
                // Convertimos a lista para poder manipularla (GetImages devuelve IEnumerable)
                List<SpriteInfo> allSprites = new List<SpriteInfo>(GetImages(allFiles));

                // PASO 4: CALCULAR (CalculatePositions)
                // Volvemos a calcular la matemática de posición con todas las imágenes juntas
                int finalW, finalH;
                CalculatePositions(allSprites, maxAtlasWidth, out finalW, out finalH);

                // PASO 5: GENERAR IMAGEN (GenerateAtlas)
                // Creamos el nuevo PNG gigante (Sobrescribe el original)
                GenerateAtlas(allSprites, finalW, finalH, imagePath);

                // PASO 6: EXPORTAR XML (ExportXML)
                // Creamos el nuevo XML actualizado (Sobrescribe el original)
                // Nota: El segundo parámetro 'atlasFileName' no se usa en tu implementación actual de ExportXML, 
                // pasamos null o string vacía, y usamos el último parámetro para el nombre de la imagen.


                List<string> carpetas = new();
                carpetas.Add(YTBFileToGameData.ContentManager.RootDirectory);
                carpetas.Add(YTBGlobalState.CompiledAssetsFolderName);
                carpetas.Add("Assets");

                string xmlPath = YTBGlobalState.DevelopmentAssetsPath + existingXmlPath.Split(carpetas.ToArray(),StringSplitOptions.None).LastOrDefault();

                string relativeImagePath = Path.GetRelativePath(YTBGlobalState.DevelopmentAssetsPath, imagePath);
                relativeImagePath = relativeImagePath.Replace('\\', '/');
                ExportXML(allSprites, "", xmlPath, relativeImagePath);

                YTBContentBuilder.Rebuild();
            }
            catch (Exception ex)
            {
                // Si algo falla, lanzamos el error para enterarnos
                throw new Exception("Error al actualizar el Atlas: " + ex.Message);
            }
            finally
            {
                // PASO 7: LIMPIEZA
                // El bloque 'finally' asegura que esto se ejecute SIEMPRE, haya error o no.
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true); // true borra archivos y subcarpetas
                }
            }
        }

        /// <summary>
        /// Ensures System.Drawing usage only occurs on Windows platforms.
        /// Garantiza que el uso de System.Drawing solo ocurra en plataformas Windows.
        /// </summary>
        private static void EnsureWindowsSupport()
        {
            if (!OperatingSystem.IsWindows())
            {
                throw new PlatformNotSupportedException("TexturePacker uses System.Drawing and requires Windows. Use a cross-platform image library for non-Windows platforms.");
            }
        }
    }
}
