//Code for Controls/ItemsControl (Container)
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using GumRuntime;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;
using System.Linq;
partial class ItemsControlRuntime : ContainerRuntime
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        GumRuntime.ElementSaveExtensions.RegisterGueInstantiationType("Controls/ItemsControl", typeof(ItemsControlRuntime));
    }
    public global::Gum.Forms.Controls.ItemsControl FormsControl => FormsControlAsObject as global::Gum.Forms.Controls.ItemsControl;
    public enum ItemsControlCategory
    {
        Enabled,
        Disabled,
        Focused,
        DisabledFocused,
    }

    ItemsControlCategory? _itemsControlCategoryState;
    public ItemsControlCategory? ItemsControlCategoryState
    {
        get => _itemsControlCategoryState;
        set
        {
            _itemsControlCategoryState = value;
            if(value != null)
            {
                if(Categories.ContainsKey("ItemsControlCategory"))
                {
                    var category = Categories["ItemsControlCategory"];
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.ApplyState(state);
                }
                else
                {
                    var category = ((global::Gum.DataTypes.ElementSave)this.Tag).Categories.FirstOrDefault(item => item.Name == "ItemsControlCategory");
                    var state = category.States.Find(item => item.Name == value.ToString());
                    this.ApplyState(state);
                }
            }
        }
    }
    public NineSliceRuntime Background { get; protected set; }
    public ScrollBarRuntime VerticalScrollBarInstance { get; protected set; }
    public ContainerRuntime ClipContainerInstance { get; protected set; }
    public ContainerRuntime InnerPanelInstance { get; protected set; }
    public NineSliceRuntime FocusedIndicator { get; protected set; }
    public ContainerRuntime ClipAndScrollContainer { get; protected set; }
    public ContainerRuntime ClipContainerParent { get; protected set; }

    public ItemsControlRuntime(bool fullInstantiation = true, bool tryCreateFormsObject = true)
    {
        if(fullInstantiation)
        {
            var element = ObjectFinder.Self.GetElementSave("Controls/ItemsControl");
            element?.SetGraphicalUiElement(this, global::RenderingLibrary.SystemManagers.Default);
        }



    }
    public override void AfterFullCreation()
    {
        if (FormsControl == null)
        {
            FormsControlAsObject = new global::Gum.Forms.Controls.ItemsControl(this);
        }
        Background = this.GetGraphicalUiElementByName("Background") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        VerticalScrollBarInstance = this.GetGraphicalUiElementByName("VerticalScrollBarInstance") as ScrollBarRuntime;
        ClipContainerInstance = this.GetGraphicalUiElementByName("ClipContainerInstance") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        InnerPanelInstance = this.GetGraphicalUiElementByName("InnerPanelInstance") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        FocusedIndicator = this.GetGraphicalUiElementByName("FocusedIndicator") as global::MonoGameGum.GueDeriving.NineSliceRuntime;
        ClipAndScrollContainer = this.GetGraphicalUiElementByName("ClipAndScrollContainer") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        ClipContainerParent = this.GetGraphicalUiElementByName("ClipContainerParent") as global::MonoGameGum.GueDeriving.ContainerRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
