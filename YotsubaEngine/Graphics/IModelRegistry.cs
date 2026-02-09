using System.Collections.Generic;

namespace YotsubaEngine.Graphics
{
    /// <summary>
    /// Base registry for 3D model assets used by the engine.
    /// Registro base para activos de modelos 3D usados por el motor.
    /// </summary>
    /// <remarks>
    /// Games must inherit from this class and register their models in the constructor.
    /// The generator creates a ModelRegistry class that inherits from this.
    /// Los juegos deben heredar de esta clase y registrar sus modelos en el constructor.
    /// El generador crea una clase ModelRegistry que hereda de esta.
    /// </remarks>
    public abstract class IModelRegistry
    {
        /// <summary>
        /// List to hold all registered model paths.
        /// Initialized with empty string as first element for "none selected" option in UI dropdowns.
        /// Lista para guardar todas las rutas de modelos registrados.
        /// Inicializada con string vacío como primer elemento para opción "ninguno seleccionado" en dropdowns de UI.
        /// </summary>
        private static List<string> _allModelsList = new List<string> { "" };

        /// <summary>
        /// Array of all available 3D model paths for UI dropdown/combo.
        /// Array de todas las rutas de modelos 3D disponibles para dropdown/combo en la UI.
        /// </summary>
        public static string[] AllModels => _allModelsList.ToArray();

        /// <summary>
        /// Registers a model path to the available models list.
        /// Registra una ruta de modelo a la lista de modelos disponibles.
        /// </summary>
        protected static void RegisterModel(string path)
        {
            if (!_allModelsList.Contains(path))
            {
                _allModelsList.Add(path);
            }
        }

        /// <summary>
        /// Clears all registered models (useful for hot-reload).
        /// Limpia todos los modelos registrados (útil para recarga en caliente).
        /// </summary>
        public static void Clear()
        {
            _allModelsList.Clear();
            _allModelsList.Add(""); // Re-add empty option
        }
    }
}
