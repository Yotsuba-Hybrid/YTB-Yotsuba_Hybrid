using SandBoxGame.Core;
using Foundation;
using UIKit;

namespace SandBoxGame.iOS
{
    [Register("AppDelegate")]
    internal class Program : UIApplicationDelegate
    {
        private static YTBProgram _game;

        /// <summary>
        /// Initializes and starts the game by creating an instance of the 
        /// Game class and calls its Run method.
        /// </summary>
        internal static void RunGame()
        {
            _game = new YTBProgram();
            _game.Run();
        }

        /// <summary>
        /// Se llama cuando la aplicación ha finalizado el lanzamiento y arranca el juego.
        /// <para>Called when the application has finished launching and starts the game.</para>
        /// </summary>
        /// <param name="app">
        /// Instancia de UIApplication que representa la aplicación.
        /// <para>The UIApplication instance representing the application.</para>
        /// </param>
        public override void FinishedLaunching(UIApplication app)
        {
            RunGame();
        }

        /// <summary>
        /// The main entry point for the application. 
        /// This sets up the application and specifies the UIApplicationDelegate 
        /// class to handle application lifecycle events.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(Program));
        }
    }
}
