using System;
using System.Collections.Generic;
using System.Linq;
using YotsubaEngine.ActionFiles.YTB_Files;

namespace YotsubaEngine.Templates
{
    /// <summary>
    /// Fachada delgada sobre <see cref="YTBFileToGameData._defaultValuesByComponent"/> (mapa estático
    /// generado por <c>YTBContentBuilder</c> a partir de los <c>[UIComponentValue(defaultValue: ...)]</c>).
    /// Ya no contiene plantillas hardcoded — todas las constantes ahora viven en los atributos del propio componente.
    /// </summary>
    public static class EntityYTBXmlTemplate
    {
        /// <summary>
        /// Construye una entidad por defecto con TODOS los componentes registrados (cada uno con sus
        /// <c>defaultValue</c> declarados). Reemplaza el cuerpo hardcoded original que tenía un <c>List&lt;YTBComponents&gt;</c> literal.
        /// </summary>
        internal static YTBEntity GenerateNew()
        {
            var components = new List<YTBComponents>();
            foreach (var kv in YTBFileToGameData._defaultValuesByComponent)
            {
                components.Add(TemplateFor(kv.Key));
            }
            // CustomComponent es un placeholder genérico fuera del sistema de atributos: lo añadimos a mano.
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

        /// <summary>
        /// Construye un <see cref="YTBComponents"/> para el tipo cuyo <c>SerializableName</c> coincida con <paramref name="componentName"/>,
        /// usando los <c>defaultValue</c> declarados en cada <c>[UIComponentValue]</c>.
        /// </summary>
        public static YTBComponents TemplateFor(string componentName)
        {
            if (!YTBFileToGameData._defaultValuesByComponent.TryGetValue(componentName, out var defaults))
            {
                return new YTBComponents
                {
                    ComponentName = componentName,
                    Propiedades = new List<Tuple<string, string>>()
                };
            }

            return new YTBComponents
            {
                ComponentName = componentName,
                Propiedades = defaults
                    .Select(kv => new Tuple<string, string>(kv.Key, kv.Value))
                    .ToList()
            };
        }

        // ===== Helpers nombrados por compatibilidad con call-sites previos =====
        public static YTBComponents TransformTemplate() => TemplateFor("TransformComponent");
        public static YTBComponents Sprite2DTemplate() => TemplateFor("SpriteComponent2D");
        public static YTBComponents Animation2DTemplate() => TemplateFor("AnimationComponent2D");
        public static YTBComponents Rigibody2DTemplate() => TemplateFor("RigidBodyComponent2D");
        public static YTBComponents Button2DTemplate() => TemplateFor("ButtonComponent2D");
        public static YTBComponents InputTemplate() => TemplateFor("InputComponent");
        public static YTBComponents CameraTemplate() => TemplateFor("CameraComponent3D");
        public static YTBComponents ScriptTemplate() => TemplateFor("ScriptComponent");
        public static YTBComponents TileMap2DTemplate() => TemplateFor("TileMapComponent2D");
        public static YTBComponents Font2DTemplate() => TemplateFor("FontComponent2D");
        public static YTBComponents ShaderTemplate() => TemplateFor("ShaderComponent");
        public static YTBComponents Model3DTemplate() => TemplateFor("ModelComponent3D");
    }
}
