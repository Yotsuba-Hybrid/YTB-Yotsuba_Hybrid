using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace YotsubaEngine.ActionFiles.XML_SpriteSheet_Files
{
    /// <summary>
    /// Construye y actualiza atlas de sprite sheets y metadatos XML
    /// <para>Builds and updates sprite sheet atlases and XML metadata</para>
    /// </summary>
    public class TexturePacker
    {
        /// <summary>
        /// Carga metadatos de imagen para las rutas proporcionadas.
        /// <para>Loads image metadata for the provided file paths.</para>
        /// </summary>
        /// <param name="images">Rutas de imagen a cargar. <para>Image paths to load.</para></param>
        /// <returns>Metadatos de sprites. <para>Sprite metadata.</para></returns>
        public static IEnumerable<SpriteInfo> GetImages(params string[] images)
        {
            foreach (var path in images)
            {
                using Image image = Image.Load(path);

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
        /// Calcula posiciones de sprites y tamaño del atlas.
        /// <para>Calculates packed sprite positions and atlas size.</para>
        /// </summary>
        /// <param name="sprites">Lista de sprites a empaquetar. <para>Sprites to pack.</para></param>
        /// <param name="maxAtlasWidth">Ancho máximo del atlas. <para>Maximum atlas width.</para></param>
        /// <param name="finalAtlasWidth">Ancho final calculado. <para>Calculated final width.</para></param>
        /// <param name="finalAtlasHeight">Alto final calculado. <para>Calculated final height.</para></param>
        public static void CalculatePositions(List<SpriteInfo> sprites, int maxAtlasWidth, out int finalAtlasWidth, out int finalAtlasHeight)
        {
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
        /// Genera una imagen de atlas a partir de sprites empaquetados.
        /// <para>Generates an atlas image from packed sprites.</para>
        /// </summary>
        /// <param name="sprites">Sprites ya posicionados. <para>Positioned sprites.</para></param>
        /// <param name="width">Ancho del atlas. <para>Atlas width.</para></param>
        /// <param name="height">Alto del atlas. <para>Atlas height.</para></param>
        /// <param name="outputPath">Ruta de salida del atlas. <para>Atlas output path.</para></param>
        public static void GenerateAtlas(List<SpriteInfo> sprites, int width, int height, string outputPath)
        {
            using (Image<Rgba32> atlasImage = new (width, height))
            {

                    // 3. Recorrer cada sprite info
                    foreach (var sprite in sprites)
                    {
                        // Cargar la imagen original desde el disco
                        // IMPORTANTE: Usamos 'using' para liberarla apenas la pintemos
                        using (Image currentImage = Image.Load(sprite.Path))
                        {
                        	if(currentImage.Width != sprite.Width || currentImage.Height != sprite.Height)
                        	{
                        			currentImage.Mutate(x => x.Resize(sprite.Width, sprite.Height));
                        	}
                        
                        	atlasImage.Mutate(m => m.DrawImage(currentImage, new Point(sprite.X, sprite.Y), 1f));
                        }
                    
                }

                atlasImage.SaveAsPng(outputPath);
            }
        }

        /// <summary>
        /// Exporta datos de sprites al formato XML esperado.
        /// <para>Exports sprite data to the expected XML format.</para>
        /// </summary>
        /// <param name="sprites">Sprites a exportar. <para>Sprites to export.</para></param>
        /// <param name="atlasFileName">Nombre del atlas. <para>Atlas file name.</para></param>
        /// <param name="xmlOutputPath">Ruta de salida del XML. <para>XML output path.</para></param>
        /// <param name="imageName">Nombre de la imagen. <para>Image name.</para></param>
     public static void ExportXML(List<SpriteInfo> sprites, string atlasFileName, string xmlOutputPath, string imageName)
	{
	    string normalizedImageName = imageName;
	    normalizedImageName = Path.ChangeExtension(normalizedImageName, null);
	
	    XElement root = new XElement("textureatlas", new XAttribute("imagepath", normalizedImageName));
	
	    foreach (var sprite in sprites)
	    {
	        XElement subtexture = new XElement("subtexture",
	            new XAttribute("name", sprite.Name),
	            new XAttribute("x", sprite.X),
	            new XAttribute("y", sprite.Y),
	            new XAttribute("width", sprite.Width),
	            new XAttribute("height", sprite.Height)
	        );
	        root.Add(subtexture);
	    }
	
	    // Preservar los nodos <animation> del XML anterior de forma segura
	    if (File.Exists(xmlOutputPath))
	    {
	        try
	        {
	            XDocument oldDoc = XDocument.Load(xmlOutputPath);
	            var animationNodes = oldDoc.Root?.Elements("animation");
	            if (animationNodes != null && animationNodes.Any())
	            {
	                root.Add(animationNodes); // Agrega todos los nodos de animación al nuevo root
	            }
	        }
	        catch 
	        { 
	            // Manejar o registrar si el XML anterior estaba corrupto
	        }
	    }
	
	    XDocument doc = new XDocument(root);
	    doc.Save(xmlOutputPath); // Guarda todo en un solo paso, perfectamente formateado
	}

        /// <summary>
        /// Separa un atlas en imágenes de sprites individuales.
        /// <para>Splits an atlas into individual sprite images.</para>
        /// </summary>
        /// <param name="xmlPath">Ruta del XML del atlas. <para>Atlas XML path.</para></param>
        /// <param name="atlasImagePath">Ruta de la imagen del atlas. <para>Atlas image path.</para></param>
        /// <param name="outputFolder">Carpeta de salida. <para>Output folder.</para></param>
        public static void UnpackAtlas(string xmlPath, string atlasImagePath, string outputFolder)
        {
            XDocument doc = XDocument.Load(xmlPath);
            if(!atlasImagePath.EndsWith(".png")) atlasImagePath += ".png";
            string imagePath = Path.Combine(YTBGlobalState.DevelopmentAssetsPath, atlasImagePath);
            using (Image bigAtlas =  Image.Load(imagePath))
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

                    using (Image subImage = bigAtlas.Clone(c => c.Crop(cropArea)))
                    {
                        string finalPath = Path.Combine(outputFolder, name + ".png");
                        subImage.SaveAsPng(finalPath);
                    }
                }
            }
        }


        /// <summary>
        /// Actualiza un atlas existente fusionando nuevas imágenes.
        /// <para>Updates an existing atlas by merging new images.</para>
        /// </summary>
        /// <param name="existingXmlPath">Ruta del XML existente. <para>Existing XML path.</para></param>
        /// <param name="newImagesPaths">Rutas de nuevas imágenes. <para>New image paths.</para></param>
        /// <param name="maxAtlasWidth">Ancho máximo del atlas. <para>Maximum atlas width.</para></param>
        public static void UpdateAtlas(string existingXmlPath, string[] newImagesPaths, int maxAtlasWidth = 2048)
        {
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

                // relativePath ya es relativo a DevelopmentAssetsPath, usarlo directamente
                UnpackAtlas(existingXmlPath, relativePath, tempFolder);

                // Construir la ruta absoluta asegurando una sola extensión .png
                string imagePath = Path.Combine(YTBGlobalState.DevelopmentAssetsPath, relativePath);
                imagePath = Path.ChangeExtension(imagePath, ".png");

                using (var fs = File.OpenRead(imagePath))
                {
                   	var tmp = Image.Identify(fs);
                    
                        if(tmp.Width > maxAtlasWidth)
                        maxAtlasWidth = tmp.Width;
                    
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
    }
}