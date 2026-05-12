using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Runtime.RPR.Events;

namespace YotsubaEngine.Core.Component.C_2D
{

    /// <summary>
    /// Componente que añade funcionalidad de mostrar un sprite en la pantalla.
    /// <para>Component that renders a sprite to the screen.</para>
    /// </summary>
    /// <param name="texture">Textura a renderizar.<para>Texture2D to render.</para></param>
    /// <param name="sourceRectangle">Región de la textura a renderizar.<para>Texture region to render.</para></param>
    [UIComponent("Sprite 2D", nameof(SpriteComponent2D))]
    public partial struct SpriteComponent2D(Texture2D texture, Rectangle sourceRectangle)
    {
        private bool is2_5D = false;

        /// <summary>
        /// Textura usada para renderizar la entidad (runtime, no serializada directamente).
        /// </summary>
        public Texture2D Texture { get; set; } = texture;

        /// <summary>
        /// Ruta al XML del atlas de texturas. Bridge de serialización: el parseo carga Texture desde aquí.
        /// </summary>
        [UIComponentValue("Atlas de texturas", "TextureAtlasPath", "Ruta relativa al XML del atlas dentro de Content/.",
            "El atlas no existe o no es válido.",
            ValueConverterForRead: "RenderTextureAtlasUI",
            defaultValue: "", inactiveValue: "")]
        public string TextureAtlasPath { get; set; }

        /// <summary>
        /// Nombre del sprite dentro del atlas. Bridge de serialización: define qué subtextura usar.
        /// </summary>
        [UIComponentValue("Sprite", "SpriteName", "Nombre del sprite dentro del atlas.",
            "El sprite no existe dentro del atlas seleccionado.",
            ValueConverterForRead: "RenderSpriteNameUI",
            defaultValue: "", inactiveValue: "")]
        public string SpriteName { get; set; }

        /// <summary>
        /// Restringe el área de la textura a dibujar.
        /// </summary>
        [UIComponentValue("Rectángulo de origen", nameof(SourceRectangle), "Área dentro del atlas (X,Y,Width,Height).",
            "Formato: 4 enteros separados por comas.",
            defaultValue: "0,0,0,0", inactiveValue: ",,,")]
        public Rectangle SourceRectangle { get; set; } = sourceRectangle;

        /// <summary>
        /// Indica si el sprite debe renderizarse.
        /// </summary>
        [UIComponentValue("Visible", nameof(IsVisible), "Si el sprite se dibuja.", "Valor true/false.",
            defaultValue: "true", inactiveValue: "")]
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Indica la dimensión en la que se renderizará el sprite.
        /// </summary>
        [UIComponentValue("2.5D", "2.5D", "Si el sprite se renderiza como billboard 2.5D.", "Valor true/false.",
            defaultValue: "false", inactiveValue: "")]
        public bool Is2_5D
        {
            get => is2_5D;
            set
            {
                is2_5D = value;
                if (is2_5D)
                {
                    EventManager.Instance.Publish<OnSpriteIsSettedAs2_5D>(new());
                }
            }
        }

        // Métodos estáticos de UI (RenderTextureAtlasUI, RenderSpriteNameUI) viven en SpriteComponent2D.UI.cs (#if YTB).
    }

}
