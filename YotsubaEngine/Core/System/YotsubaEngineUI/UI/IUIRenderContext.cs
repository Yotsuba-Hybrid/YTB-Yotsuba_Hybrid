#if YTB
using System.Collections.Generic;
using YotsubaEngine.ActionFiles.YTB_Files;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
    /// <summary>
    /// Contexto que el EntityManagerUI pasa a los métodos estáticos de render UI de los componentes
    /// (los apuntados por <see cref="YotsubaEngine.Attributes.UIComponentValue.NameMethodValueConverterForRead"/>).
    /// Permite que un componente dibuje UI compleja (combos pareados, dropdowns con datos externos) sin acoplarse al EntityManagerUI.
    /// </summary>
    public interface IUIRenderContext
    {
        /// <summary>
        /// Componente actualmente siendo renderizado (su Propiedades es la fuente y destino de los valores).
        /// </summary>
        YTBComponents Component { get; }

        /// <summary>
        /// Actualiza el valor de una propiedad serializada del componente actual.
        /// </summary>
        void UpdateProperty(string propertyName, string newValue);

        /// <summary>
        /// Lista de archivos XML de atlas de texturas disponibles bajo Content/, en rutas relativas.
        /// </summary>
        IReadOnlyList<string> TextureAtlasFiles { get; }

        /// <summary>
        /// Parsea (con caché) las subtexturas de un atlas XML dado.
        /// </summary>
        List<SubtextureInfo> ParseSubtextures(string xmlPath);

        /// <summary>
        /// Parsea (con caché) las animaciones de un atlas XML dado.
        /// </summary>
        List<AnimationInfo> ParseAnimations(string xmlPath);

        /// <summary>
        /// Nombres de las entidades en la escena actual (para dropdowns tipo "EntityName").
        /// </summary>
        IEnumerable<string> SceneEntityNames { get; }

        /// <summary>
        /// Nombres de todos los scripts registrados.
        /// </summary>
        IEnumerable<string> AllScripts { get; }
    }
}
#endif
