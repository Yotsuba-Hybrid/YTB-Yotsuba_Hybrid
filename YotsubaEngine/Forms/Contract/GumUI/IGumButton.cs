using System;
using System.Collections.Generic;
using System.Text;
using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Contract.GumUI
{
    public interface IGumButton : IGum
    {
        public GumUi.Button GetGumButton();
    }
}
