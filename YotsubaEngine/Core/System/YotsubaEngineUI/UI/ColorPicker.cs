
using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection;
using Num = System.Numerics;

namespace YotsubaEngine.Core.System.YotsubaEngineUI.UI
{
    /// <summary>
    /// Proporciona ayudas de selector de color para ImGui.
    /// <para>Provides color picker helpers for ImGui.</para>
    /// </summary>
    public static class ColorPicker
    {
        // Renderiza un combo con todos los colores estáticos de Microsoft.Xna.Framework.Color
        /// <summary>
        /// Renderiza un combo con todos los colores estáticos de XNA.
        /// <para>Renders a combo box with all static XNA colors.</para>
        /// </summary>
        /// <param name="label">Etiqueta del combo. <para>Combo label.</para></param>
        /// <param name="selectedColor">Color seleccionado actual. <para>Current selected color.</para></param>
        /// <param name="onColorChanged">Callback al cambiar el color. <para>Callback when the color changes.</para></param>
        public static void RenderColorCombo(string label, ref Color selectedColor, Action<string> onColorChanged)
        {
            Color color = selectedColor;
            var colorProps = typeof(Color)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType == typeof(Color))
                .ToList();

            var colorNames = colorProps.Select(p => p.Name).ToArray();

            int currentIndex = colorProps.FindIndex(p => (Color)p.GetValue(null)! == color);
            if (currentIndex < 0) currentIndex = 0;

            string currentName = colorNames[currentIndex];

            if (ImGui.BeginCombo(label, currentName))
            {
                for (int i = 0; i < colorNames.Length; i++)
                {
                    bool isSelected = (i == currentIndex);

                    var c = (Color)colorProps[i].GetValue(null)!;
                    ImGui.ColorButton($"##color_preview_{i}", new Num.Vector4(c.R / 255f, c.G / 255f, c.B / 255f, 1f), ImGuiColorEditFlags.NoTooltip, new Num.Vector2(16, 16));
                    ImGui.SameLine();

                    if (ImGui.Selectable(colorNames[i], isSelected))
                    {
                        selectedColor = (Color)colorProps[i].GetValue(null)!;
                        onColorChanged?.Invoke(colorNames[i]);
                        currentIndex = i;
                    }

                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
        }

        /// <summary>
        /// Busca una propiedad Color por nombre cuando está disponible.
        /// <para>Finds a Color property by name when available.</para>
        /// </summary>
        /// <param name="colorNameOrValue">Nombre del color o valor. <para>Color name or value.</para></param>
        /// <returns>Propiedad encontrada o null. <para>Found property or null.</para></returns>
        public static PropertyInfo? ParseColorPropertyInfo(string colorNameOrValue)
        {
            if (string.IsNullOrWhiteSpace(colorNameOrValue)) return null;
            var prop = typeof(Color).GetProperty(colorNameOrValue, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            return prop;
        }

        /// <summary>
        /// Convierte un Color a su nombre estático más cercano.
        /// <para>Converts a Color to its nearest static name.</para>
        /// </summary>
        /// <param name="color">Color a convertir. <para>Color to convert.</para></param>
        /// <returns>Nombre del color o "White". <para>Color name or "White".</para></returns>
        public static string ColorToName(Color color)
        {
            var prop = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(p => (Color)p.GetValue(null)! == color);
            return prop?.Name ?? "White";
        }
    }


    /// <summary>
    /// Define la paleta de colores y la configuración de tema para ImGui.
    /// <para>Defines the color palette and theme settings for ImGui.</para>
    /// </summary>
    public static class ImGuiThemeColors
    {
        // 🎨 Paleta derivada del color base #F46A22 (naranja Yotsuba)
        /// <summary>
        /// Color primario base para el tema.
        /// <para>Base primary color for the theme.</para>
        /// </summary>
        public static readonly Num.Vector4 PRIMARY_BASE = HexToVec4("F46A22"); // Naranja principal

        /// <summary>
        /// Variante más clara del color primario.
        /// <para>Lighter variant of the primary color.</para>
        /// </summary>
        public static readonly Num.Vector4 PRIMARY_LIGHT = HexToVec4("FFA15C"); // Hover / resalte suave

        /// <summary>
        /// Variante más oscura del color primario.
        /// <para>Darker variant of the primary color.</para>
        /// </summary>
        public static readonly Num.Vector4 PRIMARY_DARK = HexToVec4("C6520F"); // Active / tono fuerte

        /// <summary>
        /// Color de acento cálido.
        /// <para>Warm accent color.</para>
        /// </summary>
        public static readonly Num.Vector4 ACCENT_WARM = HexToVec4("FF7E33"); // Variación cálida

        /// <summary>
        /// Color de acento frío.
        /// <para>Cool accent color.</para>
        /// </summary>
        public static readonly Num.Vector4 ACCENT_COOL = HexToVec4("1F6FFF"); // Azul contraste

        /// <summary>
        /// Color de estado de éxito.
        /// <para>Success state color.</para>
        /// </summary>
        public static readonly Num.Vector4 SUCCESS = HexToVec4("58C451"); // Verde éxito

        /// <summary>
        /// Color de estado de advertencia.
        /// <para>Warning state color.</para>
        /// </summary>
        public static readonly Num.Vector4 WARNING = HexToVec4("FFD166"); // Amarillo advertencia

        /// <summary>
        /// Color de estado de error.
        /// <para>Error state color.</para>
        /// </summary>
        public static readonly Num.Vector4 ERROR = HexToVec4("E63946"); // Rojo error

        // Tonos neutros
        /// <summary>
        /// Color gris oscuro del tema.
        /// <para>Dark gray theme color.</para>
        /// </summary>
        public static readonly Num.Vector4 GRIS_OSCURO = new(0.10f, 0.10f, 0.10f, 1.0f);

        /// <summary>
        /// Color gris medio del tema.
        /// <para>Medium gray theme color.</para>
        /// </summary>
        public static readonly Num.Vector4 GRIS_MEDIO = new(0.18f, 0.18f, 0.18f, 1.0f);

        /// <summary>
        /// Color gris claro del tema.
        /// <para>Light gray theme color.</para>
        /// </summary>
        public static readonly Num.Vector4 GRIS_CLARO = new(0.35f, 0.35f, 0.35f, 1.0f);

        /// <summary>
        /// Color blanco del tema.
        /// <para>White theme color.</para>
        /// </summary>
        public static readonly Num.Vector4 BLANCO = new(1.0f, 1.0f, 1.0f, 1.0f);

        // Helper: hex -> Vector4
        /// <summary>
        /// Converts a hex string into a Vector4 color.
        /// Convierte una cadena hex a un color Vector4.
        /// </summary>
        private static Num.Vector4 HexToVec4(string hex)
        {
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            if (hex.Length != 6) throw new ArgumentException("Hex debe tener 6 caracteres (ej: F46A22)");
            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);
            return new Num.Vector4(r / 255f, g / 255f, b / 255f, 1f);
        }

        // 🧩 Aplicación completa del tema
        /// <summary>
        /// Aplica la paleta completa del tema ImGui.
        /// <para>Applies the complete ImGui theme palette.</para>
        /// </summary>
        public static void AplicarTemaCompleto()
        {
            var style = ImGui.GetStyle();
            var colors = style.Colors;

            // Fondos
            colors[(int)ImGuiCol.WindowBg] = GRIS_OSCURO;
            colors[(int)ImGuiCol.FrameBg] = new Num.Vector4(GRIS_MEDIO.X, GRIS_MEDIO.Y, GRIS_MEDIO.Z, 0.95f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Num.Vector4(PRIMARY_LIGHT.X, PRIMARY_LIGHT.Y, PRIMARY_LIGHT.Z, 0.25f);
            colors[(int)ImGuiCol.FrameBgActive] = new Num.Vector4(PRIMARY_BASE.X, PRIMARY_BASE.Y, PRIMARY_BASE.Z, 0.35f);

            // Botones
            colors[(int)ImGuiCol.Button] = new Num.Vector4(GRIS_CLARO.X, GRIS_CLARO.Y, GRIS_CLARO.Z, 0.85f);
            colors[(int)ImGuiCol.ButtonHovered] = new Num.Vector4(PRIMARY_LIGHT.X, PRIMARY_LIGHT.Y, PRIMARY_LIGHT.Z, 0.95f);
            colors[(int)ImGuiCol.ButtonActive] = new Num.Vector4(PRIMARY_DARK.X, PRIMARY_DARK.Y, PRIMARY_DARK.Z, 1.0f);

            // Headers
            colors[(int)ImGuiCol.Header] = new Num.Vector4(GRIS_CLARO.X, GRIS_CLARO.Y, GRIS_CLARO.Z, 0.8f);
            colors[(int)ImGuiCol.HeaderHovered] = new Num.Vector4(PRIMARY_BASE.X, PRIMARY_BASE.Y, PRIMARY_BASE.Z, 0.95f);
            colors[(int)ImGuiCol.HeaderActive] = new Num.Vector4(PRIMARY_DARK.X, PRIMARY_DARK.Y, PRIMARY_DARK.Z, 1.0f);

            // Texto
            colors[(int)ImGuiCol.Text] = BLANCO;
            colors[(int)ImGuiCol.TextDisabled] = new Num.Vector4(GRIS_CLARO.X, GRIS_CLARO.Y, GRIS_CLARO.Z, 0.6f);

            // Bordes y separadores
            colors[(int)ImGuiCol.Border] = new Num.Vector4(PRIMARY_BASE.X, PRIMARY_BASE.Y, PRIMARY_BASE.Z, 0.25f);
            colors[(int)ImGuiCol.Separator] = new Num.Vector4(GRIS_CLARO.X, GRIS_CLARO.Y, GRIS_CLARO.Z, 0.2f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Num.Vector4(PRIMARY_LIGHT.X, PRIMARY_LIGHT.Y, PRIMARY_LIGHT.Z, 0.35f);

            // Tabs
            colors[(int)ImGuiCol.Tab] = new Num.Vector4(GRIS_MEDIO.X, GRIS_MEDIO.Y, GRIS_MEDIO.Z, 0.85f);
            colors[(int)ImGuiCol.TabHovered] = new Num.Vector4(PRIMARY_LIGHT.X, PRIMARY_LIGHT.Y, PRIMARY_LIGHT.Z, 0.95f);
            colors[(int)ImGuiCol.TabSelected] = new Num.Vector4(PRIMARY_BASE.X, PRIMARY_BASE.Y, PRIMARY_BASE.Z, 1.0f);
            //colors[(int)ImGuiCol.TabUnfocused] = new Num.Vector4(GRIS_MEDIO.X, GRIS_MEDIO.Y, GRIS_MEDIO.Z, 0.6f);
            //colors[(int)ImGuiCol.TabUnfocusedActive] = new Num.Vector4(PRIMARY_BASE.X, PRIMARY_BASE.Y, PRIMARY_BASE.Z, 0.7f);

            // Menús
            colors[(int)ImGuiCol.MenuBarBg] = new Num.Vector4(GRIS_MEDIO.X, GRIS_MEDIO.Y, GRIS_MEDIO.Z, 0.98f);
            colors[(int)ImGuiCol.Separator] = PRIMARY_BASE;

            // Scrollbars
            colors[(int)ImGuiCol.ScrollbarBg] = GRIS_OSCURO;
            colors[(int)ImGuiCol.ScrollbarGrab] = new Num.Vector4(GRIS_CLARO.X, GRIS_CLARO.Y, GRIS_CLARO.Z, 0.7f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = PRIMARY_LIGHT;
            colors[(int)ImGuiCol.ScrollbarGrabActive] = PRIMARY_BASE;

            colors[(int)ImGuiCol.TextDisabled] = PRIMARY_DARK;
            // Titles
            colors[(int)ImGuiCol.TitleBg] = GRIS_MEDIO;
            colors[(int)ImGuiCol.TitleBgActive] = PRIMARY_DARK;
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Num.Vector4(GRIS_MEDIO.X, GRIS_MEDIO.Y, GRIS_MEDIO.Z, 0.7f);

            // Popups / tooltips
            colors[(int)ImGuiCol.PopupBg] = new Num.Vector4(GRIS_OSCURO.X + 0.04f, GRIS_OSCURO.Y + 0.04f, GRIS_OSCURO.Z + 0.04f, 0.96f);

            // Estados (éxito, advertencia, error)
            colors[(int)ImGuiCol.DragDropTarget] = SUCCESS;
            colors[(int)ImGuiCol.CheckMark] = PRIMARY_LIGHT;
            colors[(int)ImGuiCol.SliderGrab] = PRIMARY_BASE;
            colors[(int)ImGuiCol.SliderGrabActive] = PRIMARY_DARK;

            // Configuración de estilo moderno
            style.WindowRounding = 8.0f;
            style.FrameRounding = 6.0f;
            style.ScrollbarRounding = 6.0f;
            style.GrabRounding = 5.0f;
            style.PopupRounding = 6.0f;

            style.ItemSpacing = new Num.Vector2(8, 6);
            style.ItemInnerSpacing = new Num.Vector2(6, 6);
            style.WindowPadding = new Num.Vector2(10, 10);
        }
    }

}
