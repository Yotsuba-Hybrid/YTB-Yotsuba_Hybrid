using System;
using System.Collections.Generic;
using System.Text;
using MyraUi = Myra.Graphics2D.UI;

namespace YotsubaEngine.Forms.Contract.Myra
{
    public interface IMyraLabel : IMyra
    {
        public MyraUi.Label GetMyraLabel();
    }
}
