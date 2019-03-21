using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.HelperClasses.Parser.Popup.Controls
{
    internal class GridLayout : Layout
    {
        internal int Rows { get; set; }
        internal int Cols { get; set; }
        internal int HGrap { get; set; }
        internal int VGrap { get; set; }

        internal List<System.Windows.Forms.Control> Controls { get; set; }

        internal GridLayout(int pRows, int pCols, int pHGrap, int pVGrap)
        {
            Rows = pRows;
            Cols = pCols;
            HGrap = pHGrap;
            VGrap = pVGrap;
            Controls = new List<System.Windows.Forms.Control>();
        }
    }
}
