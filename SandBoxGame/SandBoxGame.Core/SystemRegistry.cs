using Microsoft.Xna.Framework;
using SandBoxGame.Core.Systems;
using YotsubaEngine.Core.System.S_AGNOSTIC;
using YotsubaEngine.Core.YotsubaGame;

namespace SandBoxGame.Core
{
    public class SystemRegistry
    {
        public void LoadAllCustomSystems()
        {
            SystemBuilder.AddSystem<CustomExampleSystem>();
        }
    }
}
