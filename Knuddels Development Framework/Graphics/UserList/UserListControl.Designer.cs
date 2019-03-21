namespace KDF.Graphics.UserList
{
    partial class UserListControl
    {

        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.ListTitle = new System.Windows.Forms.TextBox();
            this.ListContainer = new System.Windows.Forms.Panel();
            this.ItemListContainer = new System.Windows.Forms.Panel();
            this.ListContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListTitle
            // 
            this.ListTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.ListTitle.Location = new System.Drawing.Point(0, 0);
            this.ListTitle.Name = "ListTitle";
            this.ListTitle.ReadOnly = true;
            this.ListTitle.Size = new System.Drawing.Size(0, 20);
            this.ListTitle.TabIndex = 2;
            this.ListTitle.Text = "Channelname";
            this.ListTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ListContainer
            // 
            this.ListContainer.AutoScroll = true;
            this.ListContainer.AutoSize = true;
            this.ListContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ListContainer.Controls.Add(this.ItemListContainer);
            this.ListContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.ListContainer.Location = new System.Drawing.Point(0, 20);
            this.ListContainer.Name = "ListContainer";
            this.ListContainer.Size = new System.Drawing.Size(0, 0);
            this.ListContainer.TabIndex = 3;
            // 
            // ItemListContainer
            // 
            this.ItemListContainer.AutoSize = true;
            this.ItemListContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.ItemListContainer.Location = new System.Drawing.Point(0, 0);
            this.ItemListContainer.Name = "ItemListContainer";
            this.ItemListContainer.Size = new System.Drawing.Size(0, 0);
            this.ItemListContainer.TabIndex = 0;
            // 
            // NickList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ListContainer);
            this.Controls.Add(this.ListTitle);
            this.Name = "NickList";
            this.Size = new System.Drawing.Size(0, 20);
            this.ListContainer.ResumeLayout(false);
            this.ListContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ListTitle;
        public System.Windows.Forms.Panel ListContainer;
        private System.Windows.Forms.Panel ItemListContainer;
    }

}
