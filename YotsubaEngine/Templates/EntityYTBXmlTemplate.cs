using System;
using System.Collections.Generic;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.Component.C_AGNOSTIC;

namespace YotsubaEngine.Templates
{
    /// <summary>
    /// Proporciona constructores de plantillas para datos XML de entidades y componentes YTB.
    /// <para>Provides template builders for YTB entity and component XML data.</para>
    /// </summary>
    public static class EntityYTBXmlTemplate
    {
        /// <summary>
        /// Builds a default entity template with common components.
        /// Construye una plantilla de entidad por defecto con componentes comunes.
        /// </summary>
        internal static YTBEntity GenerateNew()
        {

            var newEntity = new YTBEntity
            {
                Name = "",
                Components = new List<YTBComponents>
                {
                    new YTBComponents
                    {
                        ComponentName = "TransformComponent",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("Position", ",,"),
                            new("Size", ",,"),
                            new("Color", ""),
                            new("SpriteEffects", ""),
                            new("Scale", ""),
                            //new("SpriteSize", "false")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "SpriteComponent2D",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("TextureAtlasPath", ""),
                            new("SpriteName", ""),
                            new("SourceRectangle", ",,,"),
                            new("IsVisible", ""),
                            new("2.5D", "")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "AnimationComponent2D",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("TextureAtlasPath", ""),
                            new("AnimationBindings", ""),
                            new("CurrentAnimationType", "")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "RigidBodyComponent2D",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("OffSetCollision", ","),
                            new("Velocity", ","),
                            new("GameType", "TopDown"),
                            new("Mass", "")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ButtonComponent2D",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("IsActive", ""),
                            new("EffectiveArea", ",,,"),
                            new("Description", "")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "InputComponent",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("InputsInUse", ""),
                            new("GamePadIndex", ""),
                            new("KeyboardMappings", ""),
                            new("MouseMappings", ""),
                            //new("WASDToolkit", "false")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = nameof(CameraComponent3D),
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("EntityName", ""),
                            new("InitialPosition", "0,0,0"),
                            new("OffsetCamera", ",,,"),
                            new("AngleView", "0"),
                            new("NearRender", "0"),
                            new("FarRender", "0")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ScriptComponent",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("Scripts", "CSHARP&:&RouteToScript&;&"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "TileMapComponent2D",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("TileMapPath", "nothing"),
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "FontComponent2D",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("Texto", ""),
                            new("Font", "")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ShaderComponent",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("ShaderPath", ""),
                            new("IsActive", ""),
                            new("params", "")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "ModelComponent3D",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("ModelPath", ""),
                            new("IsVisible", "")
                        }
                    },
                    new YTBComponents
                    {
                        ComponentName = "CustomComponent",
                        Propiedades = new List<Tuple<string,string>>
                        {
                            new("Property1", ""),
                            new("Property2", ""),
                            new("Property3", "")
                        }
                    }
                }
            };

            return newEntity;
        }

        /// <summary>
        /// Crea una plantilla para componentes de script.
        /// <para>Creates a template for script components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de script. <para>Script component template.</para></returns>
        public static YTBComponents ScriptTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "ScriptComponent",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("Scripts", "CSHARP&:&&;&"),
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de transformación.
        /// <para>Creates a template for transform components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de transformación. <para>Transform component template.</para></returns>
        public static YTBComponents TransformTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "TransformComponent",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("Position", "0,0,1"),
                            new("Size", "100,100,0"),
                            new("Color", "White"),
                            new("SpriteEffects", "None"),
                            new("Scale", "1")
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de sprite 2D.
        /// <para>Creates a template for 2D sprite components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de sprite 2D. <para>2D sprite component template.</para></returns>
        public static YTBComponents Sprite2DTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "SpriteComponent2D",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("TextureAtlasPath", ""),
                            new("SpriteName", ""),
                            new("SourceRectangle", "0,0,0,0"),
                            new("IsVisible", "true"),
                            new("2.5D", "false")
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de animación 2D.
        /// <para>Creates a template for 2D animation components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de animación 2D. <para>2D animation component template.</para></returns>
        public static YTBComponents Animation2DTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "AnimationComponent2D",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("TextureAtlasPath", ""),
                            new("AnimationBindings", ""),
                            new("CurrentAnimationType", "none")
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de cuerpo rígido 2D.
        /// <para>Creates a template for 2D rigid body components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de cuerpo rígido 2D. <para>2D rigid body component template.</para></returns>
        public static YTBComponents Rigibody2DTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "RigidBodyComponent2D",
                Propiedades = new List<Tuple<string, string>>
                {
                    new("OffSetCollision", "0,0"),
                    new("Velocity", "0,0"),
                    new("GameType", "TopDown"),
                    new("Mass", "0")
                }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de botón 2D.
        /// <para>Creates a template for 2D button components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de botón 2D. <para>2D button component template.</para></returns>
        public static YTBComponents Button2DTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "ButtonComponent2D",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("IsActive", "true"),
                            new("EffectiveArea", "0,0,0,0"),
                            new("Description", "None")
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de entrada.
        /// <para>Creates a template for input components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de entrada. <para>Input component template.</para></returns>
        public static YTBComponents InputTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "InputComponent",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("InputsInUse", ""),
                            new("GamePadIndex", ""),
                            new("KeyboardMappings", "MoveUp:W,\nMoveDown:S,\nMoveLeft:A,\nMoveRight:D,"),
                            new("MouseMappings", ""),

                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de cámara.
        /// <para>Creates a template for camera components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de cámara. <para>Camera component template.</para></returns>
        public static YTBComponents CameraTemplate()
        {
           return new YTBComponents
            {
                ComponentName = "CameraComponent3D",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("EntityName", ""),
                            new("InitialPosition", "0,60,30"),
                            new("OffsetCamera", "0,50,-100"),
                            new("AngleView", "60"),
                            new("NearRender", "10"),
                            new("FarRender", "3000")
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de tile map 2D.
        /// <para>Creates a template for 2D tile map components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de tile map 2D. <para>2D tile map component template.</para></returns>
        public static YTBComponents TileMap2DTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "TileMapComponent2D",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("TileMapPath", ""),
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de fuente 2D.
        /// <para>Creates a template for 2D font components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de fuente 2D. <para>2D font component template.</para></returns>
        public static YTBComponents Font2DTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "FontComponent2D",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("Texto", "Texto de ejemplo"),
                            new("Font", "Fonts/Hud")
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de shader 2D.
        /// <para>Creates a template for 2D shader components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de shader 2D. <para>2D shader component template.</para></returns>
        public static YTBComponents ShaderTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "ShaderComponent",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("ShaderPath", ""),
                            new("IsActive", ""),
                            new("params", "")
                        }
            };
        }

        /// <summary>
        /// Crea una plantilla para componentes de modelo 3D.
        /// <para>Creates a template for 3D model components.</para>
        /// </summary>
        /// <returns>Plantilla de componente de modelo 3D. <para>3D model component template.</para></returns>
        public static YTBComponents Model3DTemplate()
        {
            return new YTBComponents
            {
                ComponentName = "ModelComponent3D",
                Propiedades = new List<Tuple<string, string>>
                        {
                            new("ModelPath", ""),
                            new("IsVisible", "true")
                        }
            };
        }
    }
}
