#if YTB
using System.Linq;
using Hexa.NET.ImGui;
using YotsubaEngine.Core.System.YotsubaEngineUI.UI;

namespace YotsubaEngine.Core.Component.C_AGNOSTIC
{
    public partial struct TransformComponent
    {
        /// <summary>
        /// Controles extra del TransformComponent, invocados por <c>EntityManagerUI</c> al final del render
        /// (después de las propiedades [UIComponentValue]). Aquí vive el botón para sincronizar
        /// <c>Size</c> con el <c>SourceRectangle</c> del <c>SpriteComponent2D</c> hermano.
        /// </summary>
        public static void RenderExtraControls(IUIRenderContext ctx)
        {
            if (ImGui.Button("Sincronizar Size con SourceRectangle del Sprite"))
            {
                SyncSizeWithSprite(ctx);
            }
        }

        private static void SyncSizeWithSprite(IUIRenderContext ctx)
        {
            var entity = ctx.CurrentEntity;
            if (entity == null) return;

            var spriteComp = entity.Components.FirstOrDefault(c => c.ComponentName == "SpriteComponent2D");
            if (spriteComp == null) return;

            var sourceRectProp = spriteComp.Propiedades.FirstOrDefault(p => p.Item1 == "SourceRectangle");
            if (sourceRectProp == null) return;

            string sourceRect = sourceRectProp.Item2;
            if (string.IsNullOrWhiteSpace(sourceRect)) return;

            string[] parts = sourceRect.Split(',');
            if (parts.Length < 4) return;

            if (int.TryParse(parts[2].Trim(), out int width) && int.TryParse(parts[3].Trim(), out int height))
            {
                ctx.UpdateProperty("Size", $"{width},{height},0");
            }
        }
    }
}
#endif
