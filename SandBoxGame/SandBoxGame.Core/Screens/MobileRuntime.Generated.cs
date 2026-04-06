//Code for Mobile
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System.Linq;
partial class MobileRuntime : Gum.Wireframe.BindableGue
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        GumRuntime.ElementSaveExtensions.RegisterGueInstantiationType("Mobile", typeof(MobileRuntime));
    }
    public LabelRuntime LabelInstance3 { get; protected set; }
    public LabelRuntime LabelInstance2 { get; protected set; }
    public StackPanelRuntime StackPanelInstance2 { get; protected set; }
    public LabelRuntime LabelInstance1 { get; protected set; }
    public ComboBoxRuntime ComboBoxInstance { get; protected set; }
    public ButtonIconRuntime ButtonIconInstance1 { get; protected set; }
    public ButtonIconRuntime ButtonIconInstance { get; protected set; }
    public LabelRuntime LabelInstance { get; protected set; }
    public MenuRuntime MenuInstance { get; protected set; }
    public StackPanelRuntime StackPanelInstance1 { get; protected set; }
    public StackPanelRuntime StackPanelInstance { get; protected set; }
    public WindowRuntime WindowInstance { get; protected set; }

    public MobileRuntime(bool fullInstantiation = true, bool tryCreateFormsObject = true)
    {
        if(fullInstantiation)
        {
            var element = ObjectFinder.Self.GetElementSave("Mobile");
            element?.SetGraphicalUiElement(this, global::RenderingLibrary.SystemManagers.Default);
        }



    }
    public override void AfterFullCreation()
    {
        LabelInstance3 = this.GetGraphicalUiElementByName("LabelInstance3") as LabelRuntime;
        LabelInstance2 = this.GetGraphicalUiElementByName("LabelInstance2") as LabelRuntime;
        StackPanelInstance2 = this.GetGraphicalUiElementByName("StackPanelInstance2") as StackPanelRuntime;
        LabelInstance1 = this.GetGraphicalUiElementByName("LabelInstance1") as LabelRuntime;
        ComboBoxInstance = this.GetGraphicalUiElementByName("ComboBoxInstance") as ComboBoxRuntime;
        ButtonIconInstance1 = this.GetGraphicalUiElementByName("ButtonIconInstance1") as ButtonIconRuntime;
        ButtonIconInstance = this.GetGraphicalUiElementByName("ButtonIconInstance") as ButtonIconRuntime;
        LabelInstance = this.GetGraphicalUiElementByName("LabelInstance") as LabelRuntime;
        MenuInstance = this.GetGraphicalUiElementByName("MenuInstance") as MenuRuntime;
        StackPanelInstance1 = this.GetGraphicalUiElementByName("StackPanelInstance1") as StackPanelRuntime;
        StackPanelInstance = this.GetGraphicalUiElementByName("StackPanelInstance") as StackPanelRuntime;
        WindowInstance = this.GetGraphicalUiElementByName("WindowInstance") as WindowRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
