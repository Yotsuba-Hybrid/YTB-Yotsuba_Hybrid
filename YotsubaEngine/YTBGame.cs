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
    /// Anfitrión principal del juego para el runtime del motor Yotsuba.
    /// <para>Main game host for the Yotsuba engine runtime.</para>
    /// </summary>
    public class YTBGame : Game
    {
        /// <summary>
        /// Indica si el juego se está ejecutando en una plataforma móvil.
        /// <para>Indicates whether the game is running on a mobile platform.</para>
        /// </summary>
        public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

        /// <summary>
        /// Indica si el juego se está ejecutando en una plataforma de escritorio.
        /// <para>Indicates if the game is running on a desktop platform.</para>
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

        /// <summary>
        /// Stores the global game instance.
        /// Almacena la instancia global del juego.
        /// </summary>
        private static Game instance;

        /// <summary>
        /// Obtiene la instancia global del juego.
        /// <para>Gets the global game instance.</para>
        /// </summary>
        public static Game Instance { get => instance; private set => instance = value; }

        /// <summary>
        /// Proporciona acceso al administrador del dispositivo gráfico.
        /// <para>Provides access to the graphics device manager.</para>
        /// </summary>
        public GraphicsDeviceManager _graphics;

        /// <summary>
        /// Proporciona el sprite batch compartido para el renderizado.
        /// <para>Provides the shared sprite batch for rendering.</para>
        /// </summary>
        public SpriteBatch _spriteBatch;

        /// <summary>
        /// Gestiona los eventos del motor para la sesión de juego.
        /// <para>Manages engine events for the game session.</para>
        /// </summary>
        public EventManager EventManager;

        /// <summary>
        /// Mantiene la instancia del administrador de escenas activo.
        /// <para>Tracks the active scene manager instance.</para>
        /// </summary>
        public SceneManager SceneManager;

        /// <summary>
        /// Obtiene o establece el registro de scripts activo.
        /// <para>Gets or sets the active script registry.</para>
        /// </summary>
        public static IScriptRegistry ScriptRegistry { get; set; }

        /// <summary>
        /// Obtiene o establece el registro de modelos activo.
        /// <para>Gets or sets the active model registry.</para>
        /// </summary>
        public static IModelRegistry ModelRegistry { get; set; }

        /// <summary>
        /// Obtiene o establece la instancia del renderizador ImGui.
        /// <para>Gets or sets the ImGui renderer instance.</para>
        /// </summary>
        public static ImGuiRenderer GuiRenderer { get; set; }

        /// <summary>
        /// Crea una nueva instancia anfitriona del juego Yotsuba.
        /// <para>Creates a new Yotsuba game host instance.</para>
        /// </summary>
        /// <param name="isMouseVisible">Indica si el cursor del mouse es visible. <para>Whether the mouse cursor is visible.</para></param>
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
            Window.IsBorderless = false;
            YTBGlobalState.ContentManager = Content;

            if (IsDesktop)
            {
                Window.FileDrop += DragAndDropSystem.Window_FileDrop;
            }

        }

        protected virtual void SetConfig()
        {

        }



        /// <summary>
        /// Asigna el registro de scripts usado por el motor.
        /// <para>Assigns the script registry used by the engine.</para>
        /// </summary>
        /// <param name="scriptRegistry">Registro de scripts a asignar. <para>Script registry to assign.</para></param>
        public void SetScriptManager(IScriptRegistry scriptRegistry)
        {
            YTBGame.ScriptRegistry = scriptRegistry;
        }

        /// <summary>
        /// Asigna el registro de modelos usado por el motor.
        /// <para>Assigns the model registry used by the engine.</para>
        /// </summary>
        /// <param name="modelRegistry">Registro de modelos a asignar. <para>Model registry to assign.</para></param>
        public void SetModelRegistry(IModelRegistry modelRegistry)
        {
            YTBGame.ModelRegistry = modelRegistry;
        }

        /// <summary>
        /// Inicializa el dispositivo gráfico y la configuración de la ventana.
        /// <para>Initializes the graphics device and window settings.</para>
        /// </summary>
        /// <param name="graphicsDeviceManager">Administrador del dispositivo gráfico. <para>Graphics device manager.</para></param>
        /// <param name="GraphicsDevice">Dispositivo gráfico activo. <para>Active graphics device.</para></param>
        /// <param name="width">Ancho del buffer preferido. <para>Preferred back buffer width.</para></param>
        /// <param name="height">Alto del buffer preferido. <para>Preferred back buffer height.</para></param>
        /// <param name="fullScreen">Indica si la pantalla es completa. <para>Whether to use full screen.</para></param>
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
                        string fuente = Path.Combine(outputFontsDir, "GeistPixel-Square.ttf");

                        io.Fonts.AddFontFromFileTTF(fuente, 20.0f, null, (IntPtr)rangePtr);
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
            try
            {

                _spriteBatch = new SpriteBatch(GraphicsDevice);
                
                // Initialize the audio system
                // Inicializar el sistema de audio
                AudioSystem.Initialize();
                
                SceneManager = YTBFileToGameData.GenerateSceneManager(_graphics);

                SceneManager.CurrentScene.Initialize(Content);
            }
            catch (Exception ex)
            {
                EngineUISystem.SendLog($"[YTBGame] Error al cargar el contenido del juego: {ex.Message}");
            }

            base.LoadContent();
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
        /// Inicia el bucle principal del juego.
        /// <para>Starts the game loop.</para>
        /// </summary>
        public virtual void GameRun()
        {
            Run();
        }
    }
}
