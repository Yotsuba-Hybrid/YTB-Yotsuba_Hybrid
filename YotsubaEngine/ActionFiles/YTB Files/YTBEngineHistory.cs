using Microsoft.Xna.Framework;
using System;
using System.Text.Json.Serialization;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Represents historical metadata for YTB engine saves.
    /// Representa metadatos históricos para guardados del motor YTB.
    /// </summary>
    public class YTBEngineHistory
    {
        [JsonPropertyName("Time")]
        /// <summary>
        /// Gets or sets the save timestamp.
        /// Obtiene o establece la marca de tiempo de guardado.
        /// </summary>
        public string SaveTime { get; set; }

        [JsonPropertyName("XMLVersion")]
        /// <summary>
        /// Gets or sets the game development version data.
        /// Obtiene o establece los datos de versión de desarrollo del juego.
        /// </summary>
        internal YTBGameInfo GameDevelopVersion { get; set; }
    }
}
