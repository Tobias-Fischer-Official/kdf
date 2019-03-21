using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KDF.HelperClasses.Parser.Popup.Controls
{
    public class PopupPanel : System.Windows.Forms.Panel
    {

        private Layout _Layout;

        public PopupPanel(Layout pLayout)
        {
            this._Layout = pLayout;
        }

        public void addControl(string pLayout, System.Windows.Forms.Control pControl)
        {
            if (this._Layout.GetType().Equals(typeof(BorderLayout)))
                addControlBorderLayout(pLayout, pControl);
            else if (this._Layout.GetType().Equals(typeof(FlowLayout)))
                addControlFlowLayout(pControl);
            else if (this._Layout.GetType().Equals(typeof(GridLayout)))
                addControlGridLayout(pControl);
        }

        private void addControlBorderLayout(string pLayout, System.Windows.Forms.Control pControl)
        {
            switch (pLayout)
            {
                case BorderLayout.NORTH:
                    pControl.Dock = System.Windows.Forms.DockStyle.Top;
                    ((BorderLayout)this._Layout).North = pControl;
                    break;
                case BorderLayout.EAST:
                    pControl.Dock = System.Windows.Forms.DockStyle.Right;
                    ((BorderLayout)this._Layout).East = pControl;
                    break;
                case BorderLayout.WEST:
                    pControl.Dock = System.Windows.Forms.DockStyle.Left;
                    ((BorderLayout)this._Layout).West = pControl;
                    break;
                case BorderLayout.SOUTH:
                    pControl.Dock = System.Windows.Forms.DockStyle.Bottom;
                    ((BorderLayout)this._Layout).South = pControl;
                    break;
                case BorderLayout.CENTER:
                    pControl.Dock = System.Windows.Forms.DockStyle.Fill;
                    ((BorderLayout)this._Layout).Center = pControl;
                    break;
                default:
                    pControl.Dock = System.Windows.Forms.DockStyle.Bottom | System.Windows.Forms.DockStyle.Left;
                    ((BorderLayout)this._Layout).Controls.Add(pControl);
                    break;
            }
        }
        private void addControlFlowLayout(System.Windows.Forms.Control pControl)
        {
            ((FlowLayout)this._Layout).Controls.Add(pControl);
        }
        private void addControlGridLayout(System.Windows.Forms.Control pControl)
        {
            ((GridLayout)this._Layout).Controls.Add(pControl);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                List<System.Windows.Forms.Control> cons = new List<System.Windows.Forms.Control>();
                #region BoderLayout
                if (this._Layout.GetType().Equals(typeof(BorderLayout)))
                {
                    BorderLayout layout = ((BorderLayout)this._Layout);

                    if (layout.West != null)
                    {
                        cons.Add(layout.West);
                    }
                    if (layout.East != null)
                    {
                        cons.Add(layout.East);
                    } 
                    if (layout.Center != null)
                    {
                        cons.Add(layout.Center);
                    }
                    if (layout.North != null)
                    {
                        cons.Add(layout.North);
                    }
                    if (layout.South != null)
                    {
                        cons.Add(layout.South);
                    }
                    foreach (System.Windows.Forms.Control control in layout.Controls)
                        cons.Add(control);
                }
                #endregion
                #region FlowLayout
                else if (this._Layout.GetType().Equals(typeof(FlowLayout)))
                {
                    System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel()
                    {
                        Anchor = System.Windows.Forms.AnchorStyles.None,
                        AutoSize = true,
                        AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
                    };

                    int left = 0;
                    foreach (System.Windows.Forms.Control control in ((FlowLayout)this._Layout).Controls)
                    {
                        control.Left = left;
                        panel.Controls.Add(control);
                        left += control.Width + 2;
                    }
                    panel.Left = (this.Width / 2) - (panel.Width / 2) - 20;
                    cons.Add(panel);
                }
                #endregion
                #region GridLayout
                else if (this._Layout.GetType().Equals(typeof(GridLayout)))
                {
                    System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel()
                    {
                        AutoSize = true,
                        AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
                    };

                    int colsCount = 0;
                    int rowsCount = 0;
                    int top = 0;
                    int left = 0;

                    bool first = true;
                    GridLayout layout = ((GridLayout)this._Layout);
                    foreach (System.Windows.Forms.Control control in Controls)
                    {
                        if (colsCount == layout.Cols)
                        {
                            top += control.Height + layout.HGrap;
                            left = 0;
                            colsCount = 0;
                        }
                        else if (colsCount < layout.Cols)
                        {
                            left += first ? 0 : (control.Width + layout.VGrap);
                            first = false;
                        }

                        control.Top = top;
                        control.Left = left;
                        panel.Controls.Add(control);

                        rowsCount++;
                        colsCount++;
                    }
                    cons.Add(panel);
                }
                
                #endregion

                //cons.Reverse();
                this.Controls.AddRange(cons.ToArray());
            }
        }
    }
}
