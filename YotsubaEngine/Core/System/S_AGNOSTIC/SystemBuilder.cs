using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.HighestPerformanceTypes;

namespace YotsubaEngine.Core.System.S_AGNOSTIC
{
    public class SystemBuilder : IRenderSystem
    {
        /// <summary>
        /// Almacena todas las clases definidas por el usuario
        /// que heredan de ISystem que deben ser instanciadas como sistemas 
        /// en cada escena del juego.
        /// </summary>
        public static YTB<Func<ISystem>> Systems = new YTB<Func<ISystem>>();

        /// <summary>
        /// Almacena las instancias de las clases definidas por el usuario
        /// que heredan de ISystem para la escena actual del juego.
        /// </summary>
        public YTB<ISystem> SystemsInstances = new YTB<ISystem>();

        /// <summary>
        /// Método para anadir un sistema adicional definido por el usuario.
        /// Se ejecutara en todas las escenas.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void AddSystem<T>() where T : ISystem, new()
        {
            Systems.Add(() => (ISystem)new T()); 
        }

        public void InitializeSystem(EntityManager entities)
        {
            foreach (ref var system in Systems.AsSpan())
            {
                SystemsInstances.Add(system());
            }

            foreach (ref var system in SystemsInstances.AsSpan())
                system.InitializeSystem(entities);

        }

        public void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            foreach (ref var system in SystemsInstances.AsSpan())
                system.SharedEntityInitialize(ref Entidad);
        }

        public void UpdateSystem(GameTime gameTime)
        {
            foreach (ref var system in SystemsInstances.AsSpan())
                system.UpdateSystem(gameTime);
        }

        public void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
        {
            foreach (ref var system in SystemsInstances.AsSpan())
                system.SharedEntityForEachUpdate(ref Entidad, time);
        }

        public void Render2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (ref var s in SystemsInstances.AsSpan())
            {
                if (s is IRenderSystem system)
                    system.Render2D(spriteBatch, gameTime);
            }
        }

        public void Render3D(GameTime gameTime)
        {
            foreach (ref var s in SystemsInstances.AsSpan())
            {
                if (s is IRenderSystem system)
                    system.Render3D(gameTime);
            }
        }
    }
}
