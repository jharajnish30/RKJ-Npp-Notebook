using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;
using Kbg.NppPluginNET;

namespace Kbg.NppPluginNET
{
	public partial class frmSettings : Form
	{
		public frmSettings()
		{
			InitializeComponent();
		}

		private void frmRKJNppPlugin_Load(object sender, EventArgs e)
		{
			//Main.AllNotebooks();
			foreach (string str in Main.AllNotebooks())
			{
				lbNotebooks.Items.Add(str);
			}
		}

		private void frmRKJNppPlugin_Click(object sender, EventArgs e)
		{
			//MessageBox.Show(Main.ConfigFolder);
			//MessageBox.Show(Main.RootFolder);
			//MessageBox.Show(Main.NotebookFolder);

		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			if (txtSearch.Text.Length < 1) 
			{ 
				MessageBox.Show("Please enter some text to search..!!");
				return;
			}

			List<string> getAllFiles = Main.SearchStringInAllNotebooks(txtSearch.Text);

			if (getAllFiles == null)
			{
				MessageBox.Show("That text does not appeared in any Notebook..!!");
				return;
			}

			lbNotebooks.Items.Clear();
			foreach (string str in getAllFiles)
			{
				lbNotebooks.Items.Add(str);
			}

			////NPPM_LAUNCHFINDINFILESDLG
			//this.Close();
			//(new ScintillaGateway(PluginBase.GetCurrentScintilla())).ShowFindInFilesDlg(Main.NotebookFolder, "*.*");
		}

		private void btnAllNotebook_Click(object sender, EventArgs e)
		{
			//Main.NotebookFolder
			//Main.AllNotebooks();
			lbNotebooks.Items.Clear();
			foreach(string str in Main.AllNotebooks())
			{
				lbNotebooks.Items.Add(str);
			}
		}

		private void lbNotebooks_SelectedIndexChanged(object sender, EventArgs e)
		{
			//Main.NotebookFolder
			string crrfile = Main.NotebookFolder + "\\" + lbNotebooks.SelectedItem.ToString();
			Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DOOPEN, Win32.MAX_PATH, crrfile);

		}
	}
}