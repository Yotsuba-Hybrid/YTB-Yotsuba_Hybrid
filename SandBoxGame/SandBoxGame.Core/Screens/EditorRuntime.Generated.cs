//Code for Editor
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System.Linq;
partial class EditorRuntime : Gum.Wireframe.BindableGue
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        GumRuntime.ElementSaveExtensions.RegisterGueInstantiationType("Editor", typeof(EditorRuntime));
    }
    public ListBoxRuntime ListBoxInstance { get; protected set; }
    public ButtonTabRuntime ButtonTabInstance { get; protected set; }
    public ButtonTabRuntime ButtonTabInstance1 { get; protected set; }
    public DividerHorizontalRuntime DividerHorizontalInstance { get; protected set; }
    public RectangleRuntime RectangleInstance { get; protected set; }
    public ContainerRuntime ContainerInstance { get; protected set; }
    public CircleRuntime CircleInstance { get; protected set; }

    public EditorRuntime(bool fullInstantiation = true, bool tryCreateFormsObject = true)
    {
        if(fullInstantiation)
        {
            var element = ObjectFinder.Self.GetElementSave("Editor");
            element?.SetGraphicalUiElement(this, global::RenderingLibrary.SystemManagers.Default);
        }



    }
    public override void AfterFullCreation()
    {
        ListBoxInstance = this.GetGraphicalUiElementByName("ListBoxInstance") as ListBoxRuntime;
        ButtonTabInstance = this.GetGraphicalUiElementByName("ButtonTabInstance") as ButtonTabRuntime;
        ButtonTabInstance1 = this.GetGraphicalUiElementByName("ButtonTabInstance1") as ButtonTabRuntime;
        DividerHorizontalInstance = this.GetGraphicalUiElementByName("DividerHorizontalInstance") as DividerHorizontalRuntime;
        RectangleInstance = this.GetGraphicalUiElementByName("RectangleInstance") as global::MonoGameGum.GueDeriving.RectangleRuntime;
        ContainerInstance = this.GetGraphicalUiElementByName("ContainerInstance") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        CircleInstance = this.GetGraphicalUiElementByName("CircleInstance") as global::MonoGameGum.GueDeriving.CircleRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
