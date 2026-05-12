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
                Scene = new List<YTBScene>(2)
                {
                    BuildScene_0(),
                    BuildScene_1(),
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
            var entities = new List<YTBEntity>(1);
            BuildScene_0_Batch_0(entities);
            return new YTBScene
            {
                Name = "Index",
                Entities = entities
            };
        }

        private static void BuildScene_0_Batch_0(List<YTBEntity> e)
        {
            e.Add(new YTBEntity
            {
                Name = "MainCamera",
                Components = new List<YTBComponents>
                {
                    new YTBComponents
                    {
                        ComponentName = "CameraComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("EntityName", "MainCamera"),
                            new Tuple<string, string>("InitialPosition", "0,60,30"),
                            new Tuple<string, string>("OffsetCamera", "0,50,-100"),
                            new Tuple<string, string>("AngleView", "60"),
                            new Tuple<string, string>("NearRender", "10"),
                            new Tuple<string, string>("FarRender", "3000"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "InputComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("InputsInUse", ""),
                            new Tuple<string, string>("KeyboardMappings", ""),
                            new Tuple<string, string>("GamePadIndex", ""),
                            new Tuple<string, string>("MouseMappings", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ScriptComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Scripts", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ShaderComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ShaderPath", ""),
                            new Tuple<string, string>("IsActive", ""),
                            new Tuple<string, string>("params", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "TransformComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Position", "0,0,0"),
                            new Tuple<string, string>("Size", "100,100,0"),
                            new Tuple<string, string>("Color", "White"),
                            new Tuple<string, string>("SpriteEffects", "None"),
                            new Tuple<string, string>("Scale", "1"),
                            new Tuple<string, string>("Rotation", "0"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ModelComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ModelPath", ""),
                            new Tuple<string, string>("IsVisible", ""),
                            new Tuple<string, string>("SphereRadius", ""),
                            new Tuple<string, string>("OffsetSphere", ",,"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "AnimationComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("AnimationBindings", ""),
                            new Tuple<string, string>("CurrentAnimationType", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ButtonComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("IsActive", ""),
                            new Tuple<string, string>("EffectiveArea", ",,,"),
                            new Tuple<string, string>("Description", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "FontComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Texto", ""),
                            new Tuple<string, string>("Font", ""),
                            new Tuple<string, string>("IsVisible", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "RigidBodyComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("OffSetCollision", ","),
                            new Tuple<string, string>("Velocity", ",,"),
                            new Tuple<string, string>("Collide", ""),
                            new Tuple<string, string>("Mass", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "SpriteComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("SpriteName", ""),
                            new Tuple<string, string>("SourceRectangle", ",,,"),
                            new Tuple<string, string>("IsVisible", ""),
                            new Tuple<string, string>("2.5D", ""),
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

        private static YTBScene BuildScene_1()
        {
            var entities = new List<YTBEntity>(3);
            BuildScene_1_Batch_0(entities);
            return new YTBScene
            {
                Name = "Main",
                Entities = entities
            };
        }

        private static void BuildScene_1_Batch_0(List<YTBEntity> e)
        {
            e.Add(new YTBEntity
            {
                Name = "MainCamera",
                Components = new List<YTBComponents>
                {
                    new YTBComponents
                    {
                        ComponentName = "CameraComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("EntityName", "MainCamera"),
                            new Tuple<string, string>("InitialPosition", "0,60,30"),
                            new Tuple<string, string>("OffsetCamera", "0,50,-100"),
                            new Tuple<string, string>("AngleView", "60"),
                            new Tuple<string, string>("NearRender", "10"),
                            new Tuple<string, string>("FarRender", "3000"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "InputComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("InputsInUse", ""),
                            new Tuple<string, string>("KeyboardMappings", ""),
                            new Tuple<string, string>("GamePadIndex", ""),
                            new Tuple<string, string>("MouseMappings", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ScriptComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Scripts", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ShaderComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ShaderPath", ""),
                            new Tuple<string, string>("IsActive", ""),
                            new Tuple<string, string>("params", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "TransformComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Position", "0,0,0"),
                            new Tuple<string, string>("Size", "100,100,0"),
                            new Tuple<string, string>("Color", "White"),
                            new Tuple<string, string>("SpriteEffects", "None"),
                            new Tuple<string, string>("Scale", "1"),
                            new Tuple<string, string>("Rotation", "0"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ModelComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ModelPath", ""),
                            new Tuple<string, string>("IsVisible", ""),
                            new Tuple<string, string>("SphereRadius", ""),
                            new Tuple<string, string>("OffsetSphere", ",,"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "AnimationComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("AnimationBindings", ""),
                            new Tuple<string, string>("CurrentAnimationType", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ButtonComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("IsActive", ""),
                            new Tuple<string, string>("EffectiveArea", ",,,"),
                            new Tuple<string, string>("Description", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "FontComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Texto", ""),
                            new Tuple<string, string>("Font", ""),
                            new Tuple<string, string>("IsVisible", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "RigidBodyComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("OffSetCollision", ","),
                            new Tuple<string, string>("Velocity", ",,"),
                            new Tuple<string, string>("Collide", ""),
                            new Tuple<string, string>("Mass", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "SpriteComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("SpriteName", ""),
                            new Tuple<string, string>("SourceRectangle", ",,,"),
                            new Tuple<string, string>("IsVisible", ""),
                            new Tuple<string, string>("2.5D", ""),
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
                Name = "Entidad_2",
                Components = new List<YTBComponents>
                {
                    new YTBComponents
                    {
                        ComponentName = "CameraComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("EntityName", ""),
                            new Tuple<string, string>("InitialPosition", ",,"),
                            new Tuple<string, string>("OffsetCamera", ",,"),
                            new Tuple<string, string>("AngleView", ""),
                            new Tuple<string, string>("NearRender", ""),
                            new Tuple<string, string>("FarRender", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "InputComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("InputsInUse", ""),
                            new Tuple<string, string>("KeyboardMappings", ""),
                            new Tuple<string, string>("GamePadIndex", ""),
                            new Tuple<string, string>("MouseMappings", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ScriptComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Scripts", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ShaderComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ShaderPath", ""),
                            new Tuple<string, string>("IsActive", ""),
                            new Tuple<string, string>("params", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "TransformComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Size", ",,"),
                            new Tuple<string, string>("Scale", ""),
                            new Tuple<string, string>("Rotation", ""),
                            new Tuple<string, string>("Position", ",,"),
                            new Tuple<string, string>("SpriteEffects", ""),
                            new Tuple<string, string>("Color", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ModelComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ModelPath", ""),
                            new Tuple<string, string>("IsVisible", ""),
                            new Tuple<string, string>("SphereRadius", ""),
                            new Tuple<string, string>("OffsetSphere", ",,"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "AnimationComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("AnimationBindings", ""),
                            new Tuple<string, string>("CurrentAnimationType", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ButtonComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("IsActive", ""),
                            new Tuple<string, string>("EffectiveArea", ",,,"),
                            new Tuple<string, string>("Description", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "FontComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Texto", ""),
                            new Tuple<string, string>("Font", ""),
                            new Tuple<string, string>("IsVisible", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "RigidBodyComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("OffSetCollision", ","),
                            new Tuple<string, string>("Velocity", ",,"),
                            new Tuple<string, string>("Collide", ""),
                            new Tuple<string, string>("Mass", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "SpriteComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("SpriteName", ""),
                            new Tuple<string, string>("SourceRectangle", ",,,"),
                            new Tuple<string, string>("IsVisible", ""),
                            new Tuple<string, string>("2.5D", ""),
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
                Name = "Entidad_3",
                Components = new List<YTBComponents>
                {
                    new YTBComponents
                    {
                        ComponentName = "CameraComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("EntityName", ""),
                            new Tuple<string, string>("InitialPosition", ",,"),
                            new Tuple<string, string>("OffsetCamera", ",,"),
                            new Tuple<string, string>("AngleView", ""),
                            new Tuple<string, string>("NearRender", ""),
                            new Tuple<string, string>("FarRender", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "InputComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("InputsInUse", ""),
                            new Tuple<string, string>("KeyboardMappings", ""),
                            new Tuple<string, string>("GamePadIndex", ""),
                            new Tuple<string, string>("MouseMappings", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ScriptComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Scripts", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ShaderComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ShaderPath", ""),
                            new Tuple<string, string>("IsActive", ""),
                            new Tuple<string, string>("params", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "TransformComponent",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Size", ",,"),
                            new Tuple<string, string>("Scale", ""),
                            new Tuple<string, string>("Rotation", ""),
                            new Tuple<string, string>("Position", ",,"),
                            new Tuple<string, string>("SpriteEffects", ""),
                            new Tuple<string, string>("Color", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ModelComponent3D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("ModelPath", ""),
                            new Tuple<string, string>("IsVisible", ""),
                            new Tuple<string, string>("SphereRadius", ""),
                            new Tuple<string, string>("OffsetSphere", ",,"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "AnimationComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("AnimationBindings", ""),
                            new Tuple<string, string>("CurrentAnimationType", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ButtonComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("IsActive", ""),
                            new Tuple<string, string>("EffectiveArea", ",,,"),
                            new Tuple<string, string>("Description", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "FontComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("Texto", ""),
                            new Tuple<string, string>("Font", ""),
                            new Tuple<string, string>("IsVisible", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "RigidBodyComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("OffSetCollision", ","),
                            new Tuple<string, string>("Velocity", ",,"),
                            new Tuple<string, string>("Collide", ""),
                            new Tuple<string, string>("Mass", ""),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "SpriteComponent2D",
                        Propiedades = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("TextureAtlasPath", ""),
                            new Tuple<string, string>("SpriteName", ""),
                            new Tuple<string, string>("SourceRectangle", ",,,"),
                            new Tuple<string, string>("IsVisible", ""),
                            new Tuple<string, string>("2.5D", ""),
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
