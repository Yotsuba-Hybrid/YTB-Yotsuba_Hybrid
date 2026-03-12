using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Core.Component.C_3D;
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
         
        public override void InitializeSystem(EntityManager entities)
        {
            EntityManager = entities;
        }

        
        public override void SharedEntityInitialize(ref Yotsuba Entidad)
        {

            YTBModelComponent3D component3D = new YTBModelComponent3D();
            component3D.Tag = "Box";
            component3D.IsVisible = true;
            EntityManager.AddYTBObject3D(Entidad, component3D);
        }

        public override void SharedEntityForEachUpdate(ref Yotsuba Entidad, GameTime time)
        {
            
        }

        public override void UpdateSystem(GameTime gameTime)
        {
            

        }


        #region Los metodos de renderizado se ejecutan una vez por frame y despues de todos los demas metodos
        public override void Render2D(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        public override void Render3D(GameTime gameTime)
        {
        }

        public override void Dispose()
        {
        }

        #endregion


    }
}


