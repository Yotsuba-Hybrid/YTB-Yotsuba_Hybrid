using FuelCell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SandBoxGame.Core.Localization;
using SandBoxGame.Core.Scripts.ModelScreen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using YotsubaEngine;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Graphics;
using YotsubaEngine.Scripting;

namespace SandBoxGame.Core
{
    /// <summary>
    /// The main class for the game, responsible for managing game components, settings, 
    /// and platform-specific configurations.
    /// </summary>
    public class YTBProgram : YTBGame
    {
        // Resources for drawing.
        private GraphicsDeviceManager graphicsDeviceManager;

        /// <summary>
        /// Indicates if the game is running on a mobile platform.
        /// </summary>
        public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

        /// <summary>
        /// Indicates if the game is running on a desktop platform.
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();


        //private Graphics3D pincel3d;
        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings, 
        /// initializes services like settings and leaderboard managers, and sets up the 
        /// screen manager for screen transitions.
        /// </summary>
        public YTBProgram()
        {


            //Model modelo3d = Content.Load<Model>("Models/Car");
            //Vector3 PosicionPlayer = new Vector3(0, 0, 0);


            ///// Camara por defecto
            ///// El primer parametro es la posici�n de la c�mara en el mundo 3D (0, 0, 10)
            ///// El segundo parametro es el punto al que la c�mara est� mirando (Vector3.Zero)
            ///// El tercer parametro es el vector "up" de la c�mara (Vector3.Up). Es decir, la direcci�n que se considera "arriba" para la c�mara.
            //Matrix puntoExactoDeRenderizado = Matrix.CreateLookAt(new Vector3(0, 0, 10), PosicionPlayer, Vector3.Up);

            //// Proyeccion: Perspective FOV
            /////Esto es elcampo de vision, mientras mas alto, mas abierto el campo de vision, como un modquito en 180 si es ToRadians(180) ve todo a su alrededor.
            /////El metodo ToRadians convierte grados a radianes, ya que la funcion CreatePerspectiveFieldOfView trabaja con radianes.
            //float fov = MathHelper.ToRadians(45f); // float (radianes)

            ///// Definir el aspect ratio (relacion de aspecto) de la pantalla
            //float aspect = GraphicsDevice.Viewport.AspectRatio; // float

            //// el tercer parametro es la distancia minima de renderizado, y el cuarto parametro es la distancia maxima de renderizado
            //Matrix projection = Matrix.CreatePerspectiveFieldOfView(fov, aspect, 0.1f, 1000f); // returns Matrix


            //// Se crea la camara
            //Camera camera = new Camera();

            ////Se le pasan las matrices a la camara
            //camera.ProjectionMatrix = projection;
            //camera.ViewMatrix = puntoExactoDeRenderizado;

            //foreach (ModelMesh mesh in modelo3d.Meshes)
            //{
            //    foreach (BasicEffect e in mesh.Effects)
            //    {
            //        /// Se le pasan los parametros de la camara al modelo 3D
            //        /// Esto es como si tuvieramos un spritebash y le pasamos la matriz de transformacion, pero en este caso, se lo pasamos siempre a cada uno de los modelos.

            //        /// Esto convierte las coordenadas del modelo 3D a las coordenadas del mundo 3D
            //        Matrix world = Matrix.CreateTranslation(PosicionPlayer);

            //        /// Se le pasa la coordenada donde se renderizara el modelo 3D (convertido a coordenadas de mundo 3d)
            //        e.World = world;

            //        /// Se le pasa la vista de la camara
            //        e.View = camera.ViewMatrix;

            //        /// Se le pasa la proyeccion de la camara
            //        e.Projection = camera.ProjectionMatrix;
            //        e.EnableDefaultLighting();
            //        e.PreferPerPixelLighting = true;
            //    }
            //    mesh.Draw();
            //}

            ///// Esta es la derecha relativa tomando la rotacion de la camara en cuenta
            //var derecha = camera.ViewMatrix.Right;

            //var izquierda = camera.ViewMatrix.Left;

            //var atras = camera.ViewMatrix.Backward;

            //var adelante = camera.ViewMatrix.Forward;

            //var arriba = camera.ViewMatrix.Up;

            //var abajo = camera.ViewMatrix.Down;



            //// para mover alpersonaje

            //var velocidad = 0.1f;

            ////Mover hacia delante, (segun la camara, siempre miraremos a la misma direccion)
            //PosicionPlayer += adelante * velocidad;


            //puntoExactoDeRenderizado = Matrix.CreateLookAt(new Vector3(0, 0, 10), PosicionPlayer, Vector3.Up);

            _graphics.PreferMultiSampling = true;
            graphicsDeviceManager.PreferMultiSampling = true;
            System.Console.WriteLine("[SandBoxGameGame] Constructor start");
            YTBGlobalState.EngineBackground = Color.White;
            YTBGlobalState.CameraZoom = 1f;
            int width = 1920;
            int height = 1080;
            // Disable fullscreen by default during debugging so the window is easier to see
            bool fullScreen = true;
            // Configurar las rutas del engine
            // En desarrollo, los assets f�sicos (.ytb, .cs, etc) est�n en Assets dentro de Platforms.Core
            YTBGlobalState.DevelopmentAssetsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "../", "SandBoxGame.Core",
                "Assets"
            );
            YTBGlobalState.ContentProjectPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "../", "SandBoxGame.Content",
                "SandBoxGame.Content.csproj"
                );

            SetScriptManager(new ScriptRegistry());
            SetModelRegistry(new ModelRegistry());

            AudioSystem.Initialize();
            AudioAssets.InitializeAudioAssets();
            // Los assets compilados (.xnb) est�n en Content (por defecto)
            // Esta ruta se calcula autom�ticamente: DirectorioSalida + "Content"
            YTBGlobalState.CompiledAssetsFolderName = "Content";

            System.Console.WriteLine("[SandBoxGameGame] GraphicsDeviceManager created");
            
            // Share GraphicsDeviceManager as a service.
            Window.Title = "SandBoxGame - Yotsuba Engine";
            // Configurar la ruta ra�z del Content Manager para que apunte a los assets compilados
            Content.RootDirectory = YTBGlobalState.CompiledAssetsFolderName;

            // Configure screen orientations.
            //graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            //pincel3d = new();

            System.Console.WriteLine("[SandBoxGameGame] InitializeGraphicsDevice called from ctor");
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

        protected override void SetConfig()
        {
            System.Console.WriteLine("[SandBoxGameGame] Constructor start");
            YTBGlobalState.EngineBackground = Color.White;
            YTBGlobalState.CameraZoom = 1f;
            int width = 1920;
            int height = 1080;

            bool fullScreen = false;

            // Configurar las rutas del engine
            // En desarrollo, los assets f�sicos (.ytb, .cs, etc) est�n en Assets dentro de Platforms.Core
            YTBGlobalState.DevelopmentAssetsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "../", "SandBoxGame.Core",
                "Assets"
            );

            //YTBGlobalState.ContentProjectPath = @"C:\\EnumaElish-A-Ciegas----Official.my\\YotsubaEngine\\SandBoxGame\\SandBoxGame.Content\\SandBoxGame.Content.csproj";

            SetScriptManager(new ScriptRegistry());
            SetModelRegistry(new ModelRegistry());

            AudioSystem.Initialize();
            AudioAssets.InitializeAudioAssets();

            // Los assets compilados (.xnb) est�n en Content (por defecto)
            // Esta ruta se calcula autom�ticamente: DirectorioSalida + "Content"
            YTBGlobalState.CompiledAssetsFolderName = "Content";

            graphicsDeviceManager = new GraphicsDeviceManager(this);

            System.Console.WriteLine("[SandBoxGameGame] GraphicsDeviceManager created");

            // Share GraphicsDeviceManager as a service.
            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);

            //Window.Title = "SandBoxGame - Yotsuba Engine";
            // Configurar la ruta ra�z del Content Manager para que apunte a los assets compilados
            Content.RootDirectory = YTBGlobalState.CompiledAssetsFolderName;


            // Configure screen orientations.
            graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            InitializeGraphicsDevice(graphicsDeviceManager, GraphicsDevice, width, height, fullScreen);

            // Precarga de texturas y fuentes generadas por el builder
            YotsubaGraphicsManager.InitializeAssets(AssetRegister.TextureAssets, AssetRegister.FontAssets);

            base.SetConfig("Yotsuba Engine");

            System.Console.WriteLine("[SandBoxGameGame] InitializeGraphicsDevice called from ctor");
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