using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.System.GumUI;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using YotsubaEngine.Graphics;
using YotsubaEngine.Graphics.ImGuiNet;
using YotsubaEngine.Scripting;
namespace YotsubaEngine
{
    /// <summary>
    /// Main game host for the Yotsuba engine runtime.
    /// Anfitrión principal del juego para el runtime del motor Yotsuba.
    /// </summary>
    public class YTBGame : Game
    {
        /// <summary>
        /// Indicates whether the game is running on a mobile platform.
        /// Indica si el juego se está ejecutando en una plataforma móvil.
        /// </summary>
        public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

        /// <summary>
        /// Indicates if the game is running on a desktop platform.
        /// Indica si el juego se está ejecutando en una plataforma de escritorio.
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

        /// <summary>
        /// Stores the global game instance.
        /// Almacena la instancia global del juego.
        /// </summary>
        private static Game instance;

        /// <summary>
        /// Gets the global game instance.
        /// Obtiene la instancia global del juego.
        /// </summary>
        public static Game Instance { get => instance; private set => instance = value; }

        /// <summary>
        /// Provides access to the graphics device manager.
        /// Proporciona acceso al administrador del dispositivo gráfico.
        /// </summary>
        public GraphicsDeviceManager _graphics;

        /// <summary>
        /// Provides the shared sprite batch for rendering.
        /// Proporciona el sprite batch compartido para el renderizado.
        /// </summary>
        public SpriteBatch _spriteBatch;

        /// <summary>
        /// Manages engine events for the game session.
        /// Gestiona los eventos del motor para la sesión de juego.
        /// </summary>
        public EventManager EventManager;

        /// <summary>
        /// Tracks the active scene manager instance.
        /// Mantiene la instancia del administrador de escenas activo.
        /// </summary>
        public SceneManager SceneManager;

        /// <summary>
        /// Gets or sets the active script registry.
        /// Obtiene o establece el registro de scripts activo.
        /// </summary>
        public static IScriptRegistry ScriptRegistry { get; set; }

        /// <summary>
        /// Gets or sets the active model registry.
        /// Obtiene o establece el registro de modelos activo.
        /// </summary>
        public static IModelRegistry ModelRegistry { get; set; }

        /// <summary>
        /// Gets or sets the ImGui renderer instance.
        /// Obtiene o establece la instancia del renderizador ImGui.
        /// </summary>
        public static ImGuiRenderer GuiRenderer { get; set; }

        /// <summary>
        /// Creates a new Yotsuba game host instance.
        /// Crea una nueva instancia anfitriona del juego Yotsuba.
        /// </summary>
        public YTBGame(bool isMouseVisible = true) : base()
        {
            Instance = this;
            //_graphics = graphicsDeviceManager;
            // Configurar Content.RootDirectory con la carpeta de assets compilados
            // Por defecto es "Content", pero puede cambiarse antes de crear la instancia del juego
            Content.RootDirectory = YTBGlobalState.CompiledAssetsFolderName;
            IsMouseVisible = isMouseVisible;

            Window.Title = "Yotsuba Engine";
            Window.AllowUserResizing = true;

            YTBGlobalState.ContentManager = Content;

            if (IsDesktop)
            {
                Window.FileDrop += DragAndDropSystem.Window_FileDrop;
            }

            SetConfig();
        }

        protected virtual void SetConfig()
        {

        }

        protected virtual void SetConfig(string Title)
        {
            Window.Title = Title;
        }

        /// <summary>
        /// Assigns the script registry used by the engine.
        /// Asigna el registro de scripts usado por el motor.
        /// </summary>
        public void SetScriptManager(IScriptRegistry scriptRegistry)
        {
            YTBGame.ScriptRegistry = scriptRegistry;
        }

        /// <summary>
        /// Assigns the model registry used by the engine.
        /// Asigna el registro de modelos usado por el motor.
        /// </summary>
        public void SetModelRegistry(IModelRegistry modelRegistry)
        {
            YTBGame.ModelRegistry = modelRegistry;
        }

        /// <summary>
        /// Initializes the graphics device and window settings.
        /// Inicializa el dispositivo gráfico y la configuración de la ventana.
        /// </summary>
        public virtual void InitializeGraphicsDevice(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice GraphicsDevice, int width = 1920, int height = 1080, bool fullScreen = false)
        {
            _graphics = graphicsDeviceManager;
            graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphicsDeviceManager.PreferredBackBufferWidth = width;  // Ancho
            graphicsDeviceManager.PreferredBackBufferHeight = height; // Alto
            graphicsDeviceManager.IsFullScreen = fullScreen;
            graphicsDeviceManager.ApplyChanges();

            YTBGlobalState.GraphicsDevice = _graphics.GraphicsDevice;
        }

        /// <summary>
        /// Holds the font range for ImGui glyphs.
        /// Contiene el rango de fuente para glifos de ImGui.
        /// </summary>
        ushort[] ranges = { 0x0020, 0x00FF, 0 };

        /// <summary>
        /// Initializes the engine and ImGui services.
        /// Inicializa el motor y los servicios de ImGui.
        /// </summary>
        protected override void Initialize()
        {

            YTBGum.Initialize(this);
            if (YTBGlobalState.EngineEnabled && OperatingSystem.IsWindows())
            {
                GuiRenderer = new ImGuiRenderer(this);
                // ImGui setup (fonts, theme)
                var io = ImGui.GetIO();
                io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

                unsafe
                {
                    fixed (ushort* rangePtr = ranges)
                    {
                        //3.Cargar la fuente desde el archivo
                        //Ajusta la ruta y el tamaño(18.0f) a tu gusto.
                        // El tercer parámetro es la configuración(null usa default).
                        // El cuarto parámetro es el puntero al rango.

                        // Prefer runtime-copied engine font under the output `Fonts` folder
                        string outputFontsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");
                        string fuente = Path.Combine(outputFontsDir, "LibertinusMath-Regular.ttf");

                        io.Fonts.AddFontFromFileTTF(fuente, 19.0f, null, (IntPtr)rangePtr);
                    }
                }

                io.FontGlobalScale = 1.0f;
                ImGuiThemeColors.AplicarTemaCompleto();
                GuiRenderer.RebuildFontAtlas();

                WriteYTBFile.CreateYTBGameFile();

                // Configurar el título de la ventana desde YTBConfig
                try
                {
                    var config = ReadYTBFile.ReadYTBGameConfigFile().ConfigureAwait(false).GetAwaiter().GetResult();

                    if (!string.IsNullOrWhiteSpace(config?.GameName))
                    {
                        Window.Title = config.GameName;
                    }
                    else
                    {
                        Window.Title = "Yotsuba Engine";
                    }
                }
                catch (Exception ex)
                {
                    EngineUISystem.SendLog($"[YTBGame] No se pudo cargar el nombre del juego desde la configuración: {ex.Message}");
                    Window.Title = "Yotsuba Engine";
                }
            }

#if YTB

            if (YTBGlobalState.EngineEnabled)
            {
                EventManager.Instance.Subscribe<OnChangeEsceneManager>(SceneManagerChanged);
            }

#endif
            base.Initialize();
        }

#if YTB
        /// <summary>
        /// Handles scene manager changes while debugging.
        /// Maneja los cambios del administrador de escenas durante depuración.
        /// </summary>
        private void SceneManagerChanged(OnChangeEsceneManager manager)
        {
            SceneManager = manager.SceneManager;
            SceneManager.CurrentScene.Initialize(Content);
        }

#endif
        /// <summary>
        /// Loads engine content and initializes the scene.
        /// Carga el contenido del motor e inicializa la escena.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            try
            {
                _spriteBatch = new SpriteBatch(GraphicsDevice);
                
                // Initialize the audio system
                // Inicializar el sistema de audio
                AudioSystem.Initialize();
                
                SceneManager = YTBFileToGameData.GenerateSceneManager(_graphics).GetAwaiter().GetResult();

                SceneManager.CurrentScene.Initialize(Content);
            }
            catch (Exception ex)
            {
                EngineUISystem.SendLog($"[YTBGame] Error al cargar el contenido del juego: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the active scene and resolves queued events.
        /// Actualiza la escena activa y resuelve los eventos en cola.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
#if YOTSUBA
            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
#endif

            SceneManager.CurrentScene.Update(gameTime);
            EventManager.Instance.ResolveEvents();
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the active scene and clears the frame buffer.
        /// Dibuja la escena activa y limpia el frame buffer.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //GraphicsDevice.Clear(Color.DarkOrange);
            //GraphicsDevice.Clear(Color.BlanchedAlmond);
            GraphicsDevice.Clear(YTBGlobalState.EngineBackground);

            SceneManager.CurrentScene.Draw(gameTime, _spriteBatch);



        }

        /// <summary>
        /// Starts the game loop.
        /// Inicia el bucle principal del juego.
        /// </summary>
        public virtual void GameRun()
        {
            Run();
        }
    }
}
