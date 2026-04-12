using SandBoxGame.Core;
using System.Windows;

namespace MonoGame.WpfCore;

public partial class MainWindow : Window
{
    private YTBProgram _engine;
    public MainWindow()
    {
        InitializeComponent();

        _engine = new YTBProgram(YotsubaEngine.Core.YotsubaGame.Platforms.Windows_WPF_DX12);

        // Se lo asignas a tu control (asumiendo que en XAML se llama 'GameControl')
        GameControl.Game = _engine;
        Width = YTBProgram.WINDOW_WIDTH;
        Height = YTBProgram.WINDOW_HEIGHT;
    }
}
