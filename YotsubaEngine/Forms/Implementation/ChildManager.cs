using System.Collections.Generic;
using System.Linq;
using YotsubaEngine.Forms.Contract;
using YotsubaEngine.Forms.Contract.GumUI;
using YotsubaEngine.Forms.Contract.Myra;
using YotsubaEngine.Forms.Contract.ImGUI;
using GumUi = Gum.Forms.Controls;
using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Implementation
{
    internal static class ChildManager
    {
        public static void AddChildToContainer(IContainer container, IForm child)
        {
            if (child == null || container == null) return;

            child.Parent = container;

            if (container is IMyraContainer myraContainer && child is IMyra myraChild)
            {
                var myraWidget = GetMyraWidget(myraChild);
                if (myraWidget != null)
                {
                    myraContainer.GetMyraContainer().AddChild(myraWidget);
                }
            }

            if (container is IGumContainer gumContainer && child is IGum gumChild)
            {
                var gumElement = GetGumElement(gumChild);
                if (gumElement != null)
                {
                    gumContainer.GetGumContainer().AddChild(gumElement);
                }
            }

            if (container is IImGuiContainer imguiContainer && child is IImGui imguiChild)
            {
                imguiContainer.AddChildDrawAction(() =>
                {
                    imguiChild.DrawImGuI();
                });
            }

            if (container is ContainerBase containerBase)
            {
                if (!containerBase.InternalChildren.Contains(child))
                {
                    containerBase.InternalChildren.Add(child);
                }
            }
        }

        public static void RemoveChildFromContainer(IContainer container, IForm child)
        {
            if (child == null || container == null) return;

            child.Parent = null;

            if (container is IMyraContainer myraContainer && child is IMyra myraChild)
            {
                var myraWidget = GetMyraWidget(myraChild);
                if (myraWidget != null)
                {
                    myraContainer.GetMyraContainer().RemoveChild(myraWidget);
                }
            }

            if (container is IGumContainer gumContainer && child is IGum gumChild)
            {
                var gumElement = GetGumElement(gumChild);
                if (gumElement != null)
                {
                    gumContainer.GetGumContainer().RemoveChild(gumElement);
                }
            }

            if (container is IImGuiContainer imguiContainer && child is IImGui)
            {
                imguiContainer.ClearChildDrawActions();
            }

            if (container is ContainerBase containerBase)
            {
                containerBase.InternalChildren.Remove(child);
            }
        }

        public static void ClearAllChildren(IContainer container)
        {
            if (container == null) return;

            if (container is ContainerBase containerBase)
            {
                foreach (var child in containerBase.InternalChildren.ToList())
                {
                    RemoveChildFromContainer(container, child);
                }
            }
        }

        private static MyraUi.Widget GetMyraWidget(IMyra myraControl)
        {
            return myraControl switch
            {
                IMyraButton btn => btn.GetMyraButton(),
                IMyraLabel lbl => lbl.GetMyraLabel(),
                IMyraCheckBox chk => chk.GetMyraCheckBox(),
                IMyraTextBox txt => txt.GetMyraTextBox(),
                IMyraSlider sld => sld.GetMyraSlider(),
                IMyraComboBox cmb => cmb.GetMyraComboBox(),
                IMyraWindow win => win.GetMyraWindow(),
                _ => null
            };
        }

        private static GumUi.FrameworkElement GetGumElement(IGum gumControl)
        {
            return gumControl switch
            {
                IGumButton btn => btn.GetGumButton(),
                IGumLabel lbl => lbl.GetGumLabel(),
                IGumCheckBox chk => chk.GetGumCheckBox(),
                IGumTextBox txt => txt.GetGumTextBox(),
                IGumSlider sld => sld.GetGumSlider(),
                IGumComboBox cmb => cmb.GetGumComboBox(),
                IGumWindow win => win.GetGumPanel(),
                _ => null
            };
        }
    }

    public abstract class ContainerBase : IContainer
    {
        protected readonly List<IForm> _children = new();
        protected IContainer _parent;

        public IReadOnlyList<IForm> Children => _children.AsReadOnly();
        public IContainer Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public abstract string Text { get; set; }
        public abstract Microsoft.Xna.Framework.Color Color { get; set; }
        public abstract Microsoft.Xna.Framework.Vector2 Position { get; set; }

        internal List<IForm> InternalChildren => _children;

        public virtual void AddChild(IForm child)
        {
            ChildManager.AddChildToContainer(this, child);
        }

        public virtual void RemoveChild(IForm child)
        {
            ChildManager.RemoveChildFromContainer(this, child);
        }

        public virtual void ClearChildren()
        {
            ChildManager.ClearAllChildren(this);
        }
    }
}