using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Graphics
{
    /// <summary>
    /// Proporciona fábricas auxiliares para recursos gráficos comunes.
    /// <para>Provides helper factories for common graphics assets.</para>
    /// </summary>
    public static class YotsubaGraphicsManager
    {

        internal static readonly Dictionary<string, Texture2D> PreloadedTextures = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, SpriteFont> PreloadedFonts = new(StringComparer.OrdinalIgnoreCase);

//-:cnd:noEmit
#if YTB
        /// <summary>
        /// Dictionary storing all texture regions from all loaded texture atlases.
        /// Key: "AtlasName/RegionName", Value: TextureRegion
        /// Only available in DEBUG mode for engine editor purposes.
        /// </summary>
        internal static readonly Dictionary<string, TextureRegion> PreloadedTextureRegions = new(StringComparer.OrdinalIgnoreCase);
#endif
//+:cnd:noEmit

        /// <summary>
        /// Precarga texturas y las almacena en caché.
        /// <para>Preloads textures and caches them.</para>
        /// </summary>
        /// <param name="textures">Rutas de texturas a cargar. <para>Texture paths to load.</para></param>
        public static void PreloadTextures(params string[] textures)
        {
            foreach (var texture in textures)
            {
                _ = GetTexture(texture);
            }
        }

        /// <summary>
        /// Precarga texturas y fuentes proporcionadas.
        /// <para>Preloads the provided textures and fonts.</para>
        /// </summary>
        /// <param name="textures">Rutas de texturas a cargar. <para>Texture paths to load.</para></param>
        /// <param name="fonts">Rutas de fuentes a cargar. <para>Font paths to load.</para></param>
        public static void InitializeAssets(IEnumerable<string> textures, IEnumerable<string> fonts)
        {
            if (textures != null)
            {
                foreach (var tex in textures)
                {
                    _ = GetTexture(tex);
                }
            }

            if (fonts != null)
            {
                foreach (var font in fonts)
                {
                    _ = GetFont(font);
                }
            }
        }

        internal static Texture2D GetTexture(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName)) throw new ArgumentException("assetName no puede ser nulo", nameof(assetName));

            assetName = assetName.Replace('\\', '/');

            if (PreloadedTextures.TryGetValue(assetName, out var texture))
            {
                return texture;
            }

            var loaded = YTBGlobalState.ContentManager.Load<Texture2D>(assetName);
            PreloadedTextures[assetName] = loaded;
            return loaded;
        }

        internal static SpriteFont GetFont(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName)) throw new ArgumentException("assetName no puede ser nulo", nameof(assetName));

            assetName = assetName.Replace('\\', '/');

            if (PreloadedFonts.TryGetValue(assetName, out var font))
            {
                return font;
            }

            var loaded = YTBGlobalState.ContentManager.Load<SpriteFont>(assetName);
            PreloadedFonts[assetName] = loaded;
            return loaded;
        }

        /// <summary>
        /// Helper that builds a texture atlas from XML.
        /// Helper que facilita la creacion de Atlas de texturas a partir de un XML.
        /// </summary>
        /// <param name="contentManager">Content manager to load assets. ContentManager para cargar recursos.</param>
        /// <param name="xmlFilePath">XML path for the atlas. Ruta XML del atlas.</param>
        /// <returns>The built texture atlas. El atlas de textura construido.</returns>
        internal static TextureAtlas TextureAtlasGenerator(ContentManager contentManager, string xmlFilePath)
        {
			TextureAtlas textureAtlas = new();

			// 1. CORRECCIÓN: Normalizar rutas para Android (si el string viene con '\' de Windows)
			xmlFilePath = xmlFilePath.Replace('\\', '/');

			// Combina con el Root (ej: "Content")
			string filePath = Path.Combine(contentManager.RootDirectory, xmlFilePath);

			// Asegurar compatibilidad de Path.Combine en Android si RootDirectory no termina en '/'
			filePath = filePath.Replace('\\', '/');

			if (String.IsNullOrEmpty(xmlFilePath)) return textureAtlas;

			// TitleContainer funciona perfecto aquí para leer el XML crudo
			using (Stream stream = TitleContainer.OpenStream(filePath))
			{
				using (XmlReader reader = XmlReader.Create(stream))
				{
					XDocument document = XDocument.Load(reader);

					string texturePath = document.Root.Attribute("imagepath").Value;

					// 2. CORRECCIÓN CRÍTICA: Content.Load NO quiere extensiones (.png, .jpg)
					// Si el XML dice "slime.png", hay que quitarle el ".png" para cargar el .xnb
					if (Path.HasExtension(texturePath))
					{
						texturePath = Path.ChangeExtension(texturePath, null);
					}

					// 3. Ajustar ruta relativa de la textura
					// A veces el XML asume que la textura está al lado.
					// Si xmlFilePath es "animations/slime.xml", la textura debe buscarse en "animations/slime"
					//string directory = Path.GetDirectoryName(xmlFilePath);
					//if (!string.IsNullOrEmpty(directory))
					//{
					//	// Combinamos carpeta del XML + nombre de textura limpia
					//	texturePath = Path.Combine(directory, texturePath).Replace('\\', '/');
					//}

					// Ahora sí cargará "Content/animations/slimeNaranja" (el .xnb)
					textureAtlas.Texture2D = contentManager.Load<Texture2D>(texturePath);


					foreach (var subTexture in document.Root.Elements("subtexture"))
                    {
                        string name = subTexture.Attribute("name")?.Value ?? "Sin Nombre";
                        int x = Convert.ToInt32(subTexture.Attribute("x")?.Value ?? "0");
                        int y = Convert.ToInt32(subTexture.Attribute("y")?.Value ?? "0");
                        int width = Convert.ToInt32(subTexture.Attribute("width")?.Value ?? "0");
                        int height = Convert.ToInt32(subTexture.Attribute("height")?.Value ?? "0");

                        textureAtlas.AddTextureRegion(name, x, y, width, height);
                    }

                    foreach (var animation in document.Root.Elements("animation"))
                    {

                        string name = animation.Attribute("name").Value ?? "Sin Nombre";
                        float delayInMiliseconds = float.Parse(animation.Attribute("delay")?.Value ?? "0");
                        TimeSpan delay = TimeSpan.FromMilliseconds(delayInMiliseconds);

                        List<TextureRegion> frames = [];

                        var frameElements = animation.Elements("Frame");

                        if(frameElements != null)
                        {
                            foreach (var frame in frameElements)
                            {
                                string regionName = frame.Attribute("region").Value;
                                TextureRegion region = textureAtlas.GetRegion(regionName);

                                frames.Add(region);
                            }
                        }

                        Animation Animation = new([.. frames], delay);
                        textureAtlas.AddAnimation(name, Animation);

                    }

                }
            }

//-:cnd:noEmit
#if YTB
            // Store all texture regions in the global dictionary for the engine editor
            // Format: "xmlPath/regionName" -> TextureRegion
            string atlasKey = xmlFilePath.Replace('\\', '/');
            foreach (var region in textureAtlas.TextureRegions)
            {
                string fullKey = $"{atlasKey}/{region.Key}";
                PreloadedTextureRegions[fullKey] = region.Value;
            }
#endif
//+:cnd:noEmit

            return textureAtlas;
        }


        /// <summary>
        /// Helper that creates a sprite component.
        /// Helper que facilita la creacion de un componente de sprite
        /// </summary>
        /// <param name="texture2D">Texture to render. Textura a renderizar.</param>
        /// <param name="renderLimit">Texture region to render. Zona a renderizar de la textura.</param>
        /// <returns>A sprite component. Devuelve un componente de sprite.</returns>
        internal static SpriteComponent2D SpriteGenerator(TextureRegion textureRegion)
        {
            return new SpriteComponent2D(textureRegion.Texture, textureRegion.SourceRectangle);
        }

        /// <summary>
        /// Helper that creates a transform component.
        /// Helper que facilita la creacion de un componente de Transformacion
        /// </summary>
        /// <param name="position">World position. Posición en mundo.</param>
        /// <param name="scale">Scale value. Valor de escala.</param>
        /// <returns>A transform component. Devuelve un componente de Transformacion.</returns>
        internal static TransformComponent TransformGenerator(Vector3 position, float scale, Vector3 size)
        {
            return new TransformComponent(position, size, scale);
        }
    }
}
