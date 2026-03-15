using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Contract.GumUI;
using YotsubaEngine.Forms.Contract.ImGUI;
using YotsubaEngine.Forms.Contract.Myra;
using GumUi = Gum.Forms.Controls;
using ImUi = ImGuiNET.ImGui;
using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Implementation
{
    public class Window : ContainerBase, IWindow, IMyraWindow, IGumWindow, IImGuiContainer
    {
        private string _text = "Window";
        private bool _isOpen = true;
        private readonly List<Action> _imguiChildActions = new();

        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
                MyraControl.Title = value;
                GumControl.Name = value;
            }
        }

        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    if (!_isOpen)
                    {
                        OnClosed?.Invoke();
                    }
                }
            }
        }

        public override Color Color { get; set; }
        public override Vector2 Position { get; set; }

        public event Action OnClosed;

        private MyraUi.Window MyraControl { get; set; }
        private GumUi.Panel GumControl { get; set; }

        public Window()
        {
            MyraControl = new MyraUi.Window
            {
                Title = _text,
                Tag = _text,
                Content = new MyraUi.Panel()
            };
            MyraControl.Closed += (_, _) =>
            {
                _isOpen = false;
                OnClosed?.Invoke();
            };

            GumControl = new GumUi.Panel
            {
                Name = _text,
                Width = 400,
                Height = 300
            };
        }

        void IGum.DrawGumUI()
        {
            GumControl.X = Position.X;
            GumControl.Y = Position.Y;
            GumControl.UpdateState();
        }

        void IImGui.DrawImGuI()
        {
            if (!_isOpen) return;

            bool open = true;
            if (ImUi.Begin(_text, ref open))
            {
                foreach (var childAction in _imguiChildActions)
                {
                    childAction?.Invoke();
                }
            }
            ImUi.End();

            if (!open && _isOpen)
            {
                _isOpen = false;
                OnClosed?.Invoke();
            }
        }

        void IMyra.DrawMyra()
        {
        }

        MyraUi.Window IMyraWindow.GetMyraWindow()
        {
            return MyraControl;
        }

        MyraUi.Container IMyraContainer.GetMyraContainer()
        {
            return (MyraUi.Container)MyraControl.Content;
        }

        GumUi.Panel IGumWindow.GetGumPanel()
        {
            return GumControl;
        }

        GumUi.Panel IGumContainer.GetGumContainer()
        {
            return GumControl;
        }

        Action IImGuiContainer.GetChildrenDrawAction()
        {
            return () =>
            {
                foreach (var action in _imguiChildActions)
                {
                    action?.Invoke();
                }
            };
        }

        void IImGuiContainer.AddChildDrawAction(Action drawAction)
        {
            _imguiChildActions.Add(drawAction);
        }

        void IImGuiContainer.ClearChildDrawActions()
        {
            _imguiChildActions.Clear();
        }
    }
}