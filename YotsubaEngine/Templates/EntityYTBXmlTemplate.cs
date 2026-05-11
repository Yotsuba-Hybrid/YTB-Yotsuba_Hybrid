using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
#if YTB
using YotsubaEngine.Core.System;
#endif

namespace YotsubaEngine.Templates
{
    /// <summary>
    /// Proporciona constructores de plantillas para datos de entidades y componentes YTB.
    /// La mayor parte se auto-genera a partir de los atributos [UIComponent]/[UIComponentValue] usando <see cref="TemplateFromAttributes"/>.
    /// </summary>
    public static class EntityYTBXmlTemplate
    {
        /// <summary>
        /// Construye una plantilla de entidad por defecto con todos los componentes [UIComponent] registrados.
        /// En #if YTB usa <see cref="UIComponentRegistry"/> (reflexión). Fuera de #if YTB usa las plantillas hand-written.
        /// </summary>
        internal static YTBEntity GenerateNew()
        {
            var components = new List<YTBComponents>();

#if YTB
            foreach (var kv in UIComponentRegistry.AllComponents)
            {
                components.Add(TemplateFromAttributes(kv.Key));
            }
#else
            components.Add(TransformTemplate());
            components.Add(Sprite2DTemplate());
            components.Add(Animation2DTemplate());
            components.Add(Rigibody2DTemplate());
            components.Add(Button2DTemplate());
            components.Add(InputTemplate());
            components.Add(CameraTemplate());
            components.Add(ScriptTemplate());
            components.Add(TileMap2DTemplate());
            components.Add(Font2DTemplate());
            components.Add(ShaderTemplate());
            components.Add(Model3DTemplate());
#endif
            // CustomComponent es un placeholder genérico fuera del sistema de atributos.
            components.Add(new YTBComponents
            {
                ComponentName = "CustomComponent",
                Propiedades = new List<Tuple<string, string>>
                {
                    new("Property1", ""),
                    new("Property2", ""),
                    new("Property3", "")
                }
            });

            return new YTBEntity { Name = "", Components = components };
        }

#if YTB
        /// <summary>
        /// Construye un YTBComponents desde los atributos [UIComponent]/[UIComponentValue] de un tipo.
        /// Cada propiedad serializable recibe un valor por defecto razonable según su tipo.
        /// </summary>
        public static YTBComponents TemplateFromAttributes(Type componentType)
        {
            var compAttr = componentType.GetCustomAttribute<UIComponent>()
                ?? throw new ArgumentException($"El tipo {componentType.Name} no tiene [UIComponent].");

            var props = new List<Tuple<string, string>>();
            foreach (var member in UIComponentRegistry.GetMembers(componentType))
            {
                props.Add(new Tuple<string, string>(
                    member.Attribute.SerializableName,
                    GetDefaultValueForType(member.MemberType)));
            }

            return new YTBComponents
            {
                ComponentName = compAttr.SerializableName,
                Propiedades = props
            };
        }

        /// <summary>
        /// Valor por defecto de string para inicializar la plantilla según el tipo C#.
        /// </summary>
        private static string GetDefaultValueForType(Type t)
        {
            if (t == typeof(Vector3)) return ",,";
            if (t == typeof(Vector2)) return ",";
            if (t == typeof(Rectangle)) return ",,,";
            // bool, float, int, string, enum → cadena vacía (el usuario completa en la UI)
            return "";
        }
#endif

        // === Plantillas hand-written usadas como fallback / defaults amistosos ===
        public static YTBComponents ScriptTemplate() => new()
        {
            ComponentName = "ScriptComponent",
            Propiedades = new() { new("Scripts", "CSHARP&:&&;&") }
        };

        public static YTBComponents TransformTemplate() => new()
        {
            ComponentName = "TransformComponent",
            Propiedades = new()
            {
                new("Position", "0,0,1"),
                new("Size", "100,100,0"),
                new("Color", "White"),
                new("SpriteEffects", "None"),
                new("Scale", "1"),
                new("Rotation", "0")
            }
        };

        public static YTBComponents Sprite2DTemplate() => new()
        {
            ComponentName = "SpriteComponent2D",
            Propiedades = new()
            {
                new("TextureAtlasPath", ""),
                new("SpriteName", ""),
                new("SourceRectangle", "0,0,0,0"),
                new("IsVisible", "true"),
                new("2.5D", "false")
            }
        };

        public static YTBComponents Animation2DTemplate() => new()
        {
            ComponentName = "AnimationComponent2D",
            Propiedades = new()
            {
                new("TextureAtlasPath", ""),
                new("AnimationBindings", ""),
                new("CurrentAnimationType", "none")
            }
        };

        public static YTBComponents Rigibody2DTemplate() => new()
        {
            ComponentName = "RigidBodyComponent2D",
            Propiedades = new()
            {
                new("OffSetCollision", "0,0"),
                new("Velocity", "0,0,0"),
                new("Mass", "0"),
                new("Collide", "Solid")
            }
        };

        public static YTBComponents Button2DTemplate() => new()
        {
            ComponentName = "ButtonComponent2D",
            Propiedades = new()
            {
                new("IsActive", "true"),
                new("EffectiveArea", "0,0,0,0"),
                new("Description", "None")
            }
        };

        public static YTBComponents InputTemplate() => new()
        {
            ComponentName = "InputComponent",
            Propiedades = new()
            {
                new("InputsInUse", ""),
                new("GamePadIndex", ""),
                new("KeyboardMappings", "MoveUp:W,\nMoveDown:S,\nMoveLeft:A,\nMoveRight:D,"),
                new("MouseMappings", "")
            }
        };

        public static YTBComponents CameraTemplate() => new()
        {
            ComponentName = nameof(CameraComponent3D),
            Propiedades = new()
            {
                new("EntityName", ""),
                new("InitialPosition", "0,60,30"),
                new("OffsetCamera", "0,50,-100"),
                new("AngleView", "60"),
                new("NearRender", "10"),
                new("FarRender", "3000")
            }
        };

        public static YTBComponents TileMap2DTemplate() => new()
        {
            ComponentName = "TileMapComponent2D",
            Propiedades = new() { new("TileMapPath", "") }
        };

        public static YTBComponents Font2DTemplate() => new()
        {
            ComponentName = "FontComponent2D",
            Propiedades = new()
            {
                new("Texto", "Texto de ejemplo"),
                new("Font", "Fonts/Hud"),
                new("IsVisible", "true")
            }
        };

        public static YTBComponents ShaderTemplate() => new()
        {
            ComponentName = "ShaderComponent",
            Propiedades = new()
            {
                new("ShaderPath", ""),
                new("IsActive", "true"),
                new("params", "")
            }
        };

        public static YTBComponents Model3DTemplate() => new()
        {
            ComponentName = "ModelComponent3D",
            Propiedades = new()
            {
                new("ModelPath", ""),
                new("IsVisible", "true"),
                new("SphereRadius", "0"),
                new("OffsetSphere", "0,0,0")
            }
        };
    }
}
