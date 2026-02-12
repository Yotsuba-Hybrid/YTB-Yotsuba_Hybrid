using Gum.Forms;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YotsubaEngine.Attributes;
using YotsubaEngine.Audio;
using YotsubaEngine.Core.Entity;
using YotsubaEngine.Core.System.GumUI;
using YotsubaEngine.Core.System.YotsubaEngineCore;
using YotsubaEngine.Core.YotsubaGame.Scripting;
using YotsubaEngine.Events.YTBEvents;
using static YotsubaEngine.Audio.AudioAssets;

namespace SandBoxGame.Core.Scripts.Screens
{
    [Script]
    /// <summary>
    /// Script de la pantalla de inicio que gestiona la UI y la navegación.
    /// <para>Home screen script that manages UI and navigation.</para>
    /// </summary>
    public class HomeScreen : BaseScript, IKeyboardListener
    {

        private Panel _titleScreenButtonsPanel;
        private Panel _optionsPanel;
        private Button _optionsButton;
        private Button _optionsBackButton;
        /// <summary>
        /// Indica si la configuración de Gum ya fue realizada.
        /// <para>Indicates whether Gum configuration has already been performed.</para>
        /// </summary>
        public static bool WasConfigurated = false;
        /// <summary>
        /// Inicializa la pantalla de inicio, la UI y los eventos de entrada.
        /// <para>Initializes the home screen, UI, and input events.</para>
        /// </summary>
        public override void Initialize()
        {


            StopMusic();

            YTBGlobalState.EngineBackground = new Color(32, 40, 78, 255);
            base.Initialize();
            // initialize alfa from current transform color (normalized 0..1)
            var initial = GetTransformComponent().Color;
            alfa = initial.A / 255f;
            GetTransformComponent().Color = new(initial, alfa);
            if(!GetInputComponent().GamePad.ContainsKey(YotsubaEngine.Core.Component.C_AGNOSTIC.ActionEntityInput.Attack))
            GetInputComponent().GamePad.Add(YotsubaEngine.Core.Component.C_AGNOSTIC.ActionEntityInput.Attack, Microsoft.Xna.Framework.Input.Buttons.A);
            EventManager.Subscribe<OnGamePadEvent>(GamePadPressed);

            InitializeGum();
            CreateTitlePanel();
            CreateOptionsPanel();
        }

        private void CreateTitlePanel()
        {

            _optionsBackButton = new();
            // Create a container to hold all of our buttons
            _titleScreenButtonsPanel = new Panel();
            _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
            _titleScreenButtonsPanel.AddToRoot();

            var startButton = new Button();
            startButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            startButton.X = 50;
            startButton.Y = -12;
            startButton.Width = 70;
            startButton.Text = "Start";
            startButton.Click += HandleStartClicked;
            _titleScreenButtonsPanel.AddChild(startButton);

            _optionsButton = new Button();
            _optionsButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            _optionsButton.X = -50;
            _optionsButton.Y = -12;
            _optionsButton.Width = 70;
            _optionsButton.Text = "Options";
            _optionsButton.Click += HandleOptionsClicked;
            _titleScreenButtonsPanel.AddChild(_optionsButton);

            _optionsPanel = new Panel();
            startButton.IsFocused = true;
        }

        private void HandleOptionsClicked(object sender, EventArgs e)
        {

            // Set the title panel to be invisible.
            _titleScreenButtonsPanel.IsVisible = false;

            // Set the options panel to be visible.
            _optionsPanel.IsVisible = true;

            // Give the back button on the options panel focus.
            _optionsBackButton.IsFocused = true;
            PlaySound("ui");

        }

        private void HandleStartClicked(object sender, EventArgs e)
        {
            ChangeScene("Game");
            PlaySound("ui");
        }


        private void CreateOptionsPanel()
        {
            _optionsPanel = new Panel();
            _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
            _optionsPanel.IsVisible = false;
            _optionsPanel.AddToRoot();

            var optionsText = new TextRuntime();
            optionsText.X = 10;
            optionsText.Y = 10;
            optionsText.Text = "OPTIONS";
            _optionsPanel.AddChild(optionsText);

            var musicSlider = new Slider();
            musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
            musicSlider.Y = 30f;
            musicSlider.Minimum = 0;
            musicSlider.Maximum = 1;
            musicSlider.Value = AudioSystem.MasterVolume;
            musicSlider.SmallChange = .1;
            musicSlider.LargeChange = .2;
            musicSlider.ValueChanged += HandleMusicSliderValueChanged;
            musicSlider.ValueChangeCompleted += MusicSlider_ValueChangeCompleted;
            _optionsPanel.AddChild(musicSlider);

            var sfxSlider = new Slider();
            sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
            sfxSlider.Y = 93;
            sfxSlider.Minimum = 0;
            sfxSlider.Maximum = 1;
            sfxSlider.Value = AudioSystem.SfxVolume;
            sfxSlider.SmallChange = .1;
            sfxSlider.LargeChange = .2;
            sfxSlider.ValueChanged += HandleSfxSliderChanged;
            sfxSlider.ValueChangeCompleted += (e,s) => {
                PlaySound("ui");
            };
            _optionsPanel.AddChild(sfxSlider);

            _optionsBackButton = new Button();
            _optionsBackButton.Text = "BACK";
            _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            _optionsBackButton.X = -28f;
            _optionsBackButton.Y = -10f;
            _optionsBackButton.Click += HandleOptionsButtonBack;
            _optionsPanel.AddChild(_optionsBackButton);
        }

        private void MusicSlider_ValueChangeCompleted(object sender, EventArgs e)
        {
            PlayMusic("theme");
            Task.WhenAll(
                Task.Delay(1000)
            ).ContinueWith(_ => {
                PauseMusic();
            });
        }

        private void HandleOptionsButtonBack(object sender, EventArgs e)
        {
            // Set the title panel to be invisible.
            _titleScreenButtonsPanel.IsVisible = true;

            // Set the options panel to be visible.
            _optionsPanel.IsVisible = false;

            // Give the back button on the options panel focus.
            _optionsBackButton.IsFocused = false;

            PlaySound("ui");

        }

        private void HandleSfxSliderChanged(object sender, EventArgs e)
        {
            AudioSystem.SfxVolume = (float)(sender as Slider).Value;
        }

        private void HandleMusicSliderValueChanged(object sender, EventArgs e)
        {
            AudioSystem.MusicVolume = (float)(sender as Slider).Value;
        }

        private void InitializeGum()
        {
            if (WasConfigurated) return;
            WasConfigurated = true;
            //// Initialize the Gum service. The second parameter specifies
            //// the version of the default visuals to use. V2 is the latest
            //// version.
            //GumService.Default.Initialize(YTBGlobalState.Game, DefaultVisualsVersion.V3);

            //// Tell the Gum service which content manager to use.  We will tell it to
            //// use the global content manager from our Core.
            //GumService.Default.ContentLoader.XnaContentManager = YTBGlobalState.ContentManager;

            // Register keyboard input for UI control.
            FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

            // Register gamepad input for Ui control.
            FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

            // Customize the tab reverse UI navigation to also trigger when the keyboard
            // Up arrow key is pushed.
            FrameworkElement.TabReverseKeyCombos.Add(
               new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Up });

            // Customize the tab UI navigation to also trigger when the keyboard
            // Down arrow key is pushed.
            FrameworkElement.TabKeyCombos.Add(
               new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });

            // The assets created for the UI were done so at 1/4th the size to keep the size of the
            // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
            // the game's resolution then tell gum to zoom in by a factor of 4.
            GumService.Default.CanvasWidth = YTBGlobalState.GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
            GumService.Default.CanvasHeight = YTBGlobalState.GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
            GumService.Default.Renderer.Camera.Zoom = 4.0f;
        }


        /// <summary>
        /// Maneja la entrada del gamepad en la pantalla de inicio.
        /// <para>Handles gamepad input on the home screen.</para>
        /// </summary>
        /// <param name="@event">
        /// Evento de gamepad recibido.
        /// <para>Gamepad event received.</para>
        /// </param>
        public void GamePadPressed(OnGamePadEvent @event)
        {
            if (@event.Button == Microsoft.Xna.Framework.Input.Buttons.A)
            {
                ChangeScene("Game");

            }
        }

        bool Ascending = false;
        float alfa = 0f; // normalized alpha in range [0,1]
        /// <summary>
        /// Actualiza el estado de la pantalla de inicio cada cuadro.
        /// <para>Updates the home screen state each frame.</para>
        /// </summary>
        /// <param name="gametime">
        /// Tiempo de juego actual.
        /// <para>Current game time.</para>
        /// </param>
        public override void Update(GameTime gametime)
        {
            TextoParpadeante();

            base.Update(gametime);
        }

        void TextoParpadeante()
        {
            ref var transform = ref GetTransformComponent();
            // Update normalized alpha and apply to color. Compare against `alfa` (0..1)
            if (Ascending)
            {
                alfa += 0.01f;
                alfa = MathHelper.Clamp(alfa, 0f, 1f);
                transform.Color = new(transform.Color, alfa);
                if (alfa >= 1f)
                {
                    Ascending = false;
                }
            }
            else
            {
                alfa -= 0.01f;
                alfa = MathHelper.Clamp(alfa, 0f, 1f);
                transform.Color = new(transform.Color, alfa);
                if (alfa <= 0f)
                {
                    Ascending = true;
                }
            }
        }

        /// <summary>
        /// Maneja la entrada de teclado en la pantalla de inicio.
        /// <para>Handles keyboard input on the home screen.</para>
        /// </summary>
        /// <param name="@event">
        /// Evento de teclado recibido.
        /// <para>Keyboard event received.</para>
        /// </param>
        public void OnKeyboardInput(OnKeyBoardEvent @event)
        {
            ChangeScene("Game");
        }
    }
}
