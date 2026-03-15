using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Contract.Myra;
using YotsubaEngine.Forms.Contract.GumUI;
using YotsubaEngine.Forms.Contract.ImGUI;
using YotsubaEngine.Forms.Implementation;
using YotsubaEngine.Forms.Implementation.Managers;
using MyraUi = Myra.Graphics2D.UI;
using GumUi = Gum.Forms.Controls;
using MonoGameGum;

namespace YotsubaEngine.Forms
{
    public sealed class FormsManager
    {
        private static FormsManager _instance;
        private static readonly object _lock = new();

        private readonly Dictionary<UILibrary, IUIManager> _managers;
        private UILibrary _activeLibrary = UILibrary.GumUI;
        private readonly List<IForm> _registeredControls;
        private readonly List<IForm> _rootControls;
        private bool _isInitialized;

        public static FormsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new FormsManager();
                    }
                }
                return _instance;
            }
        }

        public UILibrary ActiveLibrary => _activeLibrary;
        public IReadOnlyList<IForm> RegisteredControls => _registeredControls.AsReadOnly();

        private FormsManager()
        {
            _managers = new Dictionary<UILibrary, IUIManager>();
            _registeredControls = new List<IForm>();
            _rootControls = new List<IForm>();
        }

        public void Initialize(UILibrary defaultLibrary = UILibrary.GumUI)
        {
            if (_isInitialized) return;

            Console.WriteLine($"[FormsManager] Initializing with library: {defaultLibrary}");
            System.Diagnostics.Debug.WriteLine($"[FormsManager] Initializing with library: {defaultLibrary}");

            _activeLibrary = defaultLibrary;

            _managers[UILibrary.Myra] = new MyraManager();
            _managers[UILibrary.GumUI] = new GumManager();
            _managers[UILibrary.ImGui] = new ImGuiManager();

            // Initialize ALL managers to ensure all libraries are ready
            foreach (var kvp in _managers)
            {
                Console.WriteLine($"[FormsManager] Initializing manager: {kvp.Key}");
                System.Diagnostics.Debug.WriteLine($"[FormsManager] Initializing manager: {kvp.Key}");
                kvp.Value.Initialize();
            }

            _isInitialized = true;
            Console.WriteLine("[FormsManager] Initialization complete");
            System.Diagnostics.Debug.WriteLine("[FormsManager] Initialization complete");
        }

        public void SetActiveLibrary(UILibrary library)
        {
            if (_activeLibrary == library) return;
            
            _activeLibrary = library;

            if (_managers.TryGetValue(library, out var manager))
            {
                manager.Initialize();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!_isInitialized) return;

            if (_managers.TryGetValue(_activeLibrary, out var manager) && manager.IsReady)
            {
                manager.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!_isInitialized) return;

            if (!_managers.TryGetValue(_activeLibrary, out var manager) || !manager.IsReady)
                return;

            // Para ImGui: abre el frame antes de dibujar controles
            manager.PreDraw(gameTime);

            // Render all controls added to root
            foreach (var control in _rootControls)
            {
                DrawControl(control);
            }

            // Para ImGui: cierra el frame. Para Gum/Myra: renderiza
            manager.Draw(gameTime);
        }

        private void DrawControl(IForm control)
        {
            switch (_activeLibrary)
            {
                case UILibrary.Myra:
                    if (control is IMyra myraControl)
                    {
                        myraControl.DrawMyra();
                    }
                    break;

                case UILibrary.GumUI:
                    if (control is IGum gumControl)
                    {
                        gumControl.DrawGumUI();
                    }
                    break;

                case UILibrary.ImGui:
                    if (control is IImGui imguiControl)
                    {
                        imguiControl.DrawImGuI();
                    }
                    break;
            }

            // Recursively draw children if it's a container
            if (control is IContainer container)
            {
                foreach (var child in container.Children)
                {
                    DrawControl(child);
                }
            }
        }

        public T CreateControl<T>() where T : IForm, new()
        {
            var control = new T();
            _registeredControls.Add(control);
            return control;
        }

        public void RegisterControl(IForm control)
        {
            if (!_registeredControls.Contains(control))
            {
                _registeredControls.Add(control);
            }
        }

        public void UnregisterControl(IForm control)
        {
            _registeredControls.Remove(control);
            _rootControls.Remove(control);
        }

        public void Clear()
        {
            _rootControls.Clear();
            _registeredControls.Clear();
        }

        public IUIManager GetManager(UILibrary library)
        {
            return _managers.TryGetValue(library, out var manager) ? manager : null;
        }

        public IUIManager GetCurrentManager()
        {
            return GetManager(_activeLibrary);
        }

        public void AddToRoot(IForm control)
        {
            if (control == null || !_isInitialized) return;

            _rootControls.Add(control);
            _registeredControls.Add(control);

            // Connect to the actual rendering system based on active library
            ConnectToLibrary(control);

            Console.WriteLine($"[FormsManager] Added control to root: {control.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"[FormsManager] Added control to root: {control.GetType().Name}");
        }

        private void ConnectToLibrary(IForm control)
        {
            switch (_activeLibrary)
            {
                case UILibrary.Myra:
                    ConnectToMyra(control);
                    break;

                case UILibrary.GumUI:
                    ConnectToGum(control);
                    break;

                case UILibrary.ImGui:
                    // ImGui doesn't need a root connection - controls draw themselves
                    break;
            }
        }

        private void ConnectToMyra(IForm control)
        {
            var desktop = MyraManager.Desktop;
            if (desktop == null)
            {
                Console.WriteLine("[FormsManager] Warning: Myra Desktop is null");
                System.Diagnostics.Debug.WriteLine("[FormsManager] Warning: Myra Desktop is null");
                return;
            }

            var myraWidget = GetMyraWidget(control);
            if (myraWidget != null)
            {
                desktop.Widgets.Add(myraWidget);
                Console.WriteLine($"[FormsManager] Added Myra widget to Desktop: {myraWidget.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[FormsManager] Added Myra widget to Desktop: {myraWidget.GetType().Name}");
            }
        }

        private void ConnectToGum(IForm control)
        {
            var root = GumService.Default.Root;
            if (root == null)
            {
                Console.WriteLine("[FormsManager] Warning: Gum Root is null");
                System.Diagnostics.Debug.WriteLine("[FormsManager] Warning: Gum Root is null");
                return;
            }

            var gumElement = GetGumElement(control);
            if (gumElement != null)
            {
                // FrameworkElement has a .Visual property that is GraphicalUiElement
                root.Children.Add(gumElement.Visual);
                Console.WriteLine($"[FormsManager] Added Gum element to Root: {gumElement.GetType().Name}");
                System.Diagnostics.Debug.WriteLine($"[FormsManager] Added Gum element to Root: {gumElement.GetType().Name}");
            }
        }

        public void RemoveFromRoot(IForm control)
        {
            if (control == null) return;

            _rootControls.Remove(control);

            // Disconnect from library rendering system
            switch (_activeLibrary)
            {
                case UILibrary.Myra:
                    RemoveFromMyra(control);
                    break;

                case UILibrary.GumUI:
                    RemoveFromGum(control);
                    break;
            }
        }

        private void RemoveFromMyra(IForm control)
        {
            var desktop = MyraManager.Desktop;
            if (desktop == null) return;

            var myraWidget = GetMyraWidget(control);
            if (myraWidget != null && desktop.Widgets.Contains(myraWidget))
            {
                desktop.Widgets.Remove(myraWidget);
            }
        }

        private void RemoveFromGum(IForm control)
        {
            var root = GumService.Default.Root;
            if (root == null) return;

            var gumElement = GetGumElement(control);
            if (gumElement != null && root.Children.Contains(gumElement.Visual))
            {
                root.Children.Remove(gumElement.Visual);
            }
        }

        private MyraUi.Widget GetMyraWidget(IForm control)
        {
            return control switch
            {
                IMyraButton btn => btn.GetMyraButton(),
                IMyraLabel lbl => lbl.GetMyraLabel(),
                IMyraCheckBox chk => chk.GetMyraCheckBox(),
                IMyraTextBox txt => txt.GetMyraTextBox(),
                IMyraSlider sld => sld.GetMyraSlider(),
                IMyraComboBox cmb => cmb.GetMyraComboBox(),
                IMyraWindow win => win.GetMyraWindow(),
                IMyraContainer cnt => cnt.GetMyraContainer(),
                _ => null
            };
        }

        private GumUi.FrameworkElement GetGumElement(IForm control)
        {
            return control switch
            {
                IGumButton btn => btn.GetGumButton(),
                IGumLabel lbl => lbl.GetGumLabel(),
                IGumCheckBox chk => chk.GetGumCheckBox(),
                IGumTextBox txt => txt.GetGumTextBox(),
                IGumSlider sld => sld.GetGumSlider(),
                IGumComboBox cmb => cmb.GetGumComboBox(),
                IGumWindow win => win.GetGumPanel(),
                IGumContainer cnt => cnt.GetGumContainer(),
                _ => null
            };
        }

        public static void Reset()
        {
            if (_instance != null)
            {
                _instance.Clear();
                _instance = null;
            }
        }
    }
}