using System;
using System.Collections.Generic;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Component that stores script instances and language bindings.
    /// Componente que almacena instancias de scripts y enlaces de lenguaje.
    /// </summary>
    public struct ScriptComponent()
    {

        /// <summary>
        /// All scripts associated with this entity.
        /// Todos los scripts asociados a esta entidad.
        /// </summary>
        public YTB<BaseScript> Scripts = new YTB<BaseScript>();

        /// <summary>
        /// Script language types used by the entity.
        /// Tipo del lenguaje de script utilizado.
        /// </summary>
        public Dictionary<ScriptComponentType, string> ScriptLanguaje = new Dictionary<ScriptComponentType, string>(3);
    }


    /// <summary>
    /// Supported scripting language types.
    /// Tipos de lenguajes de scripting soportados.
    /// </summary>
    public enum ScriptComponentType
    {
        //PYTHON,
        /// <summary>
        /// C# scripts.
        /// Scripts en C#.
        /// </summary>
        CSHARP
    }
}
