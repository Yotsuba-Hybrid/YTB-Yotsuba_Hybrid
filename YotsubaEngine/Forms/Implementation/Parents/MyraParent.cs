using Myra;
using Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Implementation.Parents
{
    internal class MyraParent
    {
        public static Desktop Desktop;
        public MyraParent()
        {
            MyraEnvironment.Game = YTBGame.Instance;
            Desktop = new Desktop();
        }
    }
}
