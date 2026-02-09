using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace YotsubaEngine.Graphics
{
    /// <summary>
    /// Stores texture regions and animations from an atlas.
    /// Almacena regiones de textura y animaciones de un atlas.
    /// </summary>
    public class TextureAtlas(Texture2D texture2D)
    {

        /// <summary>
        /// Base texture used to extract regions.
        /// Texture2D base de la cual se extraeran todas sus regiones.
        /// </summary>
        public Texture2D Texture2D { get; set; } = texture2D;

        /// <summary>
        /// Dictionary of texture regions keyed by name.
        /// Diccionario con el arreglo de regiones de textura existentes en la Texture2D.
        /// </summary>
        public readonly Dictionary<string, TextureRegion> TextureRegions = [];

        /// <summary>
        /// Dictionary of animations loaded from XML.
        /// Diccionario con el arreglo de animaciones del xml
        /// </summary>
        private readonly Dictionary<string, Animation> TextureAnimations = [];

        /// <summary>
        /// Adds a texture region to the atlas.
        /// Metodo para añadir Regiones de Textura al diccionario.
        /// </summary>
        /// <param name="name">Region name. Nombre de la región.</param>
        /// <param name="x">X coordinate. Coordenada X.</param>
        /// <param name="y">Y coordinate. Coordenada Y.</param>
        /// <param name="width">Width of the region. Ancho de la región.</param>
        /// <param name="heigth">Height of the region. Alto de la región.</param>
        public void AddTextureRegion(string name, int x, int y, int width, int heigth)
        {
            TextureRegions[name] = new TextureRegion(Texture2D, x, y, width, heigth);
        }

        /// <summary>
        /// Removes a specific region by name.
        /// Para remover una region en especifico.
        /// </summary>
        /// <param name="name">Region name. Nombre de la región.</param>
        /// <returns>True if removed. True si se eliminó.</returns>
        public bool RemoveRegion(string name)
        {
            return TextureRegions.Remove(name);
        }

        /// <summary>
        /// Gets a region by name.
        /// Metodo para obtener una region por su nombre
        /// </summary>
        /// <param name="name">Region name. Nombre de la región.</param>
        /// <returns>The texture region. La región de textura.</returns>
        public TextureRegion GetRegion(string name)
        {
            return TextureRegions[name];
        }

        /// <summary>
        /// Adds an animation to the atlas.
        /// Metodo para añadir Animaciones al diccionario.
        /// </summary>
        /// <param name="name">Animation name. Nombre de la animación.</param>
        /// <param name="animation">Animation instance. Instancia de animación.</param>
        public void AddAnimation(string name, Animation animation)
        {
            TextureAnimations.Add(name, animation);
        }

        /// <summary>
        /// Gets an animation by name.
        /// Metodo para obtener una animacion por su nombre
        /// </summary>
        /// <param name="name">Animation name. Nombre de la animación.</param>
        /// <returns>The animation. La animación.</returns>
        public Animation GetAnimation(string name)
        {
            return TextureAnimations[name];
        }

        /// <summary>
        /// Removes an animation by name.
        /// Metodo para eliminar una animacion.
        /// </summary>
        /// <param name="name">Animation name. Nombre de la animación.</param>
        public void RemoveAnimation(string name)
        {
            TextureAnimations.Remove(name);
        }


        /// <summary>
        /// Clears all texture regions and animations.
        /// Para limpiar el arreglo de texturas.
        /// </summary>
        public void Clear()
        {
            TextureRegions.Clear();
            TextureAnimations.Clear();
        }


        /// <summary>
        /// Creates an empty texture atlas.
        /// Crea un atlas de texturas vacío.
        /// </summary>
        public TextureAtlas() : this(null)
        {
            
        }
    }
}
