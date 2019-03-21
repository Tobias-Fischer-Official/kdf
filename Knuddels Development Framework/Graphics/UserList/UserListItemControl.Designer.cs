using System.Windows.Forms;
namespace KDF.Graphics.UserList
{
    partial class UserListItemControl
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
            this.panelNickName = new System.Windows.Forms.Panel();
            this.panelNickPics = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelNickName
            // 
            this.panelNickName.AutoSize = true;
            this.panelNickName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelNickName.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelNickName.Location = new System.Drawing.Point(0, 0);
            this.panelNickName.Name = "panelNickName";
            this.panelNickName.Size = new System.Drawing.Size(0, 18);
            this.panelNickName.TabIndex = 0;
            // 
            // panelNickPics
            // 
            this.panelNickPics.AutoSize = true;
            this.panelNickPics.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelNickPics.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelNickPics.Location = new System.Drawing.Point(0, 0);
            this.panelNickPics.Name = "panelNickPics";
            this.panelNickPics.Size = new System.Drawing.Size(0, 18);
            this.panelNickPics.TabIndex = 1;
            // 
            // ExtendedNickListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelNickPics);
            this.Controls.Add(this.panelNickName);
            this.Name = "ExtendedNickListItem";
            this.Size = new System.Drawing.Size(156, 18);            
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panelNickName;
        private Panel panelNickPics;
    }
}
