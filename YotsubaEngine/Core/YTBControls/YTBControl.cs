
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
    /// Provides an abstract base class for drawable UI controls that manage and render colored rectangles using a
    /// shared pixel texture.
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
        /// Gets a 1x1 white texture that can be used for drawing solid colored shapes or backgrounds.
        /// </summary>
        /// <remarks>This texture is commonly used as a utility for rendering rectangles, lines, or other
        /// primitives by scaling and tinting it as needed. The texture is typically initialized elsewhere and should be
        /// assigned before use.</remarks>
        public static Texture2D pixel;

        /// <summary>
        /// Represents the collection of rectangle indices used by the containing type.
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
        /// Initializes the pixel resource if it has not already been created.
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
        /// Adds one or more color and rectangle pairs to the collection.
        /// </summary>
        /// <remarks>This method appends all specified pairs to the end of the collection. The order of
        /// the added pairs is preserved.</remarks>
        /// <param name="parameters">An array of tuples, each containing a <see cref="Color"/> and a <see cref="Rectangle"/> to be added. Cannot
        /// be null.</param>
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
        /// Adds a new entity with the specified color and transform component to the entity manager.
        /// </summary>
        /// <param name="color">The color to assign to the new entity's transform component.</param>
        /// <param name="transformComponent">The transform component that defines the position and size of the new entity. Cannot be null.</param>
        /// <returns>The unique identifier of the newly added entity.</returns>
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
