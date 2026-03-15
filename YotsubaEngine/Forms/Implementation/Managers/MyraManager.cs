using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;
using YotsubaEngine.Core.YotsubaGame;

namespace YotsubaEngine.Forms.Implementation.Managers
{
    internal class MyraManager : Contract.IUIManager
    {
        public static Desktop Desktop { get; private set; }
        private static bool _isInitialized;
        public bool IsReady => _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;

            var gameInstance = YTBGame.Instance;
            if (gameInstance == null)
            {
                System.Diagnostics.Debug.WriteLine("[MyraManager] Warning: YTBGame.Instance is null, skipping Myra initialization");
                return;
            }

            MyraEnvironment.Game = gameInstance;
            Desktop = new Desktop();
            _isInitialized = true;
            System.Diagnostics.Debug.WriteLine("[MyraManager] Initialized successfully");
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(GameTime gameTime)
        {
            if (!_isInitialized || Desktop == null) return;
            Desktop.Render();
        }
    }
}