// =============================================================================
// GuiStyle.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Tema visual centralizado. Todos los colores y dimensiones métricas usadas
// por los controles se consultan desde aquí, evitando magic numbers dispersos.
//
// DECISIÓN DE DISEÑO – Struct vs Class
// ──────────────────────────────────────
// GuiStyle es un struct para que viva en el stack/inline dentro de GuiContext,
// sin indirección de heap. Al modificar el estilo se copia el struct entero
// (pass-by-value), lo que es intencional: cualquier override es transitorio
// a menos que el usuario lo almacene explícitamente.
// =============================================================================

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YotsubaEngine.Graphics.Gui;

/// <summary>
/// Tema visual completo para el sistema de UI inmediato.
/// Instancia predeterminada: <see cref="Default"/> (estilo oscuro tipo Dear ImGui).
/// </summary>
public struct GuiStyle
{
    // ─────────────────────────────────────────────────────────────────────────
    // COLORES – ventanas y fondos
    // ─────────────────────────────────────────────────────────────────────────
    public Color WindowBg;          // Fondo del panel/ventana
    public Color WindowBorder;      // Borde exterior de la ventana
    public Color TitleBarBg;        // Barra de título (estado normal)
    public Color TitleBarBgActive;  // Barra de título (ventana activa/dragging)
    public Color ChildBg;           // Fondo de paneles hijo

    // ─────────────────────────────────────────────────────────────────────────
    // COLORES – widgets generales
    // ─────────────────────────────────────────────────────────────────────────
    public Color WidgetBg;          // Fondo de campos (textInput, combobox)
    public Color WidgetBgHovered;   // Fondo al pasar el cursor
    public Color WidgetBgActive;    // Fondo al presionar / activo

    public Color ButtonBg;
    public Color ButtonBgHovered;
    public Color ButtonBgActive;

    public Color CheckMark;         // Marca del checkbox / radio button
    public Color SliderGrab;        // Manija del slider (estado normal)
    public Color SliderGrabActive;  // Manija del slider (arrastrando)

    public Color HeaderBg;          // Fondo de CollapsibleHeader (cerrado)
    public Color HeaderBgHovered;
    public Color HeaderBgActive;

    public Color ProgressBarBg;     // Fondo vacío de la barra de progreso
    public Color ProgressBarFill;   // Relleno de la barra de progreso

    // ─────────────────────────────────────────────────────────────────────────
    // COLORES – texto
    // ─────────────────────────────────────────────────────────────────────────
    public Color TextNormal;
    public Color TextDisabled;
    public Color TextSelected;      // Texto seleccionado en input fields

    // ─────────────────────────────────────────────────────────────────────────
    // COLORES – miscelánea
    // ─────────────────────────────────────────────────────────────────────────
    public Color Separator;         // Línea divisoria
    public Color ScrollbarBg;
    public Color ScrollbarGrab;
    public Color PopupBg;           // Fondo de dropdowns / popups

    // ─────────────────────────────────────────────────────────────────────────
    // MÉTRICAS DE DIMENSIÓN
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Altura de la barra de título de ventana (px).</summary>
    public float TitleBarHeight;

    /// <summary>Alto por defecto de un widget (botón, checkbox, etc.) (px).</summary>
    public float WidgetHeight;

    /// <summary>Relleno interior horizontal y vertical del widget (px).</summary>
    public float WidgetPaddingX;
    public float WidgetPaddingY;

    /// <summary>Separación vertical entre widgets consecutivos (px).</summary>
    public float ItemSpacing;

    /// <summary>Separación horizontal entre ítems en la misma línea (px).</summary>
    public float InnerSpacing;

    /// <summary>Padding interior de la ventana (distancia desde el borde al primer widget).</summary>
    public float WindowPaddingX;
    public float WindowPaddingY;

    /// <summary>Radio de esquinas redondeadas (0 = esquinas rectas).</summary>
    public float Rounding;

    /// <summary>Tamaño del cuadrado del checkbox/radio button (px).</summary>
    public float CheckBoxSize;

    /// <summary>Ancho de la barra de scroll (px).</summary>
    public float ScrollbarWidth;

    /// <summary>Altura mínima de la ventana (px).</summary>
    public float MinWindowHeight;

    // ─────────────────────────────────────────────────────────────────────────
    // PRESET PREDETERMINADO – estilo oscuro inspirado en Dear ImGui
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Tema oscuro por defecto. Crear una instancia de este preset es la forma
    /// recomendada de inicializar el sistema sin necesidad de configurar cada
    /// valor manualmente.
    /// </summary>
    public static GuiStyle Default => new()
    {
        // Ventanas
        WindowBg           = new Color(15,  15,  15,  240),
        WindowBorder       = new Color(110, 110, 128, 128),
        TitleBarBg         = new Color(41,  74,  122, 255),
        TitleBarBgActive   = new Color(26,  59,  105, 255),
        ChildBg            = new Color(0,   0,   0,   0),

        // Widgets
        WidgetBg           = new Color(41,  41,  41,  255),
        WidgetBgHovered    = new Color(66,  66,  66,  255),
        WidgetBgActive     = new Color(96,  96,  96,  255),

        ButtonBg           = new Color(66,  150, 250, 102),
        ButtonBgHovered    = new Color(66,  150, 250, 255),
        ButtonBgActive     = new Color(15,  135, 250, 255),

        CheckMark          = new Color(66,  150, 250, 255),
        SliderGrab         = new Color(61,  133, 224, 255),
        SliderGrabActive   = new Color(66,  150, 250, 255),

        HeaderBg           = new Color(41,  74,  122, 79),
        HeaderBgHovered    = new Color(66,  150, 250, 204),
        HeaderBgActive     = new Color(66,  150, 250, 255),

        ProgressBarBg      = new Color(41,  41,  41,  255),
        ProgressBarFill    = new Color(66,  150, 250, 255),

        // Texto
        TextNormal         = new Color(255, 255, 255, 255),
        TextDisabled       = new Color(128, 128, 128, 255),
        TextSelected       = new Color(66,  150, 250, 89),

        // Misc
        Separator          = new Color(110, 110, 128, 128),
        ScrollbarBg        = new Color(5,   5,   5,   135),
        ScrollbarGrab      = new Color(79,  79,  79,  255),
        PopupBg            = new Color(20,  20,  20,  240),

        // Métricas
        TitleBarHeight     = 20f,
        WidgetHeight       = 20f,
        WidgetPaddingX     = 8f,
        WidgetPaddingY     = 3f,
        ItemSpacing        = 4f,
        InnerSpacing       = 8f,
        WindowPaddingX     = 8f,
        WindowPaddingY     = 8f,
        Rounding           = 3f,
        CheckBoxSize       = 14f,
        ScrollbarWidth     = 12f,
        MinWindowHeight    = 40f,
    };

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS DE COLOR
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Retorna el color correcto del botón según su estado interactivo.
    /// Branchless-friendly: el JIT puede optimizar esto a una tabla de lookup.
    /// </summary>
    public readonly Color GetButtonColor(bool hovered, bool active)
    {
        if (active)  return ButtonBgActive;
        if (hovered) return ButtonBgHovered;
        return ButtonBg;
    }

    public readonly Color GetWidgetBg(bool hovered, bool active)
    {
        if (active)  return WidgetBgActive;
        if (hovered) return WidgetBgHovered;
        return WidgetBg;
    }

    public readonly Color GetHeaderColor(bool hovered, bool active)
    {
        if (active)  return HeaderBgActive;
        if (hovered) return HeaderBgHovered;
        return HeaderBg;
    }
}
