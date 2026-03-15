using Microsoft.Xna.Framework;

namespace YotsubaEngine.Forms.Contract
{
    public interface IUIManager
    {
        void Initialize();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}