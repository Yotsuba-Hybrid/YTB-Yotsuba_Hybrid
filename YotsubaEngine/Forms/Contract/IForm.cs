namespace YotsubaEngine.Forms.Contract
{
    public interface IForm : ChildElement.Forms
    {
        IContainer Parent { get; set; }
    }
}