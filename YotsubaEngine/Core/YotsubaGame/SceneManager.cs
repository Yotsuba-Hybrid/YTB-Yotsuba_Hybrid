using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.YotsubaGame
{
    /// <summary>
    /// Clase encargada de gestionar las escenas del juego.
    /// <para>Class responsible for managing game scenes.</para>
    /// </summary>
    /// <param name="graphicsDeviceManager">Administrador de gráficos para crear escenas. <para>Graphics device manager for scene creation.</para></param>
    public class SceneManager(GraphicsDeviceManager graphicsDeviceManager)
    {

        /// <summary>
        /// Listado de las escenas del juego.
        /// <para>List of game scenes.</para>
        /// </summary>
        public YTB<Scene> Scenes { get; set; } = new();

        /// <summary>
        /// Referencia a la escena activa.
        /// <para>Reference to the active scene.</para>
        /// </summary>
        public Scene CurrentScene { get; set; } = new(graphicsDeviceManager);

    }
}
