
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    public struct ShaderComponent(Effect effect, bool IsActive)
    {
        public Effect Effect { get; set; } = effect;

        public bool IsActive { get; set; } = true;
    }
}
