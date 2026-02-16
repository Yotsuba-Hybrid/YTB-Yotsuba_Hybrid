using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;

namespace SandBoxGame.Core.Systems
{
    /// <summary>
    /// Este es un sistema, y cada sistema nace y muere en cada escena, 
    /// en cada escena se crea una nueva instancia del sistema.
    /// </summary>
    public class CustomExampleSystem : IRenderSystem
    {
        public EntityManager EntityManager { get; private set; }

        /// <summary>
        /// Este es el primer metodo que se llama y se ejecuta una sola vez en todo el ciclo de vida del sistema.
        /// </summary>
        /// <param name="entities"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
        }

        /// <summary>
        /// Este es el segundo metodo que se llama, y se ejecuta una sola vez por cada entidad que 
        /// tenga la escena este activa en el momento.
        /// 
        /// En background sucede esto:
        /// 
        ///// foreach(var entity in EntityManager.YotsubaEntities)
        ///// {
        /////    CustomSystem.SharedEntityInitialize(entity);
        /////    other systems...
        ///// }
        /// 
        /// </summary>
        /// <param name="Entidad"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SharedEntityInitialize(Yotsuba Entidad)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Este metodo se ejecuta una vez por entidad en cada frame.
        /// </summary>
        /// <param name="Entidad"></param>
        /// <param name="time"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SharedEntityForEachUpdate(Yotsuba Entidad, GameTime time)
        {
            
        }

        /// <summary>
        /// Este metodo se ejecuta una sola vez por cada frame.
        /// Ideal para cosas que solo deben ser ejecutadas una vez por frame y no una vez por entidad.
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateSystem(GameTime gameTime)
        {
            

        }


        #region Los metodos de renderizado se ejecutan una vez por frame y despues de todos los demas metodos
        public void Render2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Render3D(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}


