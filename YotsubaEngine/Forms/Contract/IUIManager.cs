using Microsoft.Xna.Framework;

namespace YotsubaEngine.Forms.Contract
{
    public interface IUIManager
    {
        bool IsReady { get; }
        void Initialize();
        void Update(GameTime gameTime);
        /// <summary>
        /// Se llama ANTES de iterar los controles.
        /// Para ImGui: abre el frame (BeginLayout).
        /// Para Myra/Gum: no hace nada (retained mode).
        /// </summary>
        void BeginFrame(GameTime gameTime) { }
        /// <summary>
        /// Se llama DESPUES de iterar los controles.
        /// Para ImGui: cierra el frame (EndLayout que incluye Render).
        /// Para Myra: Desktop.Render().
        /// Para Gum: GumService.Default.Draw().
        /// </summary>
        void EndFrame(GameTime gameTime);
    }
}