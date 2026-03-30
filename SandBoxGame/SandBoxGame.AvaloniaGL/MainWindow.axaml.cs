using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaInside.MonoGame;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using ReactiveUI;
using SandBoxGame.Core;
using System.Threading.Tasks;
using YotsubaEngine;
using YotsubaEngine.ActionFiles.YTB_Files;
using YotsubaEngine.Core.YotsubaGame;
using YotsubaEngine.Graphics.ImGuiNet;

namespace SandBoxGame.AvaloniaGL;

public partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; set; }

    public MainWindow()
    {

        Width = YTBProgram.WINDOW_WIDTH;
        Height = YTBProgram.WINDOW_HEIGHT;
        InitializeComponent();
        ViewModel = new(new YTBProgram(Platforms.Avalonia_GL));
        // Asignar el DataContext para que el enlace {Binding CurrentGame} de XAML funcione
        DataContext = ViewModel;
        // Mouse position, buttons and scroll — control-relative coordinates go directly to ImGui
        MyGameControl.PointerMoved += OnPointerMoved;
        MyGameControl.PointerPressed += OnPointerPressed;
        MyGameControl.PointerReleased += OnPointerReleased;
        MyGameControl.PointerWheelChanged += OnPointerWheelChanged;

        var mainContainer = this.FindControl<Border>("GameViewContainer");
        if (mainContainer != null)
        {
            mainContainer.SizeChanged += MainContainer_SizeChanged;
        }
        // Keyboard — window-level so keys arrive even when OS focus routing is imperfect
        this.KeyDown += OnKeyDown;
        this.KeyUp += OnKeyUp;
        this.TextInput += OnTextInput;
    }

    private void MainContainer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var myGameControl = this.FindControl<MonoGameControl>("MyGameControl");
        var scaleContainer = this.FindControl<LayoutTransformControl>("GameScaleContainer");

        if (myGameControl != null && scaleContainer != null)
        {
            // 1. Obtener el escalado real de Windows (Ej: 1.5 si tienes 150% de zoom)
            double scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;

            // 2. Multiplicar el tamaño lógico por la escala para obtener los PÍXELES FÍSICOS
            // Si el layout es 1280x720, le forzamos a pedir 1920x1080 a MonoGame
            myGameControl.Width = e.NewSize.Width * scaling;
            myGameControl.Height = e.NewSize.Height * scaling;

            // 3. Encoger el contenedor visualmente para contrarrestar el estiramiento de Avalonia
            // (1 / 1.5) = 0.6666. Esto lo hace encajar perfecto sin deformar el UI circundante.
            scaleContainer.LayoutTransform = new ScaleTransform(1 / scaling, 1 / scaling);
        }
    }
    // ── Mouse ────────────────────────────────────────────────────────────────

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var renderer = YTBGame.GuiRenderer;
        if (renderer == null) return;

        // Obtener la escala de la pantalla
        var scale = GetScaling();
        var pos = e.GetPosition(MyGameControl);

        // Multiplicar posición y tamaño por la escala física (DPI)
        renderer.AvaloniaMouseOverride = new System.Numerics.Vector2((float)(pos.X * scale), (float)(pos.Y * scale));
        renderer.AvaloniaControlSize = new System.Numerics.Vector2((float)(MyGameControl.Bounds.Width * scale), (float)(MyGameControl.Bounds.Height * scale));
        renderer.ForceActive = true;

        UpdateButtonState(e.GetCurrentPoint(MyGameControl).Properties, renderer);
    }
    private double GetScaling() => TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        MyGameControl.Focus();

        var renderer = YTBGame.GuiRenderer;
        if (renderer == null) return;

        var scale = GetScaling();
        var pos = e.GetPosition(MyGameControl);
        renderer.AvaloniaMouseOverride = new System.Numerics.Vector2((float)(pos.X * scale), (float)(pos.Y * scale));
        renderer.AvaloniaControlSize   = new System.Numerics.Vector2((float)(MyGameControl.Bounds.Width * scale), (float)(MyGameControl.Bounds.Height * scale));
        renderer.ForceActive = true;

        UpdateButtonState(e.GetCurrentPoint(MyGameControl).Properties, renderer);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var renderer = YTBGame.GuiRenderer;
        if (renderer == null) return;

        UpdateButtonState(e.GetCurrentPoint(MyGameControl).Properties, renderer);
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var renderer = YTBGame.GuiRenderer;
        if (renderer == null) return;
        renderer.InjectMouseWheel((float)e.Delta.X, (float)e.Delta.Y);
    }

    private static void UpdateButtonState(PointerPointProperties props, ImGuiRenderer renderer)
    {
        renderer.AvaloniaMouseLeft   = props.IsLeftButtonPressed;
        renderer.AvaloniaMouseRight  = props.IsRightButtonPressed;
        renderer.AvaloniaMouseMiddle = props.IsMiddleButtonPressed;
    }

    // ── Keyboard ─────────────────────────────────────────────────────────────

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        var renderer = YTBGame.GuiRenderer;
        if (renderer == null) return;
        if (TryMapKey(e.Key, out var imguiKey))
            renderer.InjectKeyEvent(imguiKey, true);
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        var renderer = YTBGame.GuiRenderer;
        if (renderer == null) return;
        if (TryMapKey(e.Key, out var imguiKey))
            renderer.InjectKeyEvent(imguiKey, false);
    }

    private void OnTextInput(object? sender, Avalonia.Input.TextInputEventArgs e)
    {
        var renderer = YTBGame.GuiRenderer;
        if (renderer == null || e.Text == null) return;
        foreach (var c in e.Text)
        {
            if (c == '\t') continue;
            renderer.InjectChar(c);
        }
    }

    private static bool TryMapKey(Key key, out ImGuiKey imguiKey)
    {
        imguiKey = key switch
        {
            Key.Back        => ImGuiKey.Backspace,
            Key.Tab         => ImGuiKey.Tab,
            Key.Enter       => ImGuiKey.Enter,
            Key.CapsLock    => ImGuiKey.CapsLock,
            Key.Escape      => ImGuiKey.Escape,
            Key.Space       => ImGuiKey.Space,
            Key.PageUp      => ImGuiKey.PageUp,
            Key.PageDown    => ImGuiKey.PageDown,
            Key.End         => ImGuiKey.End,
            Key.Home        => ImGuiKey.Home,
            Key.Left        => ImGuiKey.LeftArrow,
            Key.Right       => ImGuiKey.RightArrow,
            Key.Up          => ImGuiKey.UpArrow,
            Key.Down        => ImGuiKey.DownArrow,
            Key.PrintScreen => ImGuiKey.PrintScreen,
            Key.Insert      => ImGuiKey.Insert,
            Key.Delete      => ImGuiKey.Delete,
            >= Key.D0 and <= Key.D9 => ImGuiKey.Key0 + (key - Key.D0),
            >= Key.A  and <= Key.Z  => ImGuiKey.A    + (key - Key.A),
            >= Key.NumPad0 and <= Key.NumPad9 => ImGuiKey.Keypad0 + (key - Key.NumPad0),
            Key.Multiply    => ImGuiKey.KeypadMultiply,
            Key.Add         => ImGuiKey.KeypadAdd,
            Key.Subtract    => ImGuiKey.KeypadSubtract,
            Key.Decimal     => ImGuiKey.KeypadDecimal,
            Key.Divide      => ImGuiKey.KeypadDivide,
            >= Key.F1 and <= Key.F12 => ImGuiKey.F1 + (key - Key.F1),
            Key.NumLock     => ImGuiKey.NumLock,
            Key.Scroll      => ImGuiKey.ScrollLock,
            Key.LeftShift   or Key.RightShift => ImGuiKey.ModShift,
            Key.LeftCtrl    or Key.RightCtrl  => ImGuiKey.ModCtrl,
            Key.LeftAlt     or Key.RightAlt   => ImGuiKey.ModAlt,
            Key.OemSemicolon     => ImGuiKey.Semicolon,
            Key.OemPlus          => ImGuiKey.Equal,
            Key.OemComma         => ImGuiKey.Comma,
            Key.OemMinus         => ImGuiKey.Minus,
            Key.OemPeriod        => ImGuiKey.Period,
            Key.OemQuestion      => ImGuiKey.Slash,
            Key.OemTilde         => ImGuiKey.GraveAccent,
            Key.OemOpenBrackets  => ImGuiKey.LeftBracket,
            Key.OemCloseBrackets => ImGuiKey.RightBracket,
            Key.OemPipe          => ImGuiKey.Backslash,
            Key.OemQuotes        => ImGuiKey.Apostrophe,
            _ => ImGuiKey.None,
        };
        return imguiKey != ImGuiKey.None;
    }
}

public class MainViewModel : ViewModelBase
{
    public Game CurrentGame { get; set; }

    public MainViewModel(Game game)
    {
        CurrentGame = game;
    }
}

public class ViewModelBase : ReactiveObject
{
}
