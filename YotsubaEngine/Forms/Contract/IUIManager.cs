using Microsoft.Xna.Framework;

namespace YotsubaEngine.Forms.Contract
{
    public interface IUIManager
    {
        bool IsReady { get; }
        void Initialize();
        void Update(GameTime gameTime);
        /// <summary>
        /// Se llama ANTES de iterar los controles. Para ImGui: abre el frame (BeginLayout).
        /// </summary>
        void PreDraw(GameTime gameTime) { }
        /// <summary>
        /// Se llama DESPUES de iterar los controles. Para ImGui: cierra el frame (Render+EndLayout).
        /// </summary>
        void Draw(GameTime gameTime);
    }
}