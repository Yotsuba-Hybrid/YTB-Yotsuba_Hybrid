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
                            new Tuple<string, string>("ModelPath", ""),
                            new Tuple<string, string>("IsVisible", "true"),
                            new Tuple<string, string>("SphereRadius", "0"),
                            new Tuple<string, string>("OffsetSphere", "0,0,0"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "InputComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("InputsInUse", ""),
                            new Tuple<string, string>("KeyboardMappings", "MoveUp:W,\nMoveDown:S,\nMoveLeft:A,\nMoveRight:D,"),
                            new Tuple<string, string>("GamePadIndex", ""),
                            new Tuple<string, string>("MouseMappings", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ShaderComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ShaderPath", ""),
                            new Tuple<string, string>("IsActive", "true"),
                            new Tuple<string, string>("params", ""),
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
                            new Tuple<string, string>("Position", "0,0,1"),
                            new Tuple<string, string>("SpriteEffects", "None"),
                            new Tuple<string, string>("Color", "White"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ScriptComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Scripts", "CSHARP&:&&;&"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "SpriteComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("SpriteName", ""),
                            new Tuple<string, string>("SourceRectangle", "0,0,0,0"),
                            new Tuple<string, string>("IsVisible", "true"),
                            new Tuple<string, string>("2.5D", "false"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "TileMapComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TileMapPath", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ButtonComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("IsActive", "true"),
                            new Tuple<string, string>("EffectiveArea", "0,0,0,0"),
                            new Tuple<string, string>("Description", "None"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "RigidBodyComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("OffSetCollision", "0,0"),
                            new Tuple<string, string>("Velocity", "0,0,0"),
                            new Tuple<string, string>("Collide", "Solid"),
                            new Tuple<string, string>("Mass", "0"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "AnimationComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("AnimationBindings", ""),
                            new Tuple<string, string>("CurrentAnimationType", "none"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "FontComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Texto", "Texto de ejemplo"),
                            new Tuple<string, string>("Font", "Fonts/Hud"),
                            new Tuple<string, string>("IsVisible", "true"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "CustomComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Property1", ""),
                            new Tuple<string, string>("Property2", ""),
                            new Tuple<string, string>("Property3", ""),
                        }
                    },
                }
            });
            e.Add(new YTBEntity
            {
                Name = "First Entity",
                Components = new List<YTBComponents>
                {
                    new YTBComponents
                    {
                        ComponentName = "ModelComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ModelPath", ""),
                            new Tuple<string, string>("IsVisible", "true"),
                            new Tuple<string, string>("SphereRadius", "0"),
                            new Tuple<string, string>("OffsetSphere", "0,0,0"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "InputComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("InputsInUse", ""),
                            new Tuple<string, string>("KeyboardMappings", "MoveUp:W,\nMoveDown:S,\nMoveLeft:A,\nMoveRight:D,"),
                            new Tuple<string, string>("GamePadIndex", ""),
                            new Tuple<string, string>("MouseMappings", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ShaderComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ShaderPath", ""),
                            new Tuple<string, string>("IsActive", "true"),
                            new Tuple<string, string>("params", ""),
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
                            new Tuple<string, string>("Position", "0,0,1"),
                            new Tuple<string, string>("SpriteEffects", "None"),
                            new Tuple<string, string>("Color", "White"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ScriptComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Scripts", "CSHARP&:&&;&"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "SpriteComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("SpriteName", ""),
                            new Tuple<string, string>("SourceRectangle", "0,0,0,0"),
                            new Tuple<string, string>("IsVisible", "true"),
                            new Tuple<string, string>("2.5D", "false"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "TileMapComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TileMapPath", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ButtonComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("IsActive", "true"),
                            new Tuple<string, string>("EffectiveArea", "0,0,0,0"),
                            new Tuple<string, string>("Description", "None"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "RigidBodyComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("OffSetCollision", "0,0"),
                            new Tuple<string, string>("Velocity", "0,0,0"),
                            new Tuple<string, string>("Collide", "Solid"),
                            new Tuple<string, string>("Mass", "0"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "AnimationComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("AnimationBindings", ""),
                            new Tuple<string, string>("CurrentAnimationType", "none"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "FontComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Texto", "Texto de ejemplo"),
                            new Tuple<string, string>("Font", "Fonts/Hud"),
                            new Tuple<string, string>("IsVisible", "true"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "CustomComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Property1", ""),
                            new Tuple<string, string>("Property2", ""),
                            new Tuple<string, string>("Property3", ""),
                        }
                    },
                }
            });
        }
    }
}
