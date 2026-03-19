//Code for YTBTEST
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System.Linq;
partial class YTBTESTRuntime : Gum.Wireframe.BindableGue
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        GumRuntime.ElementSaveExtensions.RegisterGueInstantiationType("YTBTEST", typeof(YTBTESTRuntime));
    }
    public LabelRuntime LabelInstance { get; protected set; }
    public MenuItemRuntime MenuItemInstance { get; protected set; }
    public WindowRuntime WindowInstance { get; protected set; }
    public MenuRuntime MenuInstance { get; protected set; }

    public YTBTESTRuntime(bool fullInstantiation = true, bool tryCreateFormsObject = true)
    {
        if(fullInstantiation)
        {
            var element = ObjectFinder.Self.GetElementSave("YTBTEST");
            element?.SetGraphicalUiElement(this, global::RenderingLibrary.SystemManagers.Default);
        }



    }
    public override void AfterFullCreation()
    {
        LabelInstance = this.GetGraphicalUiElementByName("LabelInstance") as LabelRuntime;
        MenuItemInstance = this.GetGraphicalUiElementByName("MenuItemInstance") as MenuItemRuntime;
        WindowInstance = this.GetGraphicalUiElementByName("WindowInstance") as WindowRuntime;
        MenuInstance = this.GetGraphicalUiElementByName("MenuInstance") as MenuRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
