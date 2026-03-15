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
    public class Panel : ContainerBase, IPanel, IMyraContainer, IGumContainer, IImGuiContainer
    {
        private string _text = string.Empty;
        private readonly List<Action> _imguiChildActions = new();

        public override string Text
        {
            get => _text;
            set => _text = value;
        }

        public override Color Color { get; set; }
        public override Vector2 Position { get; set; }

        private MyraUi.Panel MyraControl { get; set; }
        private GumUi.Panel GumControl { get; set; }

        public Panel()
        {
            MyraControl = new MyraUi.Panel();
            GumControl = new GumUi.Panel
            {
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
            ImUi.BeginChild(_text);
            foreach (var childAction in _imguiChildActions)
            {
                childAction?.Invoke();
            }
            ImUi.EndChild();
        }

        void IMyra.DrawMyra()
        {
        }

        MyraUi.Container IMyraContainer.GetMyraContainer()
        {
            return MyraControl;
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