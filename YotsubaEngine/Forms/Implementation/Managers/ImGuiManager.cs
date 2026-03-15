using Microsoft.Xna.Framework;
using YotsubaEngine.Graphics.ImGuiNet;
using ImGuiNET;

namespace YotsubaEngine.Forms.Implementation.Managers
{
    internal class ImGuiManager : Contract.IUIManager
    {
        private static bool _isInitialized;
        private static ImGuiRenderer _renderer;

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
            if (!_isInitialized || _renderer == null) return;
            _renderer.BeginLayout(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (!_isInitialized || _renderer == null) return;
            ImGui.Render();
            _renderer.EndLayout();
        }
    }
}