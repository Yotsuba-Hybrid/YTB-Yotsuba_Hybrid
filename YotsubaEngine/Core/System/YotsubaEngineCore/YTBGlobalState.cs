using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using YotsubaEngine.YTB_Toolkit;


namespace YotsubaEngine.Core.System.YotsubaEngineCore
{
    /// <summary>
    /// Stores shared global settings for the engine runtime.
    /// Almacena la configuración global compartida del runtime del motor.
    /// </summary>
    public class YTBGlobalState 
    {
        /// <summary>
        /// Zoom Global de la camara
        /// </summary>
        public static float CameraZoom { get; set; } = 1f;

        /// <summary>
        /// Desplazamiento adicional de la camara sumado al desplazamiento de la misma
        /// </summary>
        public static Vector2 OffsetCamera { get; set; } = Vector2.Zero;

        /// <summary>
        /// Ruta base donde se encuentran los assets físicos (.ytb) en desarrollo.
        /// Esta ruta es configurable y debe apuntar a la carpeta de desarrollo del juego.
        /// Por defecto apunta al directorio actual, pero debe ser cambiado en la implementación.
        /// </summary>
        public static string DevelopmentAssetsPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");

        /// <summary>
        /// Ruta al proyecto de compilación de contenido (SandBoxGame.Content.csproj).
        /// Esta ruta debe apuntar al proyecto que compila los assets del juego.
        /// Por defecto intenta encontrarlo de forma relativa a la base del motor.
        /// </summary>
        public static string ContentProjectPath { get; set; } = FindContentProjectPath();

        /// <summary>
        /// Carpeta donde se almacenan los assets compilados (.xnb).
        /// Esta carpeta se concatena automáticamente con la salida del proyecto.
        /// Por defecto es "Content" y normalmente no debe cambiarse.
        /// </summary>
        public static string CompiledAssetsFolderName { get; set; } = "Content";

        /// <summary>
        /// Ruta completa a los assets compilados (.xnb).
        /// Se construye automáticamente como: DirectorioSalida + CompiledAssetsFolderName
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
        /// Color del fondo del engine (configurable desde la UI)
        /// </summary>
        public static Color EngineBackground { get; set; } = Color.Turquoise;

        /// <summary>
        /// Intancia global del juego.
        /// </summary>
        public static YTBGame Game => (YTBGame)YTBGame.Instance;

        /// <summary>
        /// Ojo, necesita ser inicializado previamente.
        /// </summary>
        public static WASDControl YTB_WASD_Movement => WASDControl.Instance;

        /// <summary>
        /// LLamadas al los helpers del Engine
        /// </summary>
        public static SystemCall SystemCall;

        /// <summary>
        /// Referencia global al ContentManager del juego.
        /// </summary>
        public static ContentManager ContentManager { get; set; }

        /// <summary>
        /// Referencia global al GraphicsDevice del juego.
        /// </summary>
        public static GraphicsDevice GraphicsDevice { get; set; }

        /// <summary>
        /// Flag para habilitar/deshabilitar el engine en tiempo de ejecución (puede controlarse desde CI o platform checks).
        /// </summary>
        public static bool EngineEnabled { get; set; } = true;


        /// <summary>
        /// Instancia global del generador de números aleatorios.
        /// </summary>
        public static Random Random { get; set; } = new Random();

        /// <summary>
        /// Ultima escena activa antes de hacer el hotreload
        /// </summary>
        internal static string LastSceneNameBeforeUpdate { get; set; }

    }
}
