using System;
using System.Collections.Generic;
using System.Linq;
using YotsubaEngine.Core.YotsubaGame.Scripting;

namespace YotsubaEngine.Scripting
{
    /// <summary>
    /// Registro base para fábricas de scripts usadas por el motor.
    /// <para>Base registry for script factories used by the engine.</para>
    /// </summary>
    public abstract class IScriptRegistry
    {

        /// <summary>
        /// Registro global de fábricas de scripts indexado por nombre.
        /// <para>Global script factory registry keyed by script name.</para>
        /// </summary>
        public static readonly Dictionary<string, Func<BaseScript>> Scripts = new Dictionary<string, Func<BaseScript>>();

        /// <summary>
        /// Crea una instancia de script por nombre.
        /// <para>Creates a script instance by name.</para>
        /// </summary>
        /// <param name="scriptName">Nombre del script. <para>Script name.</para></param>
        /// <returns>Instancia creada del script. <para>Created script instance.</para></returns>
        public virtual BaseScript Create(string scriptName)
        {
            return default;
        }

        /// <summary>
        /// Devuelve las fábricas de scripts registradas.
        /// <para>Returns the registered script factories.</para>
        /// </summary>
        /// <returns>Fábricas de scripts registradas. <para>Registered script factories.</para></returns>
        public Dictionary<string, Func<BaseScript>> GetScripts()
        {
            return Scripts;
        }
    }
}
