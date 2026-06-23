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
                GameName = "YotsubaGame",
                Author = "YourName",
                EngineVersion = "1.0"
            };

            return (gameInfo, config);
        }

        private static YTBScene BuildScene_0()
        {
            var entities = new List<YTBEntity>(2);
            BuildScene_0_Batch_0(entities);
            return new YTBScene
            {
                Name = "First Scene",
                Entities = entities
            };
        }

        private static void BuildScene_0_Batch_0(List<YTBEntity> e)
        {
            e.Add(new YTBEntity
            {
                Name = "Camera",
                Components = new List<YTBComponents>
                {
                    new YTBComponents
                    {
                        ComponentName = "ModelComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("EntityName", "3D"),
                            new Tuple<string, string>("InitialPosition", "0,60,30"),
                            new Tuple<string, string>("OffsetCamera", "0,50,-900"),
                            new Tuple<string, string>("AngleView", "60"),
                            new Tuple<string, string>("NearRender", "10"),
                            new Tuple<string, string>("FarRender", "3000"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "TransformComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Size", "100,100,0"),
                            new Tuple<string, string>("Scale", "1"),
                            new Tuple<string, string>("Rotation", "0"),
                            new Tuple<string, string>("Position", "-500,-500,-500"),
                            new Tuple<string, string>("SpriteEffects", "None"),
                            new Tuple<string, string>("Color", "White"),
                        }
                    },
                }
            });
            e.Add(new YTBEntity
            {
                Name = "3D",
                Components = new List<YTBComponents>
                {
                    new YTBComponents
                    {
                        ComponentName = "TransformComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Size", "850,1500,0"),
                            new Tuple<string, string>("Scale", "1"),
                            new Tuple<string, string>("Rotation", "0"),
                            new Tuple<string, string>("Position", "0,0,1"),
                            new Tuple<string, string>("SpriteEffects", "None"),
                            new Tuple<string, string>("Color", "White"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "SpriteComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", "Spritesheets/SpriteSheet.xml"),
                            new Tuple<string, string>("SpriteName", "Banner Oshi No Ko"),
                            new Tuple<string, string>("SourceRectangle", "0,0,850,1500"),
                            new Tuple<string, string>("IsVisible", "true"),
                            new Tuple<string, string>("2.5D", "true"),
                        }
                    },
                }
            });
        }
    }
}
