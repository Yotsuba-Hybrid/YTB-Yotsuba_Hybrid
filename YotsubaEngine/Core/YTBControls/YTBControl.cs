
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Core.YTBMath;

namespace YotsubaEngine.Core.YTBControls
{

    /// <summary>
    /// Proporciona una clase base abstracta para controles UI dibujables que gestionan y renderizan rectángulos de color usando una textura de píxel compartida.
    /// <para>Provides an abstract base class for drawable UI controls that manage and render colored rectangles using a shared pixel texture.</para>
    /// </summary>
    /// <remarks>YTBControl maintains a collection of colored rectangles and offers methods to add rectangles
    /// and render them using a SpriteBatch. The shared pixel texture is initialized on first use and reused for
    /// efficient drawing. Derived classes can extend or customize behavior by overriding the Add and Draw
    /// methods.</remarks>
    public abstract class YTBControl : Yotsuba
    {

        /// <summary>
        /// Gets or sets the manager responsible for handling entity operations within the context.
        /// </summary>
        protected EntityManager EntityManager { get; set; }

        /// <summary>
        /// Obtiene una textura blanca 1x1 que puede usarse para dibujar formas o fondos sólidos.
        /// <para>Gets a 1x1 white texture that can be used for drawing solid colored shapes or backgrounds.</para>
        /// </summary>
        /// <remarks>This texture is commonly used as a utility for rendering rectangles, lines, or other
        /// primitives by scaling and tinting it as needed. The texture is typically initialized elsewhere and should be
        /// assigned before use.</remarks>
        public static Texture2D pixel;

        /// <summary>
        /// Representa la colección de índices de rectángulos usados por el control.
        /// <para>Represents the collection of rectangle indices used by the containing type.</para>
        /// </summary>
        public List<int> IndexRectangles;

        
        /// <summary>
        /// Initializes a new instance of the YTBControl class.
        /// </summary>
        /// <remarks>This constructor calls the Initialize method to set up the control. Derived classes
        /// should ensure that any required initialization is performed before using the control.</remarks>
        protected YTBControl(EntityManager entityManager) : base(0)
        {
            EntityManager = entityManager;
            Initialize();    
        }

        
        /// <summary>
        /// Inicializa el recurso de píxel si aún no se ha creado.
        /// <para>Initializes the pixel resource if it has not already been created.</para>
        /// </summary>
        /// <remarks>Call this method before performing operations that require the pixel resource. This
        /// method is safe to call multiple times; initialization occurs only if the resource is not already
        /// present.</remarks>
        public void Initialize()
        {
            if (pixel == null)
            {
                pixel = new(YTBGlobalState.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
            }
        }

        /// <summary>
        /// Agrega un rectángulo de color y devuelve el identificador de la entidad creada.
        /// <para>Adds a colored rectangle and returns the identifier of the created entity.</para>
        /// </summary>
        /// <param name="color">Color del rectángulo. <para>Rectangle color.</para></param>
        /// <param name="x">Posición X. <para>X position.</para></param>
        /// <param name="y">Posición Y. <para>Y position.</para></param>
        /// <param name="width">Ancho. <para>Width.</para></param>
        /// <param name="heigth">Alto. <para>Height.</para></param>
        /// <param name="scale">Escala. <para>Scale.</para></param>
        /// <param name="rotation">Rotación. <para>Rotation.</para></param>
        /// <param name="LayerDeep">Profundidad de capa. <para>Layer depth.</para></param>
        /// <param name="effect">Efecto de sprite. <para>Sprite effect.</para></param>
        /// <returns>Identificador de la entidad creada. <para>Identifier of the created entity.</para></returns>
        public int Add(Color color, float x = 0f, float y = 0f, float width = 300f, float heigth = 100f, float scale = 1f, float rotation = 0f, float LayerDeep = 0f, SpriteEffects effect = SpriteEffects.None)
        {
            TransformComponent transformComponent = new TransformComponent() 
            {
                Color = color,
                Scale = scale,
                Position = new Vector3(x, y, 0f),
                Size = new Vector3(width, heigth, 0f),
                Rotation = rotation,
                LayerDepth = LayerDeep,
                SpriteEffects = effect,
            };

            Rectangle rectangle = new Rectangle(
              YTBCartessian.Vector3ToVector2(transformComponent.Position).ToPoint(),
              YTBCartessian.Vector3ToVector2(transformComponent.Size).ToPoint()
                );

            SpriteComponent2D spriteComponent2D = new(pixel, rectangle);

            Yotsuba newEntity = new(0);
            EntityManager.AddEntity(ref newEntity);
            EntityManager.AddTransformComponent(newEntity, transformComponent);
            EntityManager.AddSpriteComponent(newEntity, spriteComponent2D);
            newEntity.AddComponent(YTBComponent.YTBUIElement);
            return newEntity.Id;
        }

        /// <summary>
        /// Agrega una nueva entidad con el color y componente de transformación especificados.
        /// <para>Adds a new entity with the specified color and transform component to the entity manager.</para>
        /// </summary>
        /// <param name="color">Color para el componente de transformación. <para>The color to assign to the new entity's transform component.</para></param>
        /// <param name="transformComponent">Componente de transformación que define posición y tamaño. <para>The transform component that defines the position and size of the new entity.</para></param>
        /// <returns>Identificador único de la entidad creada. <para>The unique identifier of the newly added entity.</para></returns>
        public int Add(Color color, TransformComponent transformComponent)
        {
            transformComponent.Color = color;

            Rectangle rectangle = new Rectangle(
              YTBCartessian.Vector3ToVector2(transformComponent.Position).ToPoint(),
              YTBCartessian.Vector3ToVector2(transformComponent.Size).ToPoint()
                );

            SpriteComponent2D spriteComponent2D = new(pixel, rectangle);

            Yotsuba newEntity = new(0);
            EntityManager.AddEntity(ref newEntity);
            EntityManager.AddTransformComponent(newEntity, transformComponent);
            EntityManager.AddSpriteComponent(newEntity, spriteComponent2D);

            newEntity.AddComponent(YTBComponent.YTBUIElement);

            return newEntity.Id;
        }
    }
}
