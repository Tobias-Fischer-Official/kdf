using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using KDF.HelperClasses.Parser.Popup.Controls;

namespace KDF.HelperClasses.Parser.Popup
{
    /// <summary>
    /// Die Klasse zum Handlen eines vom Server gesendetem Popup-Fenster
    /// </summary>
    public class PopupParser
    {
        #region private Vars
        private PopupReader _Reader;
        public PopupPanel Container { get; private set; }
        #endregion

        #region public Vars
        /// <summary>
        /// Gibt den Title das Popup-Fenstes zurück
        /// </summary>
        public string WindowTitle { get; private set; }

        /// <summary>
        /// Gibt die größe des Popup-Fenstes zurück
        /// </summary>
        public System.Drawing.Size WindowSize { get; private set; }

        /// <summary>
        /// Gibt die Position des Popup-Fenstes zurück
        /// </summary>
        public System.Drawing.Point WindowLocation { get; private set; }

        /// <summary>
        /// Gibt die Hintergrundfarbe des Popup-Fenstes zurück
        /// </summary>
        public System.Drawing.Color BackColor { get; private set; }

        /// <summary>
        /// Gibt die Vordergrundfarbe des Popup-Fenstes zurück
        /// </summary>
        public System.Drawing.Color ForeColor { get; private set; }

        /// <summary>
        /// Gibt an ob der user die Größe des Popup-Fenstes ändern kann oder nicht
        /// </summary>
        public bool Resizeable { get; private set; }

        /// <summary>
        /// Gibt den Opcode zurück der bei einer Server anfrage gesendet wird
        /// </summary>
        public string SendOpcode { get; private set; }

        /// <summary>
        /// Gibt den Parameter zurück der bei einer Server anfrage gesendet wird
        /// </summary>
        public string SendParameter { get; private set; }

        /// <summary>
        /// Gibt an ob es sich um ein neues oder ein altes Popup handelt
        /// </summary>
        public bool IsNewPopup { get; private set; }

        public string HTML { get; private set; }
        #endregion

        #region Konstruktor
        /// <summary>
        /// Erstellt eine neue Instanz der Parser-Klasse und speichert alle Daten eines vom Server gesendetem Popup-Fensters ab
        /// </summary>
        /// <param name="pPacket">Das Packet vom Server</param>
        public PopupParser(string pPacket)
        {
            if (pPacket.StartsWith("k\0"))
                pPacket = pPacket.Substring(2);

            IsNewPopup = false;

            this._Reader = new PopupReader(Encoding.Default.GetBytes(pPacket));
            Parse();
        }
        #endregion

        #region Parse
        private void Parse()
        {
            WindowTitle = this._Reader.ReadString();

            while (!this._Reader.End())
            {
                switch (this._Reader.Read())
                {
                    case 0x73: //SendOpcode/Parameter
                        SendOpcode = this._Reader.ReadString();
                        SendParameter = this._Reader.ReadString();
                        break;
                    case 0x77: //Size
                        WindowSize = new System.Drawing.Size(this._Reader.ReadShort(), this._Reader.ReadShort());
                        break;
                    case 0x70: //Position
                        WindowLocation = new System.Drawing.Point(this._Reader.ReadShort(), this._Reader.ReadShort());
                        break;
                    case 0x68: //BackColor
                        BackColor = this._Reader.ReadColor();
                        break;
                    case 0x66: //ForeColor
                        ForeColor = this._Reader.ReadColor();
                        break;
                    case 0x72: //Resizable
                        Resizeable = true;
                        break;
                    case 0x6F:
                        //Console.WriteLine("Frame (o) " + this._Reader.ReadString());
                        //Hat irgend was mit dem Painting zu tun
                        break;
                    case 0x6D: //KP
                        //Console.WriteLine("Frame (m) " + this._Reader.ReadShort());
                        break;
                    case 0x64:
                        //Console.WriteLine("Frame (d) " + this._Reader.ReadString());
                        //Wird beim aufruf vom KeyEvent mit dem Code 201 (Exit) an den Server gesendet
                        //SendOpcode + Delimiter(\0) + SendParameter + Delimiter(\0) + readedString
                        break;
                    case 0x61:
                        //AddItemListener(this);
                        break;
                }
            }

            Container = ParsePanel();
            Container.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        private PopupPanel ParsePanel()
        {
            StringBuilder sb = new StringBuilder("<P>");
            PopupPanel popupPanel = new PopupPanel(new BorderLayout());

            #region Layout
            int i = 0;
            int j = 0;
            int end;
            do
            {
                end = 0;
                switch (this._Reader.Read())
                {
                    #region BackgroundImage
                    case 0x55:
                        string backgroundImageURL = this._Reader.ReadString();
                        if (backgroundImageURL.StartsWith("pics/"))
                        {
                            System.Windows.Forms.PictureBox pb = new System.Windows.Forms.PictureBox();
                            pb.Load("http://knuddels.net/" + backgroundImageURL);
                            popupPanel.BackgroundImage = pb.Image;
                            //pControl.BackgroundImage = ImageHelper.LoadImage(bgImg);
                            if (popupPanel.BackgroundImage != null)
                            {
                                popupPanel.Size = popupPanel.BackgroundImage.Size;
                                popupPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;
                                
                            }
                        }
                        break;
                    #endregion
                    #region ScrollPane
                    case 0x50:
                        popupPanel.Size = new System.Drawing.Size(this._Reader.ReadShort(), this._Reader.ReadShort());
                        popupPanel.AutoScroll = true;
                        break;
                    #endregion
                    #region GridLayout
                    case 0x47:
                        popupPanel = new PopupPanel(new GridLayout(this._Reader.ReadSize(), this._Reader.ReadSize(), this._Reader.ReadSize(), this._Reader.ReadSize()));
                        break;
                    #endregion
                    #region CardLayout
                    case 0x43:
                        //popupPanel = new PopupPanel(new CardLayout());
                        j = 1;
                        break;
                    #endregion
                    #region FlowLayout
                    case 0x46:
                        popupPanel = new PopupPanel(new FlowLayout());
                        break;
                    #endregion
                    #region BorderLayout
                    case 0x42:
                    default:
                        i = 1;
                        break;
                    #endregion
                }
            } while (end != 0);
            #endregion

            int x = 0;
            int y = 0;
            while (!this._Reader.End())
            {
                string popupAlign = string.Empty;

                #region Align
                if (i != 0)
                {
                    switch (this._Reader.Read())
                    {
                        case 0x4E:
                            popupAlign = "NORTH";
                            break;
                        case 0x53:
                            popupAlign = "SOUTH";
                            break;
                        case 0x45:
                            popupAlign = "EAST";
                            break;
                        case 0x57:
                            popupAlign = "WEST";
                            break;
                        case 0x43:
                            popupAlign = "CENTER";
                            break;
                        default:
                            this._Reader.Back(1);
                            popupAlign = "CENTER";
                            break;
                    }
                }
                #endregion

                #region Controls
                System.Windows.Forms.Control pControl = null;
                string controlId;

                int fontSize = 8;
                System.Drawing.FontStyle fontStyle = System.Drawing.FontStyle.Regular;
                string controlcallBack;

                int opcode = this._Reader.Read();
                switch (opcode)
                {
                    #region Panel
                    case 0x70:
                        controlId = this._Reader.ReadString(opcode);
                        pControl = ParsePanel();
                        //((PopupPanel)pControl).AutoSize = true;
                        break;
                    #endregion
                    #region Label
                    case 0x6C:
                        bool setBackgroundColor = true;
                        System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                        label.Text = this._Reader.ReadString();
                        label.AutoSize = false;
                        controlId = this._Reader.ReadString(opcode);
                        controlcallBack = "ã";

                        while (!this._Reader.End())
                        {
                            switch (this._Reader.Read())
                            {
                                #region Font
                                case 0x70:
                                    fontStyle = System.Drawing.FontStyle.Regular;
                                    break;
                                case 0x69:
                                    fontStyle = System.Drawing.FontStyle.Italic;
                                    break;
                                case 0x62:
                                    fontStyle = System.Drawing.FontStyle.Bold;
                                    break;
                                case 0x67:
                                    fontSize = this._Reader.ReadSize() - 8;
                                    break;
                                #endregion
                                #region Colors
                                case 0x74:
                                    setBackgroundColor = false;
                                    break;
                                case 0x68:
                                    if (setBackgroundColor)
                                        label.BackColor = this._Reader.ReadColor();
                                    break;
                                case 0x66:
                                    label.ForeColor = this._Reader.ReadColor();
                                    break;
                                #endregion
                                #region CallBack
                                case 0x6E:
                                    controlcallBack += "ãsendbackã";
                                    break;
                                case 0x73:
                                    controlcallBack += "ãlightsubmit" + this._Reader.ReadString() + "ã";
                                    break;
                                #endregion
                                #region Unknown
                                case 0x6C:
                                case 0x63:
                                case 0x72:
                                case 0x75:
                                    break;
                                #endregion
                            }
                        }

                        if (fontSize > 0)
                            label.Font = new System.Drawing.Font("Arial", fontSize, fontStyle);
                        label.Height = System.Windows.Forms.TextRenderer.MeasureText(label.Text, label.Font).Height + 4;
                        label.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
                        pControl = label;
                        break;
                    #endregion
                    #region Button
                    case 0x62:
                        System.Windows.Forms.Button button = new System.Windows.Forms.Button();
                        button.Text = this._Reader.ReadString();

                        this._Reader.ReadString(opcode);
                        bool addForegroundColor = false;
                        controlcallBack = "ã";

                        while (!this._Reader.End())
                        {
                            switch (this._Reader.Read())
                            {
                                #region Style
                                case 0x63: //Neuer Button
                                    addForegroundColor = true;
                                    break;
                                case 0x65: //Styled (New Button)
                                    break;
                                #endregion
                                #region Font
                                case 0x70:
                                    fontStyle = System.Drawing.FontStyle.Regular;
                                    break;
                                case 0x69:
                                    fontStyle = System.Drawing.FontStyle.Italic;
                                    break;
                                case 0x62:
                                    fontStyle = System.Drawing.FontStyle.Bold;
                                    break;
                                case 0x67:
                                    fontSize = this._Reader.ReadSize() - 4;
                                    break;
                                #endregion
                                #region Colors
                                case 0x66:
                                    if (!addForegroundColor)
                                        button.ForeColor = this._Reader.ReadColor();
                                    break;
                                case 0x68:
                                    button.BackColor = this._Reader.ReadColor();
                                    break;
                                #endregion
                                #region CallBack
                                case 0x6E: //Sendback
                                    controlcallBack += "sendbackã";
                                    break;
                                case 0x53: //Submit
                                    controlcallBack += "submitã";
                                    break;
                                case 0x73: //Dispose (Close)
                                    controlcallBack += "disposeã";
                                    break;
                                case 0x75: //OpenURL
                                    controlcallBack += "openurl" + this._Reader.ReadString() + "ã";
                                    break;
                                case 0x6F: //LightSubmit
                                    controlcallBack += "lightsubmitã";
                                    break;
                                case 0x6B: //Id
                                    controlcallBack += "idõ" + this._Reader.ReadString() + "ã";
                                    break;
                                #endregion
                                #region Unknown
                                case 0x49:
                                    this._Reader.ReadString();
                                    break;
                                case 0x61:
                                    this._Reader.ReadString();
                                    break;
                                #endregion
                            }
                        }

                        if (fontSize > 0)
                            button.Font = new System.Drawing.Font("Arial", fontSize, fontStyle);

                        pControl = button;
                        break;
                    #endregion
                    #region KCodePanel
                    case 0x63:
                        System.Windows.Forms.Control kCodeControl = new System.Windows.Forms.Control();
                        string kCode = this._Reader.ReadString();
                        Debug.WriteLine(kCode);
                        controlId = this._Reader.ReadString(opcode);
                        bool bool3;
                        System.Drawing.Size prefferedDimension = new System.Drawing.Size(0, 0);

                        while (!this._Reader.End())
                        {
                            switch (this._Reader.Read())
                            {
                                #region CallBack
                                case 0x74:
                                    controlcallBack = this._Reader.ReadString();
                                    break;
                                #endregion
                                #region Colors
                                case 0x66:
                                    kCodeControl.ForeColor = this._Reader.ReadColor();
                                    break;
                                case 0x68:
                                    kCodeControl.BackColor = this._Reader.ReadColor();
                                    break;
                                #endregion
                                #region Dimension
                                case 0x73: //PrefferedDimension
                                    prefferedDimension = new System.Drawing.Size(this._Reader.ReadSize(), this._Reader.ReadSize());
                                    break;
                                #endregion
                                #region BackgroundImage
                                case 0x69: //BackgroundImage
                                    string bgImg = this._Reader.ReadString();
                                    int bgWdith = this._Reader.ReadShort(); //Nicht sicher, kp ? :D
                                    //Console.WriteLine("\tKCodePanel (BackImg) " + bgImg + " | " + bgWdith);
                                    System.Windows.Forms.PictureBox pb = new System.Windows.Forms.PictureBox();
                                    if (bgImg.StartsWith("pics/"))
                                        pb.Load("http://knuddels.net/" + bgImg);
                                    kCodeControl.BackgroundImage = pb.Image;
                                    break;
                                #endregion
                                #region Unknown
                                case 0x6E: //?
                                    bool3 = false;
                                    break;
                                #endregion
                            }
                        }

                        kCodeControl.Size = new System.Drawing.Size(200, 500);
                        kCodeControl.BackColor = System.Drawing.Color.Red;
                        pControl = kCodeControl;
                        break;
                    #endregion
                    #region TextArea
                    case 0x74:
                        System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
                        rtb.Text = this._Reader.ReadString();

                        controlId = this._Reader.ReadString(opcode);
                        int rows = this._Reader.ReadSize();
                        int cols = this._Reader.ReadSize();

                        while (!this._Reader.End())
                        {
                            switch (this._Reader.Read())
                            {
                                #region Font
                                case 0x67: //FontSize
                                    fontSize = this._Reader.ReadSize() - 8;
                                    break;
                                #endregion
                                #region ScrollBars
                                case 0x6E: //Horizontal
                                    rtb.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
                                    break;
                                case 0x73: //Vertical
                                    rtb.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
                                    break;
                                case 0x62:
                                    rtb.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
                                    break;
                                #endregion
                                #region Editable
                                case 0x65:
                                    controlcallBack = "sendbackã";
                                    rtb.ReadOnly = false;
                                    break;
                                #endregion
                                #region Colors
                                case 0x66:
                                    rtb.ForeColor = this._Reader.ReadColor();
                                    break;
                                case 0x68:
                                    rtb.BackColor = this._Reader.ReadColor();
                                    break;
                                #endregion
                            }
                        }

                        if (fontSize > 0)
                            rtb.Font = new System.Drawing.Font("Arial", fontSize, fontStyle);
                        pControl = rtb;
                        break;
                    #endregion
                    #region TextField
                    case 0x66:
                        System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
                        textBox.Text = this._Reader.ReadString();

                        int row = this._Reader.ReadSize();
                        controlId = this._Reader.ReadString(opcode);

                        while (!this._Reader.End())
                        {
                            switch (this._Reader.Read())
                            {
                                case 0x65: //Editable
                                    textBox.ReadOnly = this._Reader.ReadBoolean();
                                    break;
                                case 0x67: //FontSize
                                    fontSize = this._Reader.ReadSize() - 4;
                                    break;
                                case 0x63: //EchoChar
                                    textBox.PasswordChar = (char)this._Reader.Read();
                                    break;
                                #region Colors
                                case 0x66:
                                    textBox.ForeColor = this._Reader.ReadColor();
                                    break;
                                case 0x68:
                                    textBox.BackColor = this._Reader.ReadColor();
                                    break;
                                #endregion
                            }
                        }

                        if (fontSize > 0)
                            textBox.Font = new System.Drawing.Font("Arial", fontSize, fontStyle);
                        pControl = textBox;
                        break;
                    #endregion
                    #region CheckBox
                    case 0x78:
                        System.Windows.Forms.CheckBox checkBox = new System.Windows.Forms.CheckBox();
                        controlId = this._Reader.ReadString(opcode);

                        while (!this._Reader.End())
                        {
                            switch (this._Reader.Read())
                            {
                                case 0x6C: //Text
                                    checkBox.Text = this._Reader.ReadString();
                                    break;
                                case 0x73: //CheckedState
                                    checkBox.Checked = this._Reader.ReadBoolean();
                                    break;
                                case 0x64: //Disable
                                    checkBox.Enabled = false;
                                    break;
                                #region Font
                                case 0x70: //Regular
                                    fontStyle = System.Drawing.FontStyle.Regular;
                                    break;
                                case 0x69: //Itlalic
                                    fontStyle = System.Drawing.FontStyle.Italic;
                                    break;
                                case 0x62: //Bold
                                    fontStyle = System.Drawing.FontStyle.Bold;
                                    break;
                                case 0x67: //FontSize
                                    fontSize = this._Reader.ReadSize() - 4;
                                    break;
                                #endregion
                                #region Colors
                                case 0x66:
                                    checkBox.ForeColor = this._Reader.ReadColor();
                                    break;
                                case 0x68:
                                    checkBox.BackColor = this._Reader.ReadColor();
                                    break;
                                #endregion
                                #region Unknown
                                case 0x53:
                                    //Console.WriteLine("CheckBox (AddItemListener) " + this._Reader.ReadString());
                                    break;
                                case 0x72:
                                    this._Reader.ReadSize();
                                    break;
                                #endregion
                            }
                        }

                        if (fontSize > 0)
                            checkBox.Font = new System.Drawing.Font("Arial", fontSize, fontStyle);
                        pControl = checkBox;
                        break;
                    #endregion
                    #region ComboBox
                    case 0x6F:
                        System.Windows.Forms.ComboBox comboBox = new System.Windows.Forms.ComboBox();

                        controlcallBack = "ã";

                        int selectedItemIndex = -1;
                        string selectedItemText = null;

                        while (!this._Reader.End())
                        {
                            switch (this._Reader.Read())
                            {
                                case 0x63: //SetSelectedIndex
                                    selectedItemIndex = this._Reader.ReadSize();
                                    break;
                                case 0x43: //SetSelecedItem
                                    selectedItemText = this._Reader.ReadString();
                                    break;
                                case 0x64: //Disable
                                    comboBox.Enabled = false;
                                    break;
                                case 0x67: //FontSize
                                    fontSize = this._Reader.ReadSize() - 4;
                                    break;
                                case 0x41:
                                    controlcallBack += "sendbackã";
                                    break;
                                #region Colors
                                case 0x66:
                                    comboBox.ForeColor = this._Reader.ReadColor();
                                    break;
                                case 0x68:
                                    comboBox.BackColor = this._Reader.ReadColor();
                                    break;
                                #endregion
                                #region Unknown
                                case 0x61:
                                    Console.WriteLine("ComboBox (AddItemListener) " + this._Reader.ReadString());
                                    break;
                                #endregion
                            }
                        }

                        if (fontSize > 0)
                            comboBox.Font = new System.Drawing.Font("Arial", fontSize, fontStyle);
                        while (!this._Reader.End())
                        {
                            comboBox.Items.Add(this._Reader.ReadString());
                        }

                        if (selectedItemText != null)
                            comboBox.SelectedText = selectedItemText;
                        if (selectedItemIndex >= 0)
                            comboBox.SelectedIndex = selectedItemIndex;

                        pControl = comboBox;
                        break;
                    #endregion
                    #region Unknown
                    default:
                        controlId = this._Reader.ReadString(opcode);
                        break;
                    #endregion
                }
                #endregion

                if (pControl != null)
                {
                    
                    //pControl.Parent = popupPanel;
                    popupPanel.addControl(popupAlign, pControl);
                    //Console.WriteLine("Add Control " + pControl.GetType().Name);
                }
            }
            HTML = sb.ToString();
            return popupPanel;
        }

        #endregion
    }
}
