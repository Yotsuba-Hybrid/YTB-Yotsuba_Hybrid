using System.Text.Json.Serialization;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Configuration model for the game file.
    /// Modelo del archivo de configuración del juego
    /// </summary>
    public class YTBConfig
    {
        /// <summary>
        /// Game name.
        /// Nombre del juego
        /// </summary>
        [JsonPropertyName("GameName")]
        public string GameName { get; set; }

        /// <summary>
        /// Author or developer name.
        /// Nombre del author o desarrollador del juego
        /// </summary>
        [JsonPropertyName("Author")]
        public string Author { get; set; }

        /// <summary>
        /// Engine version used by the game.
        /// Version del game engine que esta usando el juego
        /// </summary>
        [JsonPropertyName("EngineVersion")]
        public string EngineVersion { get; set; }
    }
}
