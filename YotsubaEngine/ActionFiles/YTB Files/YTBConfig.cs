using System.Text.Json.Serialization;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Modelo del archivo de configuración del juego.
    /// <para>Configuration model for the game file.</para>
    /// </summary>
    public class YTBConfig
    {
        /// <summary>
        /// Nombre del juego.
        /// <para>Game name.</para>
        /// </summary>
        [JsonPropertyName("GameName")]
        public string GameName { get; set; }

        /// <summary>
        /// Nombre del autor o desarrollador del juego.
        /// <para>Author or developer name.</para>
        /// </summary>
        [JsonPropertyName("Author")]
        public string Author { get; set; }

        /// <summary>
        /// Versión del motor que está usando el juego.
        /// <para>Engine version used by the game.</para>
        /// </summary>
        [JsonPropertyName("EngineVersion")]
        public string EngineVersion { get; set; }
    }
}
