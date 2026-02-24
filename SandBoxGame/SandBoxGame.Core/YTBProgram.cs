using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SandBoxGame.Core.Localization;
using SandBoxGame.Core.Systems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using YotsubaEngine;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.System.S_AGNOSTIC;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Graphics;
using YotsubaEngine.Scripting;

namespace SandBoxGame.Core
{
    /// <summary>
    /// Clase principal del juego, responsable de gestionar componentes, ajustes y configuración específica de la plataforma.
    /// <para>The main class for the game, responsible for managing components, settings, and platform-specific configuration.</para>
    /// </summary>
    public class YTBProgram : YTBGame
    {

        /// <summary>
        /// Ancho de la ventana del juego. En desarrollo, se establece en 1920x1080 para facilitar la depuración en monitores de escritorio.
        /// <para>Game window width. In development it is set to 1920x1080 for easier debugging on desktop monitors.</para>
        /// </summary>
        public const int WINDOW_WIDTH = 1920;

        /// <summary>
        /// Alto de la ventana del juego. En desarrollo, se establece en 1920x1080 para facilitar la depuración en monitores de escritorio.
        /// <para>Game window height. In development it is set to 1920x1080 for easier debugging on desktop monitors.</para>
        /// </summary>
        public const int WINDOW_HEIGHT = 1080;

        /// <summary>
        /// Indica si el juego se inicia en pantalla completa.
        /// <para>Indicates whether the game starts in fullscreen.</para>
        /// </summary>
        public const bool IS_FULLSCREEN =
//-:cnd:noEmit
#if YTB
            // Para el editor visual
            false
#else
            // Para el juego
            true
#endif
//+:cnd:noEmit
;


//-:cnd:noEmit
#if YTB
        /// <summary>
        /// Indica si el cursor del ratón es visible.
        /// <para>Indicates whether the mouse cursor is visible.</para>
        /// </summary>
        public const bool IS_MOUSE_VISIBLE = true;
#else
        /// <summary>
        /// Indica si el cursor del ratón es visible.
        /// <para>Indicates whether the mouse cursor is visible.</para>
        /// </summary>
        public const bool IS_MOUSE_VISIBLE = true;
#endif
//+:cnd:noEmit

        /// <summary>
        /// Inicializa una nueva instancia del juego y configura los servicios y el renderizado.
        /// <para>Initializes a new game instance and configures services and rendering.</para>
        /// </summary>
        public YTBProgram() : base(IS_MOUSE_VISIBLE)
        {
            SetConfig();
        }

        protected override void AddSystems()
        {
            SystemBuilder.AddSystem<CustomExampleSystem>();
            base.AddSystems();
        }

        protected override void SetConfig()
        {
            _graphics = new(this);

            SetScriptManager(new ScriptRegistry());
            SetModelRegistry(new ModelRegistry());

            AudioSystem.Initialize();
            AudioAssets.InitializeAudioAssets();

            // Los assets compilados (.xnb) est�n en Content (por defecto)
            // Esta ruta se calcula autom�ticamente: DirectorioSalida + "Content"
            YTBGlobalState.CompiledAssetsFolderName = "Content";


            Content.RootDirectory = YTBGlobalState.CompiledAssetsFolderName;

            // Configure screen orientations.
            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            //-:cnd:noEmit
#if YTB || DEBUG
            System.Console.WriteLine("[SandBoxGameGame] Constructor start");
#endif
            //+:cnd:noEmit
            ///Coloca un background por defecto al juego y al engine.
            YTBGlobalState.EngineBackground = new Color(32, 40, 78, 255);

            ///Zoom de camara por defecto.
            YTBGlobalState.CameraZoom = 1f;

            #region Engine Config
            /// En desarrollo, los assets fisicos (.ytb, .cs, etc) est�n en Assets dentro de Platforms.Core
            ///NO TOCAR, A MENOS QUE HAYA CAMBIADO LA RUTA
            YTBGlobalState.DevelopmentAssetsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "..", "SandBoxGame.Core",
                "Assets"
            );

            ///NO TOCAR, A MENOS QUE HAYA CAMBIADO LA RUTA
            YTBGlobalState.ContentProjectPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "..", "SandBoxGame.Content",
                "SandBoxGame.Content.csproj"
                );

            SetScriptManager(new ScriptRegistry());
            SetModelRegistry(new ModelRegistry());

            AudioSystem.Initialize();
            AudioAssets.InitializeAudioAssets();

            // Los assets compilados (.xnb) est�n en Content (por defecto)
            // Esta ruta se calcula autom�ticamente: DirectorioSalida + "Content"
            YTBGlobalState.CompiledAssetsFolderName = "Content";

            #endregion


            //-:cnd:noEmit
#if YTB || DEBUG
            System.Console.WriteLine("[SandBoxGameGame] _graphics created");
#endif
            //+:cnd:noEmit
            // Share _graphics as a service.
            Services.AddService(typeof(GraphicsDeviceManager), _graphics);

            //Window.Title = "SandBoxGame - Yotsuba Engine";
            // Configurar la ruta ra�z del Content Manager para que apunte a los assets compilados
            Content.RootDirectory = YTBGlobalState.CompiledAssetsFolderName;

            _graphics.PreferMultiSampling = true;


            // Configure screen orientations.
            _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            InitializeGraphicsDevice(_graphics, GraphicsDevice, WINDOW_WIDTH, WINDOW_HEIGHT, IS_FULLSCREEN);

            // Precarga de texturas y fuentes generadas por el builder
            YotsubaGraphicsManager.InitializeAssets(AssetRegister.TextureAssets, AssetRegister.FontAssets);

            base.SetConfig();

            //-:cnd:noEmit
#if YTB || DEBUG
            System.Console.WriteLine("[SandBoxGameGame] InitializeGraphicsDevice called from ctor");
#endif
            //+:cnd:noEmit
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            // Load supported languages and set the default language.
            List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
            var languages = new List<CultureInfo>();
            for (int i = 0; i < cultures.Count; i++)
            {
                languages.Add(cultures[i]);
            }

            // TODO You should load this from a settings file or similar,
            // based on what the user or operating system selected.
            var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
            LocalizationManager.SetCulture(selectedLanguage);
            base.Initialize();

        }

        
        /// <summary>
        /// Loads game content, such as textures and particle systems.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        

        /// <summary>
        /// Updates the game's logic, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for game updates.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game's graphics, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for rendering.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {

            // Clears the screen with the MonoGame orange color before drawing.
            //GraphicsDevice.Clear(Color.MonoGameOrange);
            // TODO: Add your drawing code here
           
            base.Draw(gameTime);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //pincel3d.DrawBox(new Vector3(2000, 0, 500), new Vector3(500), Color.DodgerBlue, YTBGlobalState.Game.SceneManager.CurrentScene.EntityManager.Camera.ViewMatrix, YTBGlobalState.Game.SceneManager.CurrentScene.EntityManager.Camera.ProjectionMatrix);
            //pincel3d.DrawSquare(new Vector3(200, 0, 500), 500, Color.White, YTBGlobalState.Game.SceneManager.CurrentScene.EntityManager.Camera.ViewMatrix, YTBGlobalState.Game.SceneManager.CurrentScene.EntityManager.Camera.ProjectionMatrix);
        }
    }
}
