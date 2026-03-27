using Benchmarks.Core.Types;

namespace Benchmarks.Core.Systems.Contract
{
    /// <summary>
    /// Sistema base para sistemas que renderizan.
    /// Sin dependencia a SpriteBatch - simula operaciones de render.
    /// </summary>
    public abstract class IRenderSystem : ISystem
    {
        public virtual void Render2D(GameTime gameTime) { }
        public virtual void Render3D(GameTime gameTime) { }
    }
}
