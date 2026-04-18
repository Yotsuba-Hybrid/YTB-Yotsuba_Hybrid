using System;
using System.Collections.Generic;
using YotsubaEngine.ActionFiles.YTB_Files;

namespace YotsubaEngine.ActionFiles.YTB_Files
{
    /// <summary>
    /// Auto-generated class containing pre-built game data.
    /// Eliminates runtime JSON parsing for faster load times.
    /// </summary>
    internal static class GameDataRegistry
    {
        internal static (YTBGameInfo, YTBConfig) GetGameData()
        {
            var gameInfo = new YTBGameInfo
            {
                Scene = new List<YTBScene>(1)
                {
                    BuildScene_0(),
                }
            };

            var config = new YTBConfig
            {
                GameName = "Yotsuba Engine",
                Author = "MyName",
                EngineVersion = "1.0"
            };

            return (gameInfo, config);
        }

        private static YTBScene BuildScene_0()
        {
            var entities = new List<YTBEntity>(0);
            return new YTBScene
            {
                Name = "New Scene",
                Entities = entities
            };
        }
    }
}
