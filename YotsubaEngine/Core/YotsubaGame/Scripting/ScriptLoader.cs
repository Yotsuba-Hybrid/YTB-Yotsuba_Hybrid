using System;
using System.IO;
namespace YotsubaEngine.Core.YotsubaGame.Scripting
{
    /// <summary>
    /// Loads script instances from registry identifiers.
    /// Carga instancias de scripts desde identificadores de registro.
    /// </summary>
    public static class ScriptLoader
    {
        /// <summary>
        /// Creates a script instance from the provided script path.
        /// Crea una instancia de script a partir de la ruta proporcionada.
        /// </summary>
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
