using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// Clase encargada de gestionar las escenas del juego
    /// </summary>
    /// <param name="graphicsDeviceManager"></param>
    public class SceneManager(GraphicsDeviceManager graphicsDeviceManager)
    {

        /// <summary>
        /// Listado de las escenas del juego
        /// </summary>
        public YTB<Scene> Scenes { get; set; } = new();

        /// <summary>
        /// Referencia a la escena activa
        /// </summary>
        public Scene CurrentScene { get; set; } = new(graphicsDeviceManager);

    }
}
