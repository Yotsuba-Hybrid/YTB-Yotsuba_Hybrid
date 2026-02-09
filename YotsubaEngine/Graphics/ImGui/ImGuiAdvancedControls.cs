using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Numerics;
using YotsubaEngine.Core.Component.C_2D; // Para SpriteComponent2D / AnimationComponent2D
using YotsubaEngine.Graphics;
using YotsubaEngine.Graphics.ImGuiNet; // Para Animation / TextureRegion
using Num = System.Numerics;

namespace YotsubaEngine.Graphics.ImGuiNet
{

    /// <summary>
    /// Contiene controles avanzados de ImGui para previsualización y selección de Texturas, Animaciones y Componentes 2D.
    /// </summary>
    public static class ImGuiAdvancedControls
    {
        /// <summary>
        /// Preview de una TextureRegion (o porcion de Texture2D) con alto fijo y ancho proporcional.
        /// </summary>
        public static bool PreviewTextureRegion(
            string label,
            TextureRegion region,
            ImGuiRenderer renderer,
            float height = 96f,
            GameTime gameTime = null, // opcional para animaciones que llamen el CurrentFrame con GameTime
            Action onSelect = null,
            Action customImGuiRender = null)
        {
            if (region.Texture is null)
            {
                ImGui.Text($"{label} (sin textura)");
                if (customImGuiRender != null) customImGuiRender();
                if (onSelect != null && ImGui.Button($"Seleccionar {label}")) { onSelect(); return true; }
                return false;
            }

            Texture2D tex = region.Texture;
            Rectangle src = region.SourceRectangle;

            float aspect = src.Height > 0 ? (src.Width / (float)src.Height) : (tex.Width / (float)tex.Height);
            float width = height * aspect;

            ImGui.PushID(label);
            ImGui.Text(label);
            ImGui.SameLine();

            // Registrar la textura y obtener Id válido para ImGui
            IntPtr texId = renderer.BindTexture(tex);

            // Calcular UVs para la subregion (0..1)
            Num.Vector2 uv0 = new Num.Vector2(src.X / (float)tex.Width, src.Y / (float)tex.Height);
            Num.Vector2 uv1 = new Num.Vector2((src.X + src.Width) / (float)tex.Width, (src.Y + src.Height) / (float)tex.Height);

            // Render imagen
            ImGui.Image(texId, new Num.Vector2(width, height), uv0, uv1);

            // Espacio para render ImGui adicional
            if (customImGuiRender != null)
            {
                ImGui.Separator();
                customImGuiRender();
            }

            // Botón de selección debajo
            ImGui.Spacing();
            bool clicked = false;
            if (ImGui.Button($"Seleccionar##{label}"))
            {
                onSelect?.Invoke();
                clicked = true;
            }

            ImGui.PopID();
            return clicked;
        }

        /// <summary>
        /// Preview a partir de una Texture2D completa (sin sourceRect).
        /// </summary>
        public static bool PreviewTexture(
            string label,
            Texture2D texture,
            ImGuiRenderer renderer,
            float height = 96f,
            Action onSelect = null,
            Action customImGuiRender = null)
        {
            if (texture is null)
            {
                ImGui.Text($"{label} (sin textura)");
                if (customImGuiRender != null) customImGuiRender();
                if (onSelect != null && ImGui.Button($"Seleccionar {label}")) { onSelect(); return true; }
                return false;
            }

            var region = new TextureRegion(texture, new Rectangle(0, 0, texture.Width, texture.Height));
            return PreviewTextureRegion(label, region, renderer, height, null, onSelect, customImGuiRender);
        }

        /// <summary>
        /// Preview de una Animation (usa su CurrentFrame). Recibe GameTime para avanzar la animación.
        /// </summary>
        public static bool PreviewAnimation(
            string label,
            Animation animation,
            ImGuiRenderer renderer,
            GameTime gameTime,
            float height = 96f,
            Action onSelect = null,
            Action customImGuiRender = null)
        {
            if (animation is null)
            {
                ImGui.Text($"{label} (sin animación)");
                if (customImGuiRender != null) customImGuiRender();
                if (onSelect != null && ImGui.Button($"Seleccionar {label}")) { onSelect(); return true; }
                return false;
            }

            // Obtenemos el frame actual (se actualiza internamente usando gameTime)
            TextureRegion current = animation.CurrentFrame(gameTime ?? throw new ArgumentNullException(nameof(gameTime)));
            return PreviewTextureRegion(label, current, renderer, height, gameTime, onSelect, customImGuiRender);
        }

        /// <summary>
        /// Preview helper para tu SpriteComponent2D
        /// </summary>
        public static bool PreviewSpriteComponent(
            string label,
            SpriteComponent2D sprite,
            ImGuiRenderer renderer,
            float height = 96f,
            Action onSelect = null,
            Action customImGuiRender = null)
        {
            if (sprite.Texture is null)
            {
                ImGui.Text($"{label} (sin textura)");
                if (customImGuiRender != null) customImGuiRender();
                if (onSelect != null && ImGui.Button($"Seleccionar {label}")) { onSelect(); return true; }
                return false;
            }

            var region = new TextureRegion(sprite.Texture, sprite.SourceRectangle);
            return PreviewTextureRegion(label, region, renderer, height, null, onSelect, customImGuiRender);
        }

        /// <summary>
        /// Preview helper para tu AnimationComponent2D:
        /// se recibe el AnimationType que quieres mostrar (ej: AnimationType.idle).
        /// </summary>
        public static bool PreviewAnimationComponent(
            string label,
            AnimationComponent2D animComponent,
            AnimationType animationType,
            ImGuiRenderer renderer,
            GameTime gameTime,
            float height = 96f,
            Action onSelect = null,
            Action customImGuiRender = null)
        {
            // Intentamos obtener la animación (podrías querer añadir validación adicional en tu engine)
            Animation anim;
            try
            {
                anim = animComponent.GetAnimation(animationType);
            }
            catch (Exception)
            {
                ImGui.Text($"{label} (animación no encontrada)");
                if (customImGuiRender != null) customImGuiRender();
                if (onSelect != null && ImGui.Button($"Seleccionar {label}")) { onSelect(); return true; }
                return false;
            }

            return PreviewAnimation(label, anim, renderer, gameTime, height, onSelect, customImGuiRender);
        }
    }
}