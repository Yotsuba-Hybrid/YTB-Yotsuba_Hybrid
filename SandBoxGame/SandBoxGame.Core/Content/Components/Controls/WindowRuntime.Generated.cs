//Code for Controls/Window (Container)
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System.Linq;
partial class WindowRuntime : ContainerRuntime
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        GumRuntime.ElementSaveExtensions.RegisterGueInstantiationType("Controls/Window", typeof(WindowRuntime));
    }
    public global::Gum.Forms.Window FormsControl => FormsControlAsObject as global::Gum.Forms.Window;
    public NineSliceRuntime Background { get; protected set; }
    public PanelRuntime InnerPanelInstance { get; protected set; }
    public PanelRuntime TitleBarInstance { get; protected set; }
    public PanelRuntime BorderTopLeftInstance { get; protected set; }
    public PanelRuntime BorderTopRightInstance { get; protected set; }
    public PanelRuntime BorderBottomLeftInstance { get; protected set; }
    public PanelRuntime BorderBottomRightInstance { get; protected set; }
    public PanelRuntime BorderTopInstance { get; protected set; }
    public PanelRuntime BorderBottomInstance { get; protected set; }
    public PanelRuntime BorderLeftInstance { get; protected set; }
    public PanelRuntime BorderRightInstance { get; protected set; }

    public WindowRuntime(bool fullInstantiation = true, bool tryCreateFormsObject = true)
    {
        if(fullInstantiation)
        {
            var element = ObjectFinder.Self.GetElementSave("Controls/Window");
            element?.SetGraphicalUiElement(this, global::RenderingLibrary.SystemManagers.Default);
        }



    }
    public override void AfterFullCreation()
    {
        if (FormsControl == null)
        {
            FormsControlAsObject = new global::Gum.Forms.Window(this);
        }
        Background = this.GetGraphicalUiElementByName("Background") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        InnerPanelInstance = this.GetGraphicalUiElementByName("InnerPanelInstance") as PanelRuntime;
        TitleBarInstance = this.GetGraphicalUiElementByName("TitleBarInstance") as PanelRuntime;
        BorderTopLeftInstance = this.GetGraphicalUiElementByName("BorderTopLeftInstance") as PanelRuntime;
        BorderTopRightInstance = this.GetGraphicalUiElementByName("BorderTopRightInstance") as PanelRuntime;
        BorderBottomLeftInstance = this.GetGraphicalUiElementByName("BorderBottomLeftInstance") as PanelRuntime;
        BorderBottomRightInstance = this.GetGraphicalUiElementByName("BorderBottomRightInstance") as PanelRuntime;
        BorderTopInstance = this.GetGraphicalUiElementByName("BorderTopInstance") as PanelRuntime;
        BorderBottomInstance = this.GetGraphicalUiElementByName("BorderBottomInstance") as PanelRuntime;
        BorderLeftInstance = this.GetGraphicalUiElementByName("BorderLeftInstance") as PanelRuntime;
        BorderRightInstance = this.GetGraphicalUiElementByName("BorderRightInstance") as PanelRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
