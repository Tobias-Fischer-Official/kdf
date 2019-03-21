using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using KDF.ChatObjects;

namespace KDF.Graphics.UserList
{
    public partial class UserListControl : UserControl
    {
        public event EventHandler<ItemSelectedChangedEventArgs> OnItemSelectedChanged;
        private Channel _channel;
        public Dictionary<string, UserListItemControl> Items = new Dictionary<string, UserListItemControl>();
        private List<UserListItemControl> exItemBackUpArr = new List<UserListItemControl>();
        public Dictionary<string, ChannelUser> AllUsers = new Dictionary<string, ChannelUser>();
        public event EventHandler<ItemRightClickShiftedEventArgs> OnItemRightClickShifted;

        private Color _HeaderForeColor;
        public Color HeaderForeColor
        {
            get { return _HeaderForeColor; }
            set { _HeaderForeColor = value; }
        }

        private Color _HeaderBackColor;
        public Color HeaderBackColor
        {
            get { return _HeaderBackColor; }
            set { _HeaderBackColor = value; }
        }

        private string _HeaderTitle;
        public string HeaderTitle
        {
            get { return _HeaderTitle; }
            set { _HeaderTitle = value; }
        }

        private string _ItemDescription;
        public string ItemDescription
        {
            get { return _ItemDescription; }
            set { _ItemDescription = value; }
        }

        private string _ListName;
        public string ListName
        {
            get { return _ListName; }
            set { _ListName = value; }
        }

        private void nli_OnItemSelectedChanged(object sender, ItemSelectedChangedEventArgs e)
        {
            if (OnItemSelectedChanged != null)
                OnItemSelectedChanged(this, new ItemSelectedChangedEventArgs(e.Selected, e.Item));
        }

        public UserListControl()
        {
            InitializeComponent();
        }

        public UserListControl(Channel channel)
        {
            InitializeComponent();
            _channel = channel;
        }

        private void SetHeaderAppearance(Color HeaderForeColor, Color HeaderBackColor, string HeaderTitle, string ItemDescription)
        {
            //Der Header wird Formatiert
            this.HeaderBackColor = HeaderBackColor;
            this.HeaderForeColor = HeaderForeColor;
            this.ItemDescription = ItemDescription;
            this.HeaderTitle = HeaderTitle + "(" + this.Items.Count.ToString() + ")" + ItemDescription;
            //Das Control wird neu gezeichnet
            RepaintHeader();
        }

        /// <summary>
        /// Setzt den Nmen der Nickliste
        /// </summary>
        /// <param name="ListName">Der Name</param>
        public void SetListName(string ListName)
        {
            this.ListName = ListName;
        }

        /// <summary>
        /// Leert die Liste komplett
        /// </summary>
        public void Clear()
        {
            AllUsers.Clear();
            Items.Clear();
            ItemListContainer.Controls.Clear();
            this.ListName = "ListenName";
            SetHeaderAppearance(Color.Black, Color.White, "ListenName", ItemDescription);
            RepaintHeader();
        }

        /// <summary>
        /// Füllt die Liste mit Nicks
        /// </summary>
        /// <param name="backColor">Hintergrundfarbe des Channels</param>
        public void FillNickList(Channel channel)
        {
            try
            {
                //Das Layout freigeben, sonst gibt das eine ziemliche hängerei beim zeichnen
                this.ListContainer.SuspendLayout();
                //Hintergrundfarbe übernehmen
                this.BackColor = channel.BackColor;
                this.ItemListContainer.BackColor = channel.BackColor;
                //Die einzelnen Items erstellen
                this.Items = new Dictionary<string, UserListItemControl>();

                foreach (ChannelUser channelUser in channel.UserList.ChannelUserList)
                {
                    UserListItemControl userListItemControl = new UserListItemControl(channelUser, channel.BackColor);
                    userListItemControl.Dock = DockStyle.Bottom;
                    userListItemControl.OnItemSelectedChanged += new EventHandler<ItemSelectedChangedEventArgs>(nli_OnItemSelectedChanged);
                    userListItemControl.OnItemRightClickShifted += new EventHandler<ItemRightClickShiftedEventArgs>(userListItemControl_OnItemRightClickShifted);
                    //Es muss immer erst der User ins Dictionary eingefügt werden
                    //Nur hier kann kontrolliert werden, obe er sich bereits in der liste befindet!!!

                    this.AllUsers.Add(channelUser.Name, channelUser);
                    this.ItemListContainer.Controls.Add(userListItemControl);
                    this.Items.Add(channelUser.Name, userListItemControl);
                    Application.DoEvents();
                }
                //GUI-Kram
                ItemListContainer.SendToBack();
                //Anhand des Channelnamens den Listennamen einstellen
                this.ListName = channel.Name;
                //Den Header anfertigen
                SetHeaderAppearance(Color.White, channel.BackColor, this.ListName, this.ItemDescription);
                RepaintHeader();
                //Das Layout wieder aufnehmen
                this.ListContainer.ResumeLayout(false);
                //Und neu zeichnen lassen
                this.ListContainer.PerformLayout();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show("Ein Item war bereits in der Liste (Liste Doppelt erstellt?)\r\n" + argEx.ToString());
            }
        }

        void userListItemControl_OnItemRightClickShifted(object sender, ItemRightClickShiftedEventArgs e)
        {
            if (OnItemRightClickShifted != null)
                OnItemRightClickShifted(this, e);
        }

        public void ChangeChannel(Channel channel)
        {
            Clear();
            FillNickList(channel);
        }

        /// <summary>
        /// Zeichnet den Header Neu
        /// </summary>
        public void RepaintHeader()
        {
            //Hier wird der Header aktualisiert
            this._HeaderTitle = this.ListName + " (" + (Items.Count).ToString() + ") " + this.ItemDescription;
            this.ListTitle.Text = _HeaderTitle;
            this.ListTitle.ForeColor = _HeaderForeColor;
            this.ListTitle.BackColor = _HeaderBackColor;
            this.ListTitle.BorderStyle = BorderStyle.None;
        }

        /// <summary>
        /// Fügt dcer Userliste einen User hinzu
        /// </summary>
        /// <param name="user">Der hinzuzufügende User</param>
        /// <param name="backColor">Die Hintergrundfarbe die der Eintrag haben soll</param>
        public void AddUser(ChannelUser user, Color backColor)
        {
            try
            {
                //User hinzufügen
                UserListItemControl userListItemControl = new UserListItemControl(user, backColor);
                //Die Box für den User Oben anheften
                userListItemControl.Dock = DockStyle.Bottom;
                userListItemControl.OnItemSelectedChanged += new EventHandler<ItemSelectedChangedEventArgs>(nli_OnItemSelectedChanged);
                userListItemControl.OnItemRightClickShifted+=new EventHandler<ItemRightClickShiftedEventArgs>(userListItemControl_OnItemRightClickShifted);
                //Das NickListItem in die Liste hinzufügen
                this.Items.Add(user.Name, userListItemControl);
                //Das Control zur GUI hinzufügen
                this.ItemListContainer.Controls.Add(userListItemControl);
                //Den User selbst hinzufügen
                this.AllUsers.Add(user.Name, user);
                RepaintHeader();
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show("Item wurde bereits hinzugefügt\r\n" + argEx.ToString());
            }
        }

        /// <summary>
        /// Entfernt einen user aus Der Liste
        /// </summary>
        /// <param name="Nick">Der Nick des Users</param>
        public void RemoveUser(string ItemName)
        {
            //Das Layout freigeben, sonst gibt das eine ziemliche hängerei beim zeichnen
            this.ListContainer.SuspendLayout();
            try
            {
                //Den User aus allen Listen entfernen
                this.exItemBackUpArr.Remove(Items[ItemName]);
                this.ItemListContainer.Controls.Remove(Items[ItemName]);
                this.Items.Remove(ItemName);
                this.AllUsers.Remove(ItemName);
                Application.DoEvents();
            }
            catch { /*User ist nicht in der Liste*/ }

            //Das Layout wieder aufnehmen
            this.ListContainer.ResumeLayout(false);
            //Und neu zeichnen lassen
            this.ListContainer.PerformLayout();
        }

        /// <summary>
        /// Fügt ein Bild hinzu oder entfernt es
        /// </summary>
        public void ChangeUserState(bool Add, string nick, string imageName, Image userImage)
        {
            //Wir gehen die Elemente durch
            foreach (KeyValuePair<string, UserListItemControl> kvp in Items)
                //Wenn der Nick übereinstimmt und wir hinzufügen sollen
                if (kvp.Value.User.Name == nick && Add)
                    //fügen wir dem User ein Bild hinzu
                    kvp.Value.AddPicture(imageName, userImage);
                //Wenn nicht, entfernen wir es
                else if (kvp.Value.User.Name == nick && !Add)
                {
                    AllUsers[nick].UserListImages.Remove(imageName);
                    kvp.Value.RemovePicture(imageName);
                }
        }

        /// <summary>
        /// Sortiert die Nicks ind er Liste anhand eines bestimmten Bildes
        /// </summary>
        /// <param name="picName">Der Name des Bildes</param>
        public void SortByPicture(string picName)
        {
            //Das Layout freigeben, sonst gibt das eine ziemliche hängerei beim zeichnen
            this.ListContainer.SuspendLayout();
            //Hier werden alle Nicks nach einem Bild sortiert
            List<UserListItemControl> exItemArr = new List<UserListItemControl>(this.Items.Values);
            //Ein Backup der ALten Liste erstellen
            exItemBackUpArr.AddRange(exItemArr);
            //Alle Nicks werden nach dem Bild druchsucht
            foreach (UserListItemControl exItem in exItemArr)
            {
                //Wenn sie es besitzen werden sie oben angeheftet
                if (exItem.ItemPicBoxes.ContainsKey(picName))
                    exItem.Dock = DockStyle.Top;
                //Wenn nicht, dann unten
                else
                    exItem.Dock = DockStyle.Bottom;
                Application.DoEvents();
            }
            //Das Layout wieder aufnehmen
            this.ListContainer.ResumeLayout(false);
            //Und neu zeichnen lassen
            this.ListContainer.PerformLayout();
        }

        /// <summary>
        /// Stellt die ALte Nickreihenfolge wieder her
        /// </summary>
        public void ResetItemAlignment()
        {
            //Das Layout freigeben, sonst gibt das eine ziemliche hängerei beim zeichnen
            this.ListContainer.SuspendLayout();
            this.ListContainer.Visible = false;
            //Vollständige neuzeichnung der NickListe und
            //Wiederherstellung der alten Nickreihenfolge
            //Nur wenn vorher ein Backup angefertigt wurde
            if (exItemBackUpArr.Count >= 1)
            {
                this.ItemListContainer.Controls.Clear();
                foreach (UserListItemControl exItem in exItemBackUpArr)
                {
                    exItem.Dock = DockStyle.Bottom;
                    this.ItemListContainer.Controls.Add(exItem);
                    Application.DoEvents();
                }
                exItemBackUpArr.Clear();
            }
            //Das Layout wieder aufnehmen
            this.ListContainer.ResumeLayout(false);
            //Und neu zeichnen lassen
            this.ListContainer.PerformLayout();
            this.ListContainer.Visible = true;
        }
    }
}
