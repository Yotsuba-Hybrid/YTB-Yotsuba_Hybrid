using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.Component.C_AGNOSTIC;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.Contract;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Graphics;

namespace SandBoxGame.Core.Systems
{
    /// <summary>
    /// Este es un sistema, y cada sistema nace y muere en cada escena, 
    /// en cada escena se crea una nueva instancia del sistema.
    /// </summary>
    public class CustomExampleSystem : IRenderSystem
    {
        public EntityManager EntityManager { get; private set; }
        private Graphics3D _graphics3D = null!;
         
        /// <summary>
        /// Este es el primer metodo que se llama y se ejecuta una sola vez en todo el ciclo de vida del sistema.
        /// </summary>
        /// <param name="entities"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
            _graphics3D = new Graphics3D();
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
        public void SharedEntityInitialize(ref Yotsuba Entidad)
        {
            

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Este metodo se ejecuta una vez por entidad en cada frame.
        /// </summary>
        /// <param name="Entidad"></param>
        /// <param name="time"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
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
            //throw new NotImplementedException();
        }

        public void Render3D(GameTime gameTime)
        {
            CameraComponent3D camera = EntityManager.Camera;
            if (camera is null) return;

            camera.Update();

            System.Span<Yotsuba> entities = EntityManager.YotsubaEntities.AsSpan();
            System.Span<TransformComponent> transforms = EntityManager.TransformComponents.AsSpan();

            foreach (ref Yotsuba entity in entities)
            {
                if (entity.HasNotComponent(YTBComponent.Transform)) continue;

                ref TransformComponent transform = ref transforms[entity.Id];
                _graphics3D.DrawBox(
                    transform.Position,
                    transform.Size,
                    transform.Color,
                    camera.ViewMatrix,
                    camera.ProjectionMatrix);
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion


    }
}


