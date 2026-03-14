using Myra;
using Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Implementation.Managers
{
    internal class MyraManager
    {
        public static Desktop Desktop;
        public MyraManager()
        {
            MyraEnvironment.Game = YTBGame.Instance;
            Desktop = new Desktop();
        }
    }
}
