using SandBoxGame.Core.Systems;
using YotsubaEngine.Core.System.S_AGNOSTIC;


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
