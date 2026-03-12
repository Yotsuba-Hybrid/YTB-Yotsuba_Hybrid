using System;
using System.Collections.Generic;
using System.Text;
using GumUi = Gum.Forms.Controls;

namespace YotsubaEngine.Forms.Contract.GumUI
{
    public interface IGumLabel : IGum
    {
        public GumUi.Label GetGumLabel();
    }
}
