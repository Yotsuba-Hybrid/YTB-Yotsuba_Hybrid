using System;
using System.Collections.Generic;
using System.Linq;
using YotsubaEngine.Core.YotsubaGame.Scripting;

namespace YotsubaEngine.Scripting
{
    /// <summary>
    /// Base registry for script factories used by the engine.
    /// Registro base para fábricas de scripts usadas por el motor.
    /// </summary>
    public abstract class IScriptRegistry
    {

        /// <summary>
        /// Global script factory registry keyed by script name.
        /// Registro global de fábricas de scripts indexado por nombre.
        /// </summary>
        public static readonly Dictionary<string, Func<BaseScript>> Scripts = new Dictionary<string, Func<BaseScript>>();

        /// <summary>
        /// Creates a script instance by name.
        /// Crea una instancia de script por nombre.
        /// </summary>
        public virtual BaseScript Create(string scriptName)
        {
            return default;
        }

        /// <summary>
        /// Returns the registered script factories.
        /// Devuelve las fábricas de scripts registradas.
        /// </summary>
        public Dictionary<string, Func<BaseScript>> GetScripts()
        {
            return Scripts;
        }
    }
}
