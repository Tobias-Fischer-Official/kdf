using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.HelperClasses.Parser.Popup.Controls
{
    internal class FlowLayout : Layout
    {
        internal List<System.Windows.Forms.Control> Controls { get; set; }

        internal FlowLayout()
        {
            Controls = new List<System.Windows.Forms.Control>();
        }

    }
}
