using System;
using System.Collections.Generic;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    /// <summary>
    /// Componente que almacena instancias de scripts y enlaces de lenguaje.
    /// <para>Component that stores script instances and language bindings.</para>
    /// </summary>
    public struct ScriptComponent()
    {

        /// <summary>
        /// Todos los scripts asociados a esta entidad.
        /// <para>All scripts associated with this entity.</para>
        /// </summary>
        public YTB<BaseScript> Scripts = new YTB<BaseScript>();

        /// <summary>
        /// Tipos de lenguaje de script usados por la entidad.
        /// <para>Script language types used by the entity.</para>
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
