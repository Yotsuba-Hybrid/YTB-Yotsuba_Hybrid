using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace YotsubaEngine.ActionFiles.YTB_Files
{

    /// <summary>
    /// Source-generated JSON context for YTB file serialization.
    /// Contexto JSON generado para serialización de archivos YTB.
    /// </summary>
    [JsonSerializable(typeof(YTBConfig))]
    [JsonSerializable(typeof(YTBGameInfo))]
    [JsonSerializable(typeof(YTBEngineHistory))]
    [JsonSerializable(typeof(List<YTBEngineHistory>))]
    internal partial class YotsubaJsonContext : JsonSerializerContext
    {
    }
}
