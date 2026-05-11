using System;
using System.Collections.Generic;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Componente que almacena instancias de scripts y enlaces de lenguaje.
    /// <para>Component that stores script instances and language bindings.</para>
    /// </summary>
    [UIComponent("Script", nameof(ScriptComponent))]
    public partial struct ScriptComponent()
    {

        /// <summary>
        /// Todos los scripts asociados a esta entidad (runtime).
        /// </summary>
        public YTB<BaseScript> Scripts = new YTB<BaseScript>();

        /// <summary>
        /// Bridge serializable: scripts en formato "CSHARP&amp;:&amp;Route1&amp;;&amp;CSHARP&amp;:&amp;Route2".
        /// </summary>
        [UIComponentValue("Scripts", "Scripts",
            "Scripts asociados a la entidad. Formato compuesto con separadores '&;&' y '&:&'.",
            "Formato inválido.",
            ValueConverterForRead:"RenderScriptsUI")]
        public string ScriptsRaw { get; set; }

        /// <summary>
        /// Tipos de lenguaje de script usados por la entidad (runtime).
        /// </summary>
        public Dictionary<ScriptComponentType, string> ScriptLanguaje = new Dictionary<ScriptComponentType, string>(3);
    }


    /// <summary>
    /// Tipos de lenguajes de scripting soportados.
    /// <para>Supported scripting language types.</para>
    /// </summary>
    public enum ScriptComponentType
    {
        //PYTHON,
        /// <summary>
        /// Scripts en C#.
        /// <para>C# scripts.</para>
        /// </summary>
        CSHARP
    }
}
