using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KDF.ChatObjects;
using KDF.Helper.Parser;
using System.IO;

namespace KDF.Graphics.UserList
{
    public partial class UserListItemControl : UserControl
    {
        public Dictionary<string, PictureBox> ItemPicBoxes = new Dictionary<string, PictureBox>();
        public event EventHandler<ItemSelectedChangedEventArgs> OnItemSelectedChanged;
        public event EventHandler<ItemRightClickShiftedEventArgs> OnItemRightClickShifted;

        private ChannelUser _user;
        public ChannelUser User
        {
            get { return _user; }
        }

        private bool _ItemSelected = false;
        public bool ItemSelected
        {
            get { return _ItemSelected; }
            set { _ItemSelected = value; }
        }

        private Label ItemLabel = new Label();

        private Color ItemSavedForeColor = Color.White;
        private Color ItemSavedBackColor = Color.DarkBlue;
        private Color _backColor;

        private Font ItemNormalFont = new Font("Arial", (float)11.25, FontStyle.Regular);
        private Font ItemBoldFont = new Font("Arial", (float)11.25, FontStyle.Bold);
        private Font ItemUnderlineFont = new Font("Arial", (float)11.25, FontStyle.Underline);
        private Font ItemUnderlineFontBold = new Font("Arial", (float)11.25, FontStyle.Underline | FontStyle.Bold);
        private Font ItemFont;

        private bool shiftPressed;

        /// <summary>
        /// Erstellt ein neues Item 
        /// </summary>
        /// <param name="User">der Usöööör hihi</param>
        public UserListItemControl(ChannelUser User, Color BackColor)
        {
            InitializeComponent();

            _user = User;
            _backColor = BackColor;

            ItemFont = (_user.FontFormat == 'b') ? ItemBoldFont : ItemNormalFont;

            //Das Label wird Links gebunden, dh der Nick steht immer Links
            ItemLabel.Dock = DockStyle.Fill;

            ItemLabel.Font = ItemFont;
            //Wir fügen alle pics des Nicks hinzu
            //foreach (string picName in User.UserListImages)
            //    AddPicture(picName,Image.FromStream(new MemoryStream(ImageHelper.LoadImage(picName).Data)));
            Repaint();

            ItemLabel.MouseLeave += new EventHandler(ItemLabel_MouseLeave);
            ItemLabel.MouseHover += new EventHandler(ItemLabel_MouseHover);

            this.KeyDown += new KeyEventHandler(UserListItem_KeyDown);
            this.KeyUp += new KeyEventHandler(UserListItemControl_KeyUp);

            ItemLabel.MouseClick += new MouseEventHandler(ExtendedNickListItem_MouseClick);
            panelNickName.MouseClick += new MouseEventHandler(ExtendedNickListItem_MouseClick);
            panelNickPics.MouseClick += new MouseEventHandler(ExtendedNickListItem_MouseClick);
        }

        void UserListItemControl_KeyUp(object sender, KeyEventArgs e)
        {
            shiftPressed = e.Shift;
        }

        void UserListItem_KeyDown(object sender, KeyEventArgs e)
        {
            shiftPressed = e.Shift;
        }


        public void ItemLabel_MouseLeave(object sender, EventArgs e)
        {
            if (ItemLabel.Font.Bold)
                ItemLabel.Font = ItemBoldFont;
            else
                ItemLabel.Font = ItemNormalFont;
            ItemLabel.Location = new Point(ItemLabel.Location.X, ItemLabel.Location.Y - 2);            
        }

        public void ItemLabel_MouseHover(object sender, EventArgs e)
        {
            if (ItemLabel.Font.Bold)
                ItemLabel.Font = ItemUnderlineFontBold;
            else
                ItemLabel.Font = ItemUnderlineFont;
            ItemLabel.Location = new Point(ItemLabel.Location.X, ItemLabel.Location.Y + 2);
            this.Focus();
        }

        public void ExtendedNickListItem_MouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();
            //Wenn wir den Nick anklicken möchten wir das gerne Sehen,
            //anhand einer Markierung
            if (e.Button == MouseButtons.Left && OnItemSelectedChanged != null)
            {
                //Aktuelle Vordergrundfarbe temporär speichern
                Color temp = _user.ForeColor;
                //Die Vordergrundfarbe des Users Ändern
                _user.ForeColor = ItemSavedForeColor;
                //Die Temporäre Farbe zwischenspeichern
                ItemSavedForeColor = temp;
                //Aktuelle Hintergrundfarbe temporär speichern
                temp = _backColor;
                //Die Hintergrundfarbe des Users ändern
                _backColor = ItemSavedBackColor;
                //Die Temporäre Farbe zwischenspeichern
                ItemSavedBackColor = temp;
                ItemSelected = !ItemSelected;
                Repaint();
                //Wir lösen zum Schluss noch den Event aus, und teilen mit das sich etwas geändert hat
                OnItemSelectedChanged(this, new ItemSelectedChangedEventArgs(ItemSelected, this));
            }
            else if (e.Button == MouseButtons.Right && shiftPressed && OnItemRightClickShifted != null)
                OnItemRightClickShifted(this, new ItemRightClickShiftedEventArgs(_user.Name));
        }

        public void UpdatePictures(Dictionary<string, Image> newImageList)
        {
            this.ItemPicBoxes.Clear();
            foreach (KeyValuePair<string, Image> kvp in newImageList)
                AddPicture(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Fügt ein neues Bild zum Nick hinzu
        /// </summary>
        /// <param name="imageName">Name des Bildes</param>
        /// <param name="image">Bild</param>
        public void AddPicture(string imageName, Image image)
        {
            try
            {
                //Neue PicturBox erstellen
                PictureBox pib = new PictureBox();
                pib.Image = image;
                pib.SizeMode = PictureBoxSizeMode.StretchImage;
                //Alle statuspics sind 18x18 pixel groß
                pib.Size = new Size(18, 18);
                pib.Name = imageName;
                pib.BackColor = _backColor;
                //Der Liste unserer Boxen hinzufügen
                ItemPicBoxes.Add(pib.Name, pib);
                pib.MouseClick += new MouseEventHandler(ExtendedNickListItem_MouseClick);
                //Control neu zeichnen
                Repaint();
            }
            catch /*(ArgumentException argEx)*/
            {
                //MessageBox.Show("Bild ist bereits auf dem Benutzer\r\n" + argEx.ToString());
            }
        }

        /// <summary>
        /// Entfernt, wenn vorhanden, ein Bild vom Item
        /// </summary>
        /// <param name="imageName">Das Bild was zu entfernen ist</param>
        public void RemovePicture(string imageName)
        {
            //Wir suchen und entfernen das Bild wieder
            try { ItemPicBoxes.Remove(imageName); }
            catch { /*Bild nicht vorhanden*/ }
            Repaint();
        }

        /// <summary>
        /// Zeichnet ein Item vollständig neu
        /// </summary>
        private void Repaint()
        {
            //Und alle Controls aus unserem Nick
            panelNickName.Controls.Clear();
            panelNickPics.Controls.Clear();
            //Stellen die höhe auf 18 ein
            Height = 18;
            //Färben den hintergrund ein
            BackColor = _backColor;

            //Erstellen das Label neu in dem der Nicktext steht
            ItemLabel.BackColor = _backColor;
            if (_user.Age != 0)
                ItemLabel.Text = _user.Name + "(" + _user.Age.ToString() + ")";
            else
                ItemLabel.Text = _user.Name;
            ItemLabel.Font = ItemFont;
            ItemLabel.Location = new Point(0, 4);
            ItemLabel.Height = 18;

            ItemLabel.ForeColor = _user.ForeColor;
            ItemLabel.BackColor = _backColor;
            ItemLabel.AutoSize = true;

            panelNickName.Controls.Add(ItemLabel);
            //MessageBox.Show(ItemLabel.Width.ToString());


            //Und dann die Bilder
            foreach (PictureBox picBox in ItemPicBoxes.Values)
            {
                picBox.Dock = DockStyle.Right;
                picBox.BackColor = _backColor;
                picBox.Dock = DockStyle.Left;
                panelNickPics.Controls.Add(picBox);
                picBox.BringToFront();
            }
        }
    }
}