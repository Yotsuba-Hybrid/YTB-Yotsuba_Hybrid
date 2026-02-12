using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace YotsubaEngine.Graphics
{
    /// <summary>
    /// Almacena regiones de textura y animaciones de un atlas.
    /// <para>Stores texture regions and animations from an atlas.</para>
    /// </summary>
    /// <param name="texture2D">Textura base del atlas. <para>Base texture for the atlas.</para></param>
    public class TextureAtlas(Texture2D texture2D)
    {

        /// <summary>
        /// Texture2D base de la cual se extraen todas sus regiones.
        /// <para>Base texture used to extract regions.</para>
        /// </summary>
        public Texture2D Texture2D { get; set; } = texture2D;

        /// <summary>
        /// Diccionario con el arreglo de regiones de textura existentes en la Texture2D.
        /// <para>Dictionary of texture regions keyed by name.</para>
        /// </summary>
        public readonly Dictionary<string, TextureRegion> TextureRegions = [];

        /// <summary>
        /// Dictionary of animations loaded from XML.
        /// Diccionario con el arreglo de animaciones del xml
        /// </summary>
        private readonly Dictionary<string, Animation> TextureAnimations = [];

        /// <summary>
        /// Añade regiones de textura al diccionario.
        /// <para>Adds texture regions to the atlas.</para>
        /// </summary>
        /// <param name="name">Nombre de la región. <para>Region name.</para></param>
        /// <param name="x">Coordenada X. <para>X coordinate.</para></param>
        /// <param name="y">Coordenada Y. <para>Y coordinate.</para></param>
        /// <param name="width">Ancho de la región. <para>Width of the region.</para></param>
        /// <param name="heigth">Alto de la región. <para>Height of the region.</para></param>
        public void AddTextureRegion(string name, int x, int y, int width, int heigth)
        {
            TextureRegions[name] = new TextureRegion(Texture2D, x, y, width, heigth);
        }

        /// <summary>
        /// Elimina una región específica por nombre.
        /// <para>Removes a specific region by name.</para>
        /// </summary>
        /// <param name="name">Nombre de la región. <para>Region name.</para></param>
        /// <returns>True si se eliminó. <para>True if removed.</para></returns>
        public bool RemoveRegion(string name)
        {
            return TextureRegions.Remove(name);
        }

        /// <summary>
        /// Obtiene una región por su nombre.
        /// <para>Gets a region by name.</para>
        /// </summary>
        /// <param name="name">Nombre de la región. <para>Region name.</para></param>
        /// <returns>Región de textura. <para>The texture region.</para></returns>
        public TextureRegion GetRegion(string name)
        {
            return TextureRegions[name];
        }

        /// <summary>
        /// Añade una animación al diccionario.
        /// <para>Adds an animation to the atlas.</para>
        /// </summary>
        /// <param name="name">Nombre de la animación. <para>Animation name.</para></param>
        /// <param name="animation">Instancia de animación. <para>Animation instance.</para></param>
        public void AddAnimation(string name, Animation animation)
        {
            TextureAnimations.Add(name, animation);
        }

        /// <summary>
        /// Obtiene una animación por nombre.
        /// <para>Gets an animation by name.</para>
        /// </summary>
        /// <param name="name">Nombre de la animación. <para>Animation name.</para></param>
        /// <returns>Animación encontrada. <para>The animation.</para></returns>
        public Animation GetAnimation(string name)
        {
            return TextureAnimations[name];
        }

        /// <summary>
        /// Elimina una animación por nombre.
        /// <para>Removes an animation by name.</para>
        /// </summary>
        /// <param name="name">Nombre de la animación. <para>Animation name.</para></param>
        public void RemoveAnimation(string name)
        {
            TextureAnimations.Remove(name);
        }


        /// <summary>
        /// Limpia todas las regiones de textura y animaciones.
        /// <para>Clears all texture regions and animations.</para>
        /// </summary>
        public void Clear()
        {
            TextureRegions.Clear();
            TextureAnimations.Clear();
        }


        /// <summary>
        /// Crea un atlas de texturas vacío.
        /// <para>Creates an empty texture atlas.</para>
        /// </summary>
        public TextureAtlas() : this(null)
        {
             
        }
    }
}
