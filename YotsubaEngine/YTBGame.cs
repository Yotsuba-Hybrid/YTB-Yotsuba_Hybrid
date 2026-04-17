
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Text;
#if YTB
using YotsubaEngine.Core.System.YotsubaEngineUI;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;
using YotsubaEngine.Graphics.ImGuiNet;
using System.Threading.Tasks;
using YotsubaEngine.Core.System.YTBDragAndDrop;
using YotsubaEngine.Events.YTBEvents.EngineEvents;
using Hexa.NET.ImGui;
using Hexa.NET.ImPlot;
using Hexa.NET.ImNodes;
using Hexa.NET.ImGuizmo;
#endif
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Graphics;
using YotsubaEngine.Scripting;
using YotsubaEngine.Core.YotsubaGame.Scripting;
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

#if YTB
        /// <summary>
        /// Obtiene o establece la instancia del renderizador ImGui.
        /// <para>Gets or sets the ImGui renderer instance.</para>
        /// </summary>
        public static ImGuiRenderer GuiRenderer { get; set; }
#endif
        /// <summary>
        /// Crea una nueva instancia anfitriona del juego Yotsuba.
        /// <para>Creates a new Yotsuba game host instance.</para>
        /// </summary>
        /// <param name="isMouseVisible">Indica si el cursor del mouse es visible. <para>Whether the mouse cursor is visible.</para></param>
        public YTBGame(Platforms platform, bool isMouseVisible) : base()
        {

            YTBGlobalState.Platform = platform;
#if YTB
            if (IsDesktop)
                Console.ResetColor();
#endif


            Instance = this;
            IsMouseVisible = isMouseVisible;
            //_graphics = graphicsDeviceManager;
            // Configurar Content.RootDirectory con la carpeta de assets compilados
            // Por defecto es "Content", pero puede cambiarse antes de crear la instancia del juego
            Content.RootDirectory = YTBGlobalState.CompiledAssetsFolderName;

#if YTB
            (YTBGameInfo, YTBConfig) game;

            Task.Run(async () =>
                {

                    game = await ReadYTBFile.ReadYTBFiles(false);
                    YTBGlobalState.GameData = game;

                });

#endif
            //Window.Title = "Yotsuba Engine";
            //Window.AllowUserResizing = true;
            YTBGlobalState.ContentManager = Content;

#if YTB
            if (IsDesktop)
            {
                Window.FileDrop += DragAndDropSystem.Window_FileDrop;
            }
#endif

        }

        protected virtual void SetConfig()
        {
            AddSystems();
        }

        protected virtual void AddSystems()
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
            GraphicsDevice = _graphics.GraphicsDevice;
            YTBGlobalState.GraphicsDeviceManager = graphicsDeviceManager;
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
            YTBGlobalState.GraphicsDevice = GraphicsDevice;

            if (YTBGlobalState.IsDesktop && YTBGlobalState.EngineEnabled)
            {

#if YTB

                GuiRenderer = new ImGuiRenderer(this);
                // ImGui setup (fonts, theme)
                var io = ImGui.GetIO();
                ImGui.GetIO().BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
                if (!IsMobile)
                {
                    io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
                }


                const string outputFontsDir = "Fonts";
                string fuentePrincipal = Path.Combine(outputFontsDir, "LibertinusMath-Regular.ttf");
                string fuenteIconos = Path.Combine(outputFontsDir, "NerdFontsSymbolsOnly.ttf");

              

                Platforms platforms = YTBGlobalState.Platform;

                if (YTBGlobalState.IsDesktop)
                {
                    unsafe
                    {

                    // 1. Cargar la fuente principal (texto)
                    // Usamos GetGlyphRangesDefault() para que cargue el alfabeto normal (ASCII).
                    io.Fonts.AddFontFromFileTTF(fuentePrincipal, platforms == Platforms.Avalonia_GL ? 20.0f : 24.0f, null, io.Fonts.GetGlyphRangesDefault());

                        // 2. Crear la configuración para la fuente de íconos
                        ImFontConfigPtr config = ImGui.ImFontConfig();
                        config.MergeMode = true;
                        config.PixelSnapH = true;


                        // 3. Definir el rango de caracteres para Nerd Fonts.
                        uint[] iconRanges = new uint[]
                        {
                        0xE000, 0xF8FF,
                        0
                        };

                        fixed (uint* rangePtr = iconRanges)
                        {
                            byte[] fuenteBytes = Encoding.UTF8.GetBytes(fuenteIconos);

                            fixed (byte* fb = fuenteBytes)
                            {


                                // Pass rangePtr instead of GetGlyphRangesDefault()
                                io.Fonts.AddFontFromFileTTF(fb, platforms == Platforms.Avalonia_GL ? 20.0f : 24.0f, config, rangePtr);
                            }
                        }

                        config.Destroy();
                    }

                    io.FontGlobalScale = 1;

                    // 6. Construir la textura final (Obligatorio después de añadir fuentes)
                    io.Fonts.Build();
                }

                // Nota: Algunos wrappers de MonoGame (ImGuiRenderer) requieren que llames a un método 
                // interno para actualizar la textura en la GPU. Si no ves las fuentes, puede que necesites:
                // GuiRenderer.RebuildFontAtlas();
                ImGuiThemeColors.AplicarTemaCompleto();

                var style = ImGui.GetStyle();

                if (YTBGlobalState.IsAndroid)
                {
                    GuiRenderer.InitNativeBackend();
                }
                else if (!YTBGlobalState.IsIOS)
                {
                    GuiRenderer.RebuildFontAtlas();
                }
                var guiContext = ImGui.GetCurrentContext();

                if (YTBGlobalState.IsDesktop)
                {
                    ImPlot.SetImGuiContext(guiContext);
                    var plotContext = ImPlot.CreateContext();
                    ImPlot.SetCurrentContext(plotContext);

                    ImNodes.SetImGuiContext(guiContext);
                    var nodesContext = ImNodes.CreateContext();
                    ImNodes.SetCurrentContext(nodesContext);
                    var editorCtx = ImNodes.EditorContextCreate();
                    ImNodes.EditorContextSet(editorCtx);
                    ImNodes.StyleColorsDark(ImNodes.GetStyle());

                    // Le pasamos el contexto principal de ImGui a ImGuizmo para que sepa dónde dibujar
                    ImGuizmo.SetImGuiContext(guiContext);
                }
#endif


                WriteYTBFile.CreateYTBGameFile();
                
                try
                {
                    YTBConfig config = YTBGlobalState.GameData.Item2;

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
#if YTB
                    EngineUISystem.SendLog($"[YTBGame] No se pudo cargar el nombre del juego desde la configuración: {ex.Message}");
#endif
                    Window.Title = "Yotsuba Engine";
                }
            }

            //-:cnd:noEmit
#if YTB

            if (YTBGlobalState.EngineEnabled)
            {
                EventManager.Instance.Subscribe<OnChangeEsceneManager>(SceneManagerChanged);
            }

#endif
            //+:cnd:noEmit
            base.Initialize();
        }


        //-:cnd:noEmit
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
//+:cnd:noEmit

        /// <summary>
        /// Loads engine content and initializes the scene.
        /// Carga el contenido del motor e inicializa la escena.
        /// </summary>
        protected override void LoadContent()
        {
#if YTB
            try
            {

#endif
                YTBGlobalState.GraphicsDevice = GraphicsDevice;
                _spriteBatch = new SpriteBatch(GraphicsDevice);

                // Initialize the audio system
                // Inicializar el sistema de audio
                AudioSystem.Initialize();

                SceneManager = YTBFileToGameData.GenerateSceneManager(_graphics);

                SceneManager.CurrentScene.Initialize(Content);
#if YTB
        }
            catch (Exception ex)
            {
                EngineUISystem.SendLog($"[YTBGame] Error al cargar el contenido del juego: {ex.Message}");
            }
#endif


            base.LoadContent();
        }

        /// <summary>
        /// Updates the active scene and resolves queued events.
        /// Actualiza la escena activa y resuelve los eventos en cola.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            //-:cnd:noEmit
#if YTB
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
#endif
            //+:cnd:noEmit

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

            GraphicsDevice.Clear(YTBGlobalState.ColorBackground);
            SceneManager.CurrentScene.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);


        }

        protected override void EndDraw()
        {
            if (YTBGlobalState.Platform == Platforms.Windows_WPF_DX12)
            {

            }
            else
            {
                base.EndDraw();
            }
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
