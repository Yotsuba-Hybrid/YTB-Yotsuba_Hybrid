using Microsoft.Xna.Framework;
using System;
using System.Text.Json.Serialization;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Representa metadatos históricos para guardados del motor YTB.
    /// <para>Represents historical metadata for YTB engine saves.</para>
    /// </summary>
    public class YTBEngineHistory
    {
        [JsonPropertyName("Time")]
        /// <summary>
        /// Obtiene o establece la marca de tiempo de guardado.
        /// <para>Gets or sets the save timestamp.</para>
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
