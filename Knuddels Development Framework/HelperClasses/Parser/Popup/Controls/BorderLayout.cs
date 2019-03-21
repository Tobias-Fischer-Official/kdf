using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.HelperClasses.Parser.Popup.Controls
{
    internal class BorderLayout : Layout
    {
        public const string NORTH = "NORTH";
        public const string SOUTH = "SOUTH";
        public const string EAST = "EAST";
        public const string WEST = "WEST";
        public const string CENTER = "CENTER";

        internal System.Windows.Forms.Control North { get; set; }
        internal System.Windows.Forms.Control East { get; set; }
        internal System.Windows.Forms.Control West { get; set; }
        internal System.Windows.Forms.Control South { get; set; }
        internal System.Windows.Forms.Control Center { get; set; }
        internal List<System.Windows.Forms.Control> Controls { get; set; }

        internal BorderLayout()
        {
            Controls = new List<System.Windows.Forms.Control>();
        }

    }
}
