using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using YotsubaEngine.YTB_Toolkit;


namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// Almacena la configuración global compartida del runtime del motor.
    /// <para>Stores shared global settings for the engine runtime.</para>
    /// </summary>
    public class YTBGlobalState 
    {


        /// <summary>
        /// Indica si el juego se está ejecutando en una plataforma móvil.
        /// <para>Indicates if the game is running on a mobile platform.</para>
        /// </summary>
        public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
        /// <summary>
        /// Indica si el juego se está ejecutando en una plataforma de escritorio.
        /// <para>Indicates if the game is running on a desktop platform.</para>
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

        /// <summary>
        /// Indica si el juego se está ejecutando en Windows.
        /// <para>Indicates if the game is running on Windows.</para>
        /// </summary>
        public readonly static bool IsWindows = OperatingSystem.IsWindows();

        /// <summary>
        /// Indica si el juego se está ejecutando en macOS.
        /// <para>Indicates if the game is running on macOS.</para>
        /// </summary>
        public readonly static bool IsMacOS = OperatingSystem.IsMacOS();

        /// <summary>
        /// Indica si el juego se está ejecutando en Linux.
        /// <para>Indicates if the game is running on Linux.</para>
        /// </summary>
        public readonly static bool IsLinux = OperatingSystem.IsLinux();

        /// <summary>
        /// Indica si el juego se está ejecutando en Android.
        /// <para>Indicates if the game is running on Android.</para>
        /// </summary>
        public readonly static bool IsAndroid = OperatingSystem.IsAndroid();

        /// <summary>
        /// Indica si el juego se está ejecutando en iOS.
        /// <para>Indicates if the game is running on iOS.</para>
        /// </summary>
        public readonly static bool IsIOS = OperatingSystem.IsIOS();

        /// <summary>
        /// Indica si el juego se está ejecutando en Mac Catalyst.
        /// <para>Indicates if the game is running on Mac Catalyst.</para>
        /// </summary>
        public readonly static bool IsMacCatalist = OperatingSystem.IsMacCatalyst();

        /// <summary>
        /// Indica si el juego se está ejecutando en tvOS.
        /// <para>Indicates if the game is running on tvOS.</para>
        /// </summary>
        public readonly static bool IsTvOS = OperatingSystem.IsTvOS();

        /// <summary>
        /// Indica si el juego se está ejecutando en watchOS.
        /// <para>Indicates if the game is running on watchOS.</para>
        /// </summary>
        public readonly static bool IsWatchOS = OperatingSystem.IsWatchOS();

        /// <summary>
        /// Zoom global de la cámara.
        /// <para>Global camera zoom.</para>
        /// </summary>
        public static float CameraZoom { get; set; } = 1f;

        /// <summary>
        /// Desplazamiento adicional de la cámara sumado a su posición.
        /// <para>Additional camera offset added to its position.</para>
        /// </summary>
        public static Vector2 OffsetCamera { get; set; } = Vector2.Zero;

        /// <summary>
        /// Ruta base donde se encuentran los assets físicos (.ytb) en desarrollo.
        /// <para>Base path for physical assets (.ytb) during development.</para>
        /// </summary>
        public static string DevelopmentAssetsPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");

        /// <summary>
        /// Ruta al proyecto de compilación de contenido (SandBoxGame.Content.csproj).
        /// <para>Path to the content build project (SandBoxGame.Content.csproj).</para>
        /// </summary>
        public static string ContentProjectPath { get; set; } = FindContentProjectPath();

        /// <summary>
        /// Carpeta donde se almacenan los assets compilados (.xnb).
        /// <para>Folder where compiled assets (.xnb) are stored.</para>
        /// </summary>
        public static string CompiledAssetsFolderName { get; set; } = "Content";

        /// <summary>
        /// Ruta completa a los assets compilados (.xnb).
        /// <para>Full path to compiled assets (.xnb).</para>
        /// </summary>
        public static string CompiledAssetsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CompiledAssetsFolderName);

        /// <summary>
        /// Intenta encontrar el proyecto de compilación de contenido de forma automática.
        /// Busca en la estructura estándar del motor: ../../../SandBoxGame/SandBoxGame.Content/SandBoxGame.Content.csproj
        /// </summary>
        private static string FindContentProjectPath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Intentar encontrar el proyecto de forma relativa (desde bin/Debug/net9.0/ -> ../../../SandBoxGame/SandBoxGame.Content/)
            string[] possiblePaths = new[]
            {
                Path.Combine(baseDir, "..", "..", "..", "SandBoxGame", "SandBoxGame.Content", "SandBoxGame.Content.csproj"),
                Path.Combine(baseDir, "..", "..", "SandBoxGame", "SandBoxGame.Content", "SandBoxGame.Content.csproj"),
                Path.Combine(baseDir, "..", "SandBoxGame", "SandBoxGame.Content", "SandBoxGame.Content.csproj"),
                Path.Combine(baseDir, "SandBoxGame.Content", "SandBoxGame.Content.csproj")
            };

            foreach (var path in possiblePaths)
            {
                string fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            // Si no se encuentra, devolver una ruta por defecto (esto generará un error más adelante, pero es intencional)
            return Path.Combine(baseDir, "SandBoxGame.Content", "SandBoxGame.Content.csproj");
        }

        /// <summary>
        /// Color del fondo del engine (configurable desde la UI).
        /// <para>Engine background color (configurable from the UI).</para>
        /// </summary>
        public static Color EngineBackground { get; set; } = Color.Turquoise;

        /// <summary>
        /// Instancia global del juego.
        /// <para>Global game instance.</para>
        /// </summary>
        public static YTBGame Game => (YTBGame)YTBGame.Instance;

        /// <summary>
        /// Acceso al control WASD global; requiere inicialización previa.
        /// <para>Access to the global WASD control; requires prior initialization.</para>
        /// </summary>
        public static WASDControl YTB_WASD_Movement => WASDControl.Instance;

        /// <summary>
        /// Llamadas a los helpers del engine.
        /// <para>Calls to engine helpers.</para>
        /// </summary>
        public static SystemCall SystemCall;

        /// <summary>
        /// Referencia global al ContentManager del juego.
        /// <para>Global reference to the game's ContentManager.</para>
        /// </summary>
        public static ContentManager ContentManager { get; set; }

        /// <summary>
        /// Referencia global al GraphicsDevice del juego.
        /// <para>Global reference to the game's GraphicsDevice.</para>
        /// </summary>
        public static GraphicsDevice GraphicsDevice { get; set; }

        /// <summary>
        /// Flag para habilitar/deshabilitar el engine en tiempo de ejecución.
        /// <para>Flag to enable/disable the engine at runtime.</para>
        /// </summary>
        public static bool EngineEnabled { get; set; } = true;


        /// <summary>
        /// Instancia global del generador de números aleatorios.
        /// <para>Global random number generator instance.</para>
        /// </summary>
        public static Random Random { get; set; } = new Random();

        /// <summary>
        /// Ultima escena activa antes de hacer el hotreload
        /// </summary>
        internal static string LastSceneNameBeforeUpdate { get; set; }

        /// <summary>
        /// Indica si los atajos del engine están activos (CapsLock activado).
        /// Cuando es true, los atajos del engine funcionan y los inputs del juego se desactivan.
        /// <para>Indicates whether engine shortcuts are active (CapsLock on).
        /// When true, engine shortcuts work and game inputs are disabled.</para>
        /// </summary>
        public static bool EngineShortcutsMode { get; set; } = false;

        public bool DisablePhisicSystem { get; set; } = false;
        public bool DisableRender2DSystem { get; set; } = false;
        public bool DisableFontSystem { get; set; } = false;
        public bool DisableRender3DSystem { get; set; } = false;
        public bool DisableAnimationSystem { get; set; } = false;
        public bool DisableTilemapSystem { get; set; } = false;
        public bool DisableButtonSystem { get; set; } = false;
        public bool DisableDebugSystem { get; set; } = false;

//-:cnd:noEmit
#if YTB
        /// <summary>
        /// Posición de la cámara libre del engine en el mundo 3D.
        /// <para>Engine free camera position in 3D world space.</para>
        /// </summary>
        internal static Vector3 FreeCameraPosition { get; set; } = Vector3.Zero;

        /// <summary>
        /// Yaw (rotación horizontal) de la cámara libre del engine en radianes.
        /// <para>Engine free camera yaw (horizontal rotation) in radians.</para>
        /// </summary>
        internal static float FreeCameraYaw { get; set; } = 0f;

        /// <summary>
        /// Pitch (rotación vertical) de la cámara libre del engine en radianes.
        /// <para>Engine free camera pitch (vertical rotation) in radians.</para>
        /// </summary>
        internal static float FreeCameraPitch { get; set; } = 0f;

        /// <summary>
        /// Indica si la cámara libre del engine se ha inicializado con la posición actual de la cámara del juego.
        /// <para>Indicates whether the engine free camera has been initialized from the game camera position.</para>
        /// </summary>
        internal static bool FreeCameraInitialized { get; set; } = false;

        /// <summary>
        /// IDs de las entidades 3D seleccionadas en modo engine. Vacío indica que no hay selección.
        /// <para>Selected 3D entity IDs in engine mode. Empty means no selection.</para>
        /// </summary>
        internal static HashSet<int> SelectedModel3DEntityIds { get; set; } = new();

#endif
//+:cnd:noEmit

    }
}
