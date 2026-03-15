using System;

namespace YotsubaEngine.Forms.Contract.ImGUI
{
    public interface IImGuiContainer : IImGui
    {
        Action GetChildrenDrawAction();
        void AddChildDrawAction(Action drawAction);
        void ClearChildDrawActions();
    }
}