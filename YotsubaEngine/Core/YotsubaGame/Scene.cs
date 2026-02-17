
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;
using YotsubaEngine.Core.Component.C_2D;
using YotsubaEngine.Core.System.S_2D;
using YotsubaEngine.Core.System.S_3D;
using YotsubaEngine.Core.System.S_AGNOSTIC;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.YTB_Toolkit;

namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// La representación de una escena del juego.
    /// <para>The representation of a game scene.</para>
    /// </summary>
    public class Scene
    {

        bool isWindows = OperatingSystem.IsWindows();

        /// <summary>
        /// Nombre de la escena.
        /// <para>Scene name.</para>
        /// </summary>
        public string SceneName { get; set; }
        /// <summary>
        /// Una instancia del administrador de entidades.
        /// <para>An instance of the entity manager.</para>
        /// </summary>
        public EntityManager EntityManager { get; set; }

        /// <summary>
        /// Instancia global del control WASD.
        /// <para>Global instance of the WASD control.</para>
        /// </summary>
        public static WASDControl ToolkitWASDControl { get; private set; }
        /// <summary>
        /// La instancia global del administrador de eventos.
        /// <para>Global instance of the event manager.</para>
        /// </summary>
        public EventManager EventManager { get; set; }

        #region Sistemas del engine

        internal FontSystem2D FontSystem2D;
        /// <summary>
        /// Sistema encargado de gestionar todas las animaciones de las entidades del juego
        /// </summary>

        private AnimationSystem2D AnimationSystem2D;

        /// <summary>
        /// Sistema encargado de gestionar los botones, cuando están presionados, etc... dentro del juego
        /// </summary>
        private ButtonSystem2D ButtonSystem2D;

        /// <summary>
        /// Sistema que se encarga de toda la física del mundo (mover a las entidades)
        /// </summary>
        private PhysicsSystem2D PhysicsSystem2D;

        /// <summary>
        /// Sistema de renderizado de la UI. Ultimo sistema en ejecutarse.
        /// Renderiza la UI despues de que el estado del juego se actualizo por completo en los demás sistemas.
        /// </summary>
        private RenderSystem2D RenderSystem2D;

        private GumUISystem2D GumUISystem2D;

        /// <summary>
        /// Sistema que gestiona todo lo que se ve en pantalla y que sea 3D específicamente, renderizar modelos 3D.
        /// </summary>
        private RenderSystem3D RenderSystem3D;

        /// <summary>
        /// Sistema encargado de colocar aplicar las camaras a la escena
        /// </summary>
        private CameraSystem CameraSystem;

        /// <summary>
        /// Sistema que se encarga de manejar la entrada del usuario (teclado, ratón, gamepad).
        /// </summary>
        private InputSystem InputSystem;

        /// <summary>
        /// Sistema que se encarga de manejar los scripts de las entidades.
        /// <para>System responsible for handling entity scripts.</para>
        /// </summary>
        public ScriptSystem ScriptSystem;

        /// <summary>
        /// Sistema que se encarga de dibujar los tilemaps en pantalla
        /// </summary>
        private TileMapSystem2D TilemapSystem;

        /// <summary>
        /// Clase propia del engine (NO SE EJECUTA EN PRODUCCION) que coordina los subsistemas UI. Mantiene referencias compartidas
        /// (GameInfo, GuiRenderer, SelectedEntity, etc.) y delega el render a clases pequeñas.
        /// </summary>
        private EngineUISystem EngineUISystem;

        private DragAndDropSystem DragAndDropSystem;

//-:cnd:noEmit
#if YTB
        /// <summary>
        /// Sistema que permite arrastrar entidades con FontComponent2D usando Ctrl + Shift + Click Izquierdo.
        /// Solo disponible en modo DEBUG.
        /// </summary>
        private FontDragSystem FontDragSystem;
#endif
//+:cnd:noEmit
        #endregion

        /// <summary>
        /// Constructor que recibe los gráficos para pasárselos al sistema de cámara.
        /// <para>Constructor that receives graphics to pass to the camera system.</para>
        /// </summary>
        /// <param name="_graphics">Administrador de gráficos. <para>Graphics device manager.</para></param>
        public Scene(GraphicsDeviceManager _graphics)
        {
            EventManager = EventManager.Instance;
            EntityManager = new();
            AnimationSystem2D = new AnimationSystem2D();
            ButtonSystem2D = new ButtonSystem2D();
            PhysicsSystem2D = new PhysicsSystem2D();
            RenderSystem2D = new RenderSystem2D();
            GumUISystem2D = new();
            RenderSystem3D = new RenderSystem3D();
            CameraSystem = new CameraSystem(_graphics);
            InputSystem = new InputSystem();
            EngineUISystem = new EngineUISystem();
            ScriptSystem = new();
            TilemapSystem = new();
            DragAndDropSystem = new();
            FontSystem2D = new();
//-:cnd:noEmit
#if YTB
            FontDragSystem = new();
#endif
//+:cnd:noEmit
        }


        /// <summary>
        /// Primer método que se ejecuta y prepara todos los sistemas.
        /// <para>First method executed that prepares all systems.</para>
        /// </summary>
        /// <param name="content">Administrador de contenido. <para>Content manager.</para></param>
        public void Initialize(ContentManager content)
        {
            ToolkitWASDControl = new(EntityManager);
            ToolkitWASDControl.Initialize();
            AnimationSystem2D.InitializeSystem(EntityManager);
            ButtonSystem2D.InitializeSystem(EntityManager);
            PhysicsSystem2D.InitializeSystem(EntityManager);
            RenderSystem2D.InitializeSystem(EntityManager);
            GumUISystem2D.InitializeSystem(EntityManager);
            RenderSystem3D.InitializeSystem(EntityManager);
            CameraSystem.InitializeSystem(EntityManager);
            InputSystem.InitializeSystem(EntityManager);
            ScriptSystem.InitializeSystem(EntityManager);
            TilemapSystem.InitializeSystem(EntityManager);
            DragAndDropSystem.InitializeSystem(EntityManager);
            FontSystem2D.InitializeSystem(EntityManager);
//-:cnd:noEmit
#if YTB
            FontDragSystem.InitializeSystem(EntityManager);
            FontDragSystem.SetFontSystem(FontSystem2D); // Pasar la referencia del FontSystem2D
#endif
//+:cnd:noEmit
//-:cnd:noEmit
#if YTB
            if (OperatingSystem.IsWindows())
            {
                EngineUISystem.InitializeSystem(EntityManager, content);
            }
#endif
//+:cnd:noEmit

            foreach(var entity in EntityManager.YotsubaEntities)
            {
                ScriptSystem.SharedEntityInitialize(entity);
                AnimationSystem2D.SharedEntityInitialize(entity);
                ButtonSystem2D.SharedEntityInitialize(entity);
                PhysicsSystem2D.SharedEntityInitialize(entity);
                CameraSystem.SharedEntityInitialize(entity);
                InputSystem.SharedEntityInitialize(entity);
                FontSystem2D.SharedEntityInitialize(entity);

            }
        }


        /// <summary>
        /// Método que se ejecuta en cada frame para actualizar el estado del juego.
        /// <para>Method executed each frame to update the game state.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        public void Update(GameTime gameTime)
        {
			InputSystem.UpdateSystem(gameTime);

			if (EntityManager.YotsubaEntities == null || EntityManager.YotsubaEntities.Count == 0) return;
            PhysicsSystem2D.UpdateSystem(gameTime);
            ButtonSystem2D.UpdateSystem(gameTime);
            AnimationSystem2D.UpdateSystem(gameTime);
            GumUISystem2D.UpdateSystem(gameTime);
            CameraSystem.UpdateSystem(gameTime);
            try
            {
                foreach (var entity in EntityManager.YotsubaEntities)
                {
                    ScriptSystem.SharedEntityForEachUpdate(entity, gameTime);
                    AnimationSystem2D.SharedEntityForEachUpdate(entity, gameTime);
                    ButtonSystem2D.SharedEntityForEachUpdate(entity, gameTime);
                    PhysicsSystem2D.SharedEntityForEachUpdate(entity, gameTime);
                    CameraSystem.SharedEntityForEachUpdate(entity, gameTime);
                    InputSystem.SharedEntityForEachUpdate(entity, gameTime);
                    DragAndDropSystem.SharedEntityForEachUpdate(entity, gameTime);
//-:cnd:noEmit
#if YTB
                    FontDragSystem.SharedEntityForEachUpdate(entity, gameTime);
#endif
//+:cnd:noEmit

                }
            }catch(Exception ex)
            {
                EngineUISystem.SendLog($"Error in Scene.Update: {ex.Message}");
//-:cnd:noEmit
#if YTB
                EngineUISystem.SendLog($"StackTrace: {ex.StackTrace}");
#endif
//+:cnd:noEmit
            }
        }

        /// <summary>
        /// Método que se ejecuta en cada frame para dibujar la escena y la UI.
        /// <para>Method executed each frame to draw the scene and UI.</para>
        /// </summary>
        /// <param name="gameTime">Tiempo de juego. <para>Game time.</para></param>
        /// <param name="_spriteBatch">SpriteBatch compartido. <para>Shared sprite batch.</para></param>
        public void Draw(GameTime gameTime, SpriteBatch _spriteBatch)
        {




            if (EntityManager.YotsubaEntities == null || EntityManager.YotsubaEntities.Count == 0) return;

            TilemapSystem.UpdateSystem(gameTime, _spriteBatch);

            RenderSystem2D.UpdateSystem(_spriteBatch, gameTime);
            //RenderSystem3D.UpdateSystem(gameTime);

            FontSystem2D.DrawSystem(gameTime, _spriteBatch);

            GumUISystem2D.DrawSystem(gameTime);


            Draw3D(gameTime);

            Draw2D(gameTime, _spriteBatch);
//-:cnd:noEmit
#if YTB
            if (isWindows)
            {
                EngineUISystem.UpdateSystem(_spriteBatch, gameTime);
            }


#endif
//+:cnd:noEmit
        }

        void Draw3D(GameTime gameTime)
        {

            var gd = YTBGlobalState.GraphicsDevice;

            // Save current states (left by previous SpriteBatch.End() calls)
            // Note: MonoGame state objects are immutable singletons, so storing references is safe
            var oldBlendState = gd.BlendState;
            var oldDepthStencilState = gd.DepthStencilState;
            var oldRasterizerState = gd.RasterizerState;
            var oldSamplerState = gd.SamplerStates[0];

            gd.BlendState = BlendState.Opaque;
            gd.DepthStencilState = DepthStencilState.Default;
            gd.RasterizerState = RasterizerState.CullCounterClockwise;
            gd.SamplerStates[0] = SamplerState.LinearWrap;

            RenderSystem3D.UpdateSystem(gameTime);
            ScriptSystem.DrawSystem3D(gameTime);

            gd.BlendState = oldBlendState;
            gd.DepthStencilState = oldDepthStencilState;
            gd.RasterizerState = oldRasterizerState;
            gd.SamplerStates[0] = oldSamplerState;
        }

        void Draw2D(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            ScriptSystem.DrawSystem2D(gameTime, _spriteBatch);
        }

    }
}
