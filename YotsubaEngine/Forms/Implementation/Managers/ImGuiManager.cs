using Microsoft.Xna.Framework;
using YotsubaEngine.Graphics.ImGuiNet;

namespace YotsubaEngine.Forms.Implementation.Managers
{
    internal class ImGuiManager : Contract.IUIManager
    {
        private static bool _isInitialized;
        private static ImGuiRenderer _renderer;
        public bool IsReady => _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;

            _renderer = YTBGame.GuiRenderer;

            if (_renderer == null)
            {
                System.Diagnostics.Debug.WriteLine("[ImGuiManager] Warning: YTBGame.GuiRenderer is null, ImGui will not be available");
                return;
            }

            _isInitialized = true;
            System.Diagnostics.Debug.WriteLine("[ImGuiManager] Initialized successfully");
        }

        public void Update(GameTime gameTime)
        {
        }

        public void BeginFrame(GameTime gameTime)
        {
            if (!_isInitialized || _renderer == null) return;
            _renderer.BeginLayout(gameTime);
        }

        public void EndFrame(GameTime gameTime)
        {
            if (!_isInitialized || _renderer == null) return;
            _renderer.EndLayout();
        }
    }
}