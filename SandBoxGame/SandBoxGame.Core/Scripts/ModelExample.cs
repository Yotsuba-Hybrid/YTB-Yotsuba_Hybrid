using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YotsubaEngine.Attributes;
using YotsubaEngine.Core.Component.C_3D;
using YotsubaEngine.Core.YotsubaGame.Scripting;

namespace SandBoxGame.Core.Scripts
{
    [Script]
    public class ModelExample : BaseScript
    {
        public override void Initialize()
        {
            base.Initialize();
            if (Entity.HasNotComponent(YotsubaEngine.Core.Entity.YTBComponent.Model3D))
            {
                throw new Exception("La entidad no tiene un modelo 3d");
            }

            ModelComponent3D modelComponent3D = EntityManager.ModelComponents3D[Entity.Id];

            Model model = modelComponent3D.Model;

            foreach(var bone in model.Bones)
            {
                SendLog(bone.Name, Color.White);
            }
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }
    }
}
