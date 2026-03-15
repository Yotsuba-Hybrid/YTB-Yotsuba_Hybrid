using Microsoft.Xna.Framework;
using Gum.Forms.Controls;
using MonoGameGum;
using Gum.Forms;

namespace YotsubaEngine.Forms.Implementation.Managers
{
    internal class GumManager : Contract.IUIManager
    {
        private static bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;

            var gameInstance = YTBGame.Instance;
            if (gameInstance == null)
            {
                System.Diagnostics.Debug.WriteLine("[GumManager] Warning: YTBGame.Instance is null, skipping Gum initialization");
                return;
            }

            GumService.Default.Initialize(gameInstance, DefaultVisualsVersion.V3);

            FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);
            FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

            _isInitialized = true;
            System.Diagnostics.Debug.WriteLine("[GumManager] Initialized successfully");
        }

        public void Update(GameTime gameTime)
        {
            if (!_isInitialized) return;
            GumService.Default.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (!_isInitialized) return;
            GumService.Default.Draw();
        }
    }
}