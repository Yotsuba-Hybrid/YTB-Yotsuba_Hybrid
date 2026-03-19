//Code for TitleScreen
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System.Linq;
partial class TitleScreenRuntime : Gum.Wireframe.BindableGue
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        GumRuntime.ElementSaveExtensions.RegisterGueInstantiationType("TitleScreen", typeof(TitleScreenRuntime));
    }
    public ButtonDenyRuntime ButtonDenyInstance { get; protected set; }
    public ButtonCloseRuntime ButtonCloseInstance { get; protected set; }
    public DialogBoxRuntime DialogBoxInstance { get; protected set; }

    public TitleScreenRuntime(bool fullInstantiation = true, bool tryCreateFormsObject = true)
    {
        if(fullInstantiation)
        {
            var element = ObjectFinder.Self.GetElementSave("TitleScreen");
            element?.SetGraphicalUiElement(this, global::RenderingLibrary.SystemManagers.Default);
        }



    }
    public override void AfterFullCreation()
    {
        ButtonDenyInstance = this.GetGraphicalUiElementByName("ButtonDenyInstance") as ButtonDenyRuntime;
        ButtonCloseInstance = this.GetGraphicalUiElementByName("ButtonCloseInstance") as ButtonCloseRuntime;
        DialogBoxInstance = this.GetGraphicalUiElementByName("DialogBoxInstance") as DialogBoxRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
