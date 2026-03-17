// =============================================================================
// Gui.cs  –  YotsubaEngine Immediate Mode GUI
// =============================================================================
// Fachada estática pública del sistema. Los usuarios llaman a Gui.Button(),
// Gui.Begin(), etc. sin gestionar el contexto directamente.
//
// DECISIÓN DE DISEÑO – API estática vs instanciada
// ─────────────────────────────────────────────────
// Una API estática con contexto [ThreadStatic] replica exactamente el patrón
// de Dear ImGui (ImGui::Button, ImGui::Begin) que los usuarios ya conocen.
// Ventajas:
//  • Sintaxis más limpia: if (Gui.Button("Start")) ...
//  • No requiere pasar el contexto por parámetro en cada widget.
//  • [ThreadStatic] garantiza thread-safety sin locks: cada hilo tiene su propio
//    contexto (útil para GUIs de herramientas multi-hilo).
//
// NOTA: Las clases parciales (partial class) se usan para distribuir los 10
// controles en archivos separados sin romper la cohesión de la clase Gui.
// =============================================================================

using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace YotsubaEngine.Graphics.Gui;

/// <summary>
/// API estática de Immediate Mode GUI para YotsubaEngine.
/// Todos los widgets son llamadas a métodos estáticos de esta clase.
/// </summary>
public static partial class Gui
{
    // ─────────────────────────────────────────────────────────────────────────
    // CONTEXTO ACTUAL
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Contexto activo para el hilo actual.
    /// [ThreadStatic] permite múltiples hilos con contextos independientes.
    /// </summary>
    [ThreadStatic]
    private static GuiContext? _context;

    /// <summary>Contexto activo. Lanza si no hay contexto asignado.</summary>
    public static GuiContext Context
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _context ?? ThrowNoContext();
    }

    /// <summary>
    /// Asigna el contexto que usarán todos los widgets en este hilo.
    /// Llamar una vez durante inicialización.
    /// </summary>
    public static void SetContext(GuiContext ctx)
        => _context = ctx ?? throw new ArgumentNullException(nameof(ctx));

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static GuiContext ThrowNoContext()
        => throw new InvalidOperationException(
            "No hay GuiContext activo. Llama a Gui.SetContext(ctx) antes de usar la UI.");

    // ─────────────────────────────────────────────────────────────────────────
    // FRAME LIFECYCLE
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Inicia un nuevo frame de UI. Llama antes de cualquier widget.
    /// </summary>
    public static void NewFrame(
        MouseState    mouse,
        KeyboardState keyboard,
        GameTime      gameTime,
        int           displayW,
        int           displayH)
        => Context.NewFrame(mouse, keyboard, gameTime, displayW, displayH);

    /// <summary>
    /// Finaliza el frame de UI. Llama después de todos los widgets y antes del render.
    /// </summary>
    public static void EndFrame()
        => Context.EndFrame();

    // ─────────────────────────────────────────────────────────────────────────
    // UTILIDADES DE LAYOUT
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Hace que el próximo widget se coloque en la misma línea que el anterior.
    /// </summary>
    public static void SameLine()
        => Context.SetNextSameLine();

    /// <summary>
    /// Inserta un separador horizontal.
    /// </summary>
    public static void Separator()
    {
        var ctx = Context;
        if (!ctx.HasActiveLayout) return;
        float x0 = ctx.CurrentLayout.Cursor.X;
        float x1 = ctx.CurrentLayout.ContentMaxX;
        float y  = ctx.CurrentLayout.Cursor.Y + ctx.Style.ItemSpacing;
        ctx.DrawList.AddHorizontalLine(x0, x1, y, ctx.Style.Separator);
        ctx.CurrentLayout.Cursor.Y = y + 1f + ctx.Style.ItemSpacing;
    }

    /// <summary>Añade espacio vertical vacío.</summary>
    public static void Spacing(float pixels = -1f)
    {
        var ctx = Context;
        if (!ctx.HasActiveLayout) return;
        float sp = pixels < 0f ? ctx.Style.ItemSpacing * 2f : pixels;
        ctx.CurrentLayout.Cursor.Y += sp;
    }

    /// <summary>Empuja un ID numérico al stack (útil dentro de loops for).</summary>
    public static void PushId(int id) => Context.PushId((uint)id);

    /// <summary>Saca el último ID del stack.</summary>
    public static void PopId() => Context.PopId();

    // ─────────────────────────────────────────────────────────────────────────
    // ACCESO AL DRAW LIST (para widgets avanzados o custom rendering)
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// DrawList del frame actual. Permite añadir geometría personalizada
    /// sin pasar por la capa de widgets estándar.
    /// </summary>
    public static DrawList DrawList => Context.DrawList;

    // ─────────────────────────────────────────────────────────────────────────
    // HELPERS INTERNOS COMPARTIDOS POR TODOS LOS CONTROLES
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Centra texto verticalmente dentro de un rectángulo.
    /// Devuelve la posición Y para que el texto quede centrado.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CenterTextY(Rectangle rect, float textHeight)
        => rect.Top + (rect.Height - textHeight) * 0.5f;

    /// <summary>
    /// Centra texto horizontalmente dentro de un rectángulo.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CenterTextX(Rectangle rect, float textWidth)
        => rect.Left + (rect.Width - textWidth) * 0.5f;

    /// <summary>
    /// Dibuja texto centrado vertical y horizontalmente en un rectángulo.
    /// </summary>
    private static void DrawCenteredText(
        GuiContext ctx, Rectangle rect, ReadOnlySpan<char> text, Color color)
    {
        if (ctx.Font is null) return;
        Vector2 size = ctx.MeasureText(text);
        float x = rect.Left + (rect.Width  - size.X) * 0.5f;
        float y = rect.Top  + (rect.Height - size.Y) * 0.5f;
        ctx.DrawList.AddText(new Vector2(x, y), text, color);
    }

    /// <summary>
    /// Dibuja texto alineado a la izquierda con padding estilo widget.
    /// </summary>
    private static void DrawLeftAlignedText(
        GuiContext ctx, Rectangle rect, ReadOnlySpan<char> text, Color color)
    {
        if (ctx.Font is null) return;
        Vector2 size = ctx.MeasureText(text);
        float y = rect.Top + (rect.Height - size.Y) * 0.5f;
        float x = rect.Left + ctx.Style.WidgetPaddingX;
        ctx.DrawList.AddText(new Vector2(x, y), text, color);
    }
}
