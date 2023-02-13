namespace Kbg.NppPluginNET
{
    partial class frmSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.btnSearch = new System.Windows.Forms.Button();
			this.btnAllNotebook = new System.Windows.Forms.Button();
			this.txtSearch = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lbNotebooks = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// btnSearch
			// 
			this.btnSearch.Location = new System.Drawing.Point(416, 10);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(171, 23);
			this.btnSearch.TabIndex = 2;
			this.btnSearch.Text = "Search Text in All Notebooks";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// btnAllNotebook
			// 
			this.btnAllNotebook.Location = new System.Drawing.Point(593, 10);
			this.btnAllNotebook.Name = "btnAllNotebook";
			this.btnAllNotebook.Size = new System.Drawing.Size(100, 23);
			this.btnAllNotebook.TabIndex = 3;
			this.btnAllNotebook.Text = "All Notebook";
			this.btnAllNotebook.UseVisualStyleBackColor = true;
			this.btnAllNotebook.Click += new System.EventHandler(this.btnAllNotebook_Click);
			// 
			// txtSearch
			// 
			this.txtSearch.Location = new System.Drawing.Point(13, 13);
			this.txtSearch.Name = "txtSearch";
			this.txtSearch.Size = new System.Drawing.Size(394, 20);
			this.txtSearch.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(222, 309);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(264, 25);
			this.label1.TabIndex = 5;
			this.label1.Text = "More Settings to Come Soon";
			// 
			// lbNotebooks
			// 
			this.lbNotebooks.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lbNotebooks.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbNotebooks.FormattingEnabled = true;
			this.lbNotebooks.ItemHeight = 20;
			this.lbNotebooks.Location = new System.Drawing.Point(13, 41);
			this.lbNotebooks.Name = "lbNotebooks";
			this.lbNotebooks.Size = new System.Drawing.Size(680, 260);
			this.lbNotebooks.Sorted = true;
			this.lbNotebooks.TabIndex = 6;
			this.lbNotebooks.SelectedIndexChanged += new System.EventHandler(this.lbNotebooks_SelectedIndexChanged);
			// 
			// frmSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(705, 343);
			this.Controls.Add(this.lbNotebooks);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtSearch);
			this.Controls.Add(this.btnAllNotebook);
			this.Controls.Add(this.btnSearch);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "RKJ Notebook Settings";
			this.Load += new System.EventHandler(this.frmRKJNppPlugin_Load);
			this.Click += new System.EventHandler(this.frmRKJNppPlugin_Click);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.Button btnAllNotebook;
		private System.Windows.Forms.TextBox txtSearch;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox lbNotebooks;
	}
}