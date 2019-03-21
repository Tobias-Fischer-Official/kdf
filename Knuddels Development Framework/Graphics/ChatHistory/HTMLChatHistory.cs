using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KDF.ChatObjects;
using System.Threading;
using System.Drawing;
using KDF.Helper.Parser;
using KDF.HelperClasses.Parser;

namespace KDF.Graphics
{
    public class HTMLChatHistory : WebBrowser
    {
        public event EventHandler<LinkClickedEventArgs> OnLinkClicked;
        public ContextMenuStrip ctmsLink;

        private Channel _channel;
        private string _styleSheet = "";
        private System.ComponentModel.IContainer _components;
        private ToolStripMenuItem _toolStripMenuItem1;
        private ToolStripMenuItem _toolStripMenuItem2;
        private bool _firstLoad = true;
        private List<string> _workingRows = new List<string>();
        private List<string> _waitingRows = new List<string>();
        private int _x = 0;
        private int? _scrollTop = null;

        public HTMLChatHistory()
        {

        }

        public void Activate(Channel channel)
        {
            this.AllowWebBrowserDrop = false;
            this.WebBrowserShortcutsEnabled = false;
            this.IsWebBrowserContextMenuEnabled = false;
            this.ScrollBarsEnabled = false;

            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            this.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(ChatBrowser_DocumentCompleted);
            this.Navigating += new WebBrowserNavigatingEventHandler(HTMLTextBox_Navigating);

            this._channel = channel;
            Clear();
            //this.Dock = DockStyle.Fill;
            Thread thr = new Thread(Work);
            thr.Start();
        }

        void HTMLTextBox_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (!_firstLoad)
                e.Cancel = true;
            else
                _firstLoad = false;
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            try
            {
                HtmlElement link = this.Document.ActiveElement;
                this.OnLinkClicked(this, new LinkClickedEventArgs(link.GetAttribute("name").Split('|')[1]));
                this.ContextMenu.Disposed += new EventHandler(ContextMenu_Disposed);
                this.ContextMenu.Dispose();
                this.IsWebBrowserContextMenuEnabled = false;
            }
            catch { }
        }

        void ContextMenu_Disposed(object sender, EventArgs e)
        {
            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
            this.IsWebBrowserContextMenuEnabled = false;
        }

        string PreparedStyleSheet()
        {
            string stylesheet = global::KDF.Properties.Resources.stylesheet;
            stylesheet = stylesheet.Replace("/*", "");
            stylesheet = stylesheet.Replace("*/", "");
            stylesheet = stylesheet.Replace("<bgcolor>", ColorTranslator.ToHtml(_channel.BackColor));
            //stylesheet = stylesheet.Replace("<bgimage>", " url('" + CS.bgImagePath + "')");
            stylesheet = stylesheet.Replace("<foreColor>", ColorTranslator.ToHtml(_channel.ForeColor));
            stylesheet = stylesheet.Replace("<fontSize>", _channel.FontSize.ToString());
            stylesheet = stylesheet.Replace("<linepitch>", _channel.LinePitch.ToString());
            stylesheet = stylesheet.Replace("<blue>", ColorTranslator.ToHtml(_channel.Color2));
            //stylesheet = stylesheet.Replace("", "");
            return stylesheet;
        }

        public void Clear()
        {
            _firstLoad = true;
            this.DocumentText = "<!-- ChannelLog :" + _channel.Name + ": " + DateTime.Now.ToLongDateString() + " -->\n\n"
                + "<html>\n"
                + "<head>\n"
                + "<style type=\"text/css\">\n"
                + PreparedStyleSheet()
                + "</style>\n"
                + "</head>\n"
                + "<body>\n"
                + "<div>\n"
                + "<img oncontextmenu=\"return false\" id=\"background\" src=\"http://chat.knuddels.de/" + _channel.BackgroundImage + "\" alt=\"\" title=\"\" /> \n"
                + "</div>\n"
                + "<div id=\"ContentContainer\"></div>"
                + "</body>\n"
                + "</html>\n";
        }

        public void Work()
        {
            while (true)
            {
                if (this._waitingRows.Count > 0)
                {
                    lock (_workingRows)
                    {
                        _workingRows = new List<string>();
                        _workingRows.AddRange(_waitingRows);
                        _waitingRows.Clear();
                    }
                    while (_workingRows.Count > 0)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            foreach (string row in _workingRows)
                                this.Document.GetElementById("ContentContainer").InnerHtml += row;
                            _workingRows.Clear();
                            if (!this.Focused)
                                ScrollIt();
                            LinkIt();
                            Application.DoEvents();
                        });
                    }
                }
                else
                    Thread.Sleep(400);
            }
        }

        private void LinkClicked(object sender, EventArgs e)
        {
            string text = this.DocumentText;
            _scrollTop = this.Document.GetElementById("ContentContainer").ScrollTop;
            HtmlElement link = this.Document.ActiveElement;
            this.OnLinkClicked(this, new LinkClickedEventArgs(link.GetAttribute("name").Split('|')[0]));
            this.DocumentText = text;
        }

        public void Append(PublicMessage msg, Channel channel, bool function)
        {
            bool ub = msg.Sender == ">" || msg.Sender == ">>" || msg.Sender == ":";
            string row = KCode2HTML.ToHTML(msg.Message, channel, function, ub);
            string nick = KCode2HTML.ToHTML("_°>_h" + msg.Sender + "|/m \"|/w \"<°:_ ", channel, false, false);
            if (ub)
                nick = string.Empty;
            row = nick + row;
            AppendHTML(row);
        }

        public void Append(PrivateMessage msg, Channel channel)
        {
            string row = KCode2HTML.ToHTML(msg.Message, channel, false, false);
            string nick = KCode2HTML.ToHTML("_RR°>_h" + msg.Sender + "|/m \"|/w \"<°:_ ", channel, false, false);
            if (msg.Sender == ">" || msg.Sender == ">>" || msg.Sender == ":")
                nick = string.Empty;
            row = nick + row;
            AppendHTML(row);
        }

        private void AppendHTML(string row)
        {
            try
            {
                string timeStamp = DateTime.Now.ToShortTimeString();
                lock (_waitingRows)
                    _waitingRows.Add("<!-- " + timeStamp + " --><div class=\"row\">" + row + "</div>\n<div style=\"clear:left;\"></div>\n");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        private void ScrollIt()
        {
            if (_scrollTop == null)
                this.Document.GetElementById("ContentContainer").ScrollTop = this.Document.GetElementById("ContentContainer").ScrollRectangle.Bottom;
            else if (_x <= 1)
            {
                this.Document.GetElementById("ContentContainer").ScrollTop = (int)_scrollTop;
                _x++;
                if (_x >= 2)
                {
                    _x = 0;
                    _scrollTop = null;
                }
            }

        }

        private void LinkIt()
        {
            HtmlElementCollection links = this.Document.Links;
            foreach (HtmlElement link in links)
            {
                link.DetachEventHandler("onclick", new EventHandler(LinkClicked));
                link.AttachEventHandler("onclick", new EventHandler(LinkClicked));
            }
        }

        private void ChatBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.WebBrowserShortcutsEnabled = false;
            this.IsWebBrowserContextMenuEnabled = false;
            ScrollIt();
            LinkIt();
        }

        private void InitializeComponent()
        {
            this._components = new System.ComponentModel.Container();
            this.ctmsLink = new System.Windows.Forms.ContextMenuStrip(this._components);
            this._toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ctmsLink.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctmsLink
            // 
            this.ctmsLink.Items.AddRange(new System.Windows.Forms.ToolStripItem[] 
                {
                    this._toolStripMenuItem1,
                    this._toolStripMenuItem2
                });
            this.ctmsLink.Name = "contextMenuStrip1";
            this.ctmsLink.Size = new System.Drawing.Size(181, 48);
            // 
            // HTMLTextBox
            // 
            this.AllowNavigation = false;
            this.AllowWebBrowserDrop = false;
            this.ContextMenuStrip = this.ctmsLink;
            this.IsWebBrowserContextMenuEnabled = false;
            this.ctmsLink.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
