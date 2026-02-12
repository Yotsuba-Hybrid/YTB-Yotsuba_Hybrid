using System;
using System.IO;
namespace YotsubaEngine.Core.YotsubaGame.Scripting
{
    /// <summary>
    /// Carga instancias de scripts desde identificadores de registro.
    /// <para>Loads script instances from registry identifiers.</para>
    /// </summary>
    public static class ScriptLoader
    {
        /// <summary>
        /// Crea una instancia de script a partir de la ruta proporcionada.
        /// <para>Creates a script instance from the provided script path.</para>
        /// </summary>
        /// <param name="scriptPath">Ruta del script. <para>Script path.</para></param>
        /// <returns>Instancia del script creada. <para>Created script instance.</para></returns>
        public static BaseScript LoadScriptInstance(string scriptPath)
        {
            string scriptName = Path.GetFileName(scriptPath);
            if (string.IsNullOrWhiteSpace(scriptName))
            {
                throw new ArgumentException("Script path is invalid.", nameof(scriptPath));
            }

            if (YTBGame.ScriptRegistry is null)
            {
                throw new InvalidOperationException("Script registry is not configured.");
            }

            try
            {
                BaseScript instance = YTBGame.ScriptRegistry.Create(scriptName);
                return instance;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Script '{scriptName}' could not be created.", ex);
            }
        }
    }
}
