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
    /// Contiene controles avanzados de ImGui para previsualización y selección de texturas, animaciones y componentes 2D.
    /// <para>Contains advanced ImGui controls for previewing and selecting textures, animations, and 2D components.</para>
    /// </summary>
    public static class ImGuiAdvancedControls
    {
        /// <summary>
        /// Preview de una TextureRegion (o porción de Texture2D) con alto fijo y ancho proporcional.
        /// <para>Previews a TextureRegion (or portion of a Texture2D) with fixed height and proportional width.</para>
        /// </summary>
        /// <param name="label">Etiqueta del control. <para>Control label.</para></param>
        /// <param name="region">Región de textura a mostrar. <para>Texture region to display.</para></param>
        /// <param name="renderer">Renderizador de ImGui. <para>ImGui renderer.</para></param>
        /// <param name="height">Alto deseado. <para>Desired height.</para></param>
        /// <param name="gameTime">Tiempo de juego opcional para animaciones. <para>Optional game time for animations.</para></param>
        /// <param name="onSelect">Acción al seleccionar. <para>Selection callback.</para></param>
        /// <param name="customImGuiRender">Renderizado extra de ImGui. <para>Extra ImGui rendering.</para></param>
        /// <returns>True si se seleccionó. <para>True if selected.</para></returns>
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
        /// <para>Previews a full Texture2D (without a source rectangle).</para>
        /// </summary>
        /// <param name="label">Etiqueta del control. <para>Control label.</para></param>
        /// <param name="texture">Textura completa. <para>Full texture.</para></param>
        /// <param name="renderer">Renderizador de ImGui. <para>ImGui renderer.</para></param>
        /// <param name="height">Alto deseado. <para>Desired height.</para></param>
        /// <param name="onSelect">Acción al seleccionar. <para>Selection callback.</para></param>
        /// <param name="customImGuiRender">Renderizado extra de ImGui. <para>Extra ImGui rendering.</para></param>
        /// <returns>True si se seleccionó. <para>True if selected.</para></returns>
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
        /// Preview de una Animation (usa su CurrentFrame) y avanza con GameTime.
        /// <para>Previews an Animation (using CurrentFrame) and advances with GameTime.</para>
        /// </summary>
        /// <param name="label">Etiqueta del control. <para>Control label.</para></param>
        /// <param name="animation">Animación a mostrar. <para>Animation to display.</para></param>
        /// <param name="renderer">Renderizador de ImGui. <para>ImGui renderer.</para></param>
        /// <param name="gameTime">Tiempo de juego para animación. <para>Game time for animation.</para></param>
        /// <param name="height">Alto deseado. <para>Desired height.</para></param>
        /// <param name="onSelect">Acción al seleccionar. <para>Selection callback.</para></param>
        /// <param name="customImGuiRender">Renderizado extra de ImGui. <para>Extra ImGui rendering.</para></param>
        /// <returns>True si se seleccionó. <para>True if selected.</para></returns>
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
        /// Preview helper para tu SpriteComponent2D.
        /// <para>Preview helper for your SpriteComponent2D.</para>
        /// </summary>
        /// <param name="label">Etiqueta del control. <para>Control label.</para></param>
        /// <param name="sprite">Componente de sprite. <para>Sprite component.</para></param>
        /// <param name="renderer">Renderizador de ImGui. <para>ImGui renderer.</para></param>
        /// <param name="height">Alto deseado. <para>Desired height.</para></param>
        /// <param name="onSelect">Acción al seleccionar. <para>Selection callback.</para></param>
        /// <param name="customImGuiRender">Renderizado extra de ImGui. <para>Extra ImGui rendering.</para></param>
        /// <returns>True si se seleccionó. <para>True if selected.</para></returns>
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
        /// Preview helper para tu AnimationComponent2D usando el AnimationType deseado.
        /// <para>Preview helper for your AnimationComponent2D using the desired AnimationType.</para>
        /// </summary>
        /// <param name="label">Etiqueta del control. <para>Control label.</para></param>
        /// <param name="animComponent">Componente de animación. <para>Animation component.</para></param>
        /// <param name="animationType">Tipo de animación. <para>Animation type.</para></param>
        /// <param name="renderer">Renderizador de ImGui. <para>ImGui renderer.</para></param>
        /// <param name="gameTime">Tiempo de juego para animación. <para>Game time for animation.</para></param>
        /// <param name="height">Alto deseado. <para>Desired height.</para></param>
        /// <param name="onSelect">Acción al seleccionar. <para>Selection callback.</para></param>
        /// <param name="customImGuiRender">Renderizado extra de ImGui. <para>Extra ImGui rendering.</para></param>
        /// <returns>True si se seleccionó. <para>True if selected.</para></returns>
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
