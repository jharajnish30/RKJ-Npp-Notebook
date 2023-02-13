using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using Kbg.NppPluginNET.PluginInfrastructure;
using System.Collections.Generic;

namespace Kbg.NppPluginNET
{
    class Main
    {
        #region "Variables"
        internal const string PluginName = "RKJ Notebook";
        static string iniFilePath = null;
        static bool someSetting = false;
        static frmSettings frmMyDlg = null;
        static int idMyDlg = -1;
        static Bitmap tbBmp = Properties.Resources.star;
        static Bitmap tbBmp_tbTab = Properties.Resources.star_bmp;
        static Icon tbIcon = null;
        public static string RootFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MyNoteBooks";
        public static string ConfigFolder = RootFolder + "\\.config";
        public static string NotebookFolder = RootFolder + "\\.books";
        #endregion

        #region "Plugin Main Routines"
        public static void OnNotification(ScNotification notification)
        {
            // This method is invoked whenever something is happening in notepad++
            // use eg. as
            // if (notification.Header.Code == (uint)NppMsg.NPPN_xxx)
            // { ... }
            // or
            //
            // if (notification.Header.Code == (uint)SciMsg.SCNxxx)
            // { ... }
        }

        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            PluginBase.SetCommand(0, "Search In &Notebooks (Ctrl+Alt+A)", SearchInNotebooks, new ShortcutKey(true, true, false, Keys.A));
            PluginBase.SetCommand(1, "New &Notebook (Ctrl+Alt+Z)", CreateNewNotebook, new ShortcutKey(true, true, false, Keys.Z));
            PluginBase.SetCommand(2, "Mark &Day Start (Ctrl+Alt+X)", MarkDayStart, new ShortcutKey(true, true, false, Keys.X));
            PluginBase.SetCommand(3, "Start New &Idea (Ctrl+Alt+C)", StartNewIdea, new ShortcutKey(true, true, false, Keys.C));
            PluginBase.SetCommand(4, "Idea &Sub-Section (Ctrl+Alt+V)", StartIdeaSubSection, new ShortcutKey(true, true, false, Keys.V));
            PluginBase.SetCommand(5, "Indent Bullet » (Ct+At+Sh+A)", IndentBulletOne, new ShortcutKey(true, true, true, Keys.A));
            PluginBase.SetCommand(6, "Indent Bullet → (Ct+At+Sh+S)", IndentBulletTwo, new ShortcutKey(true, true, true, Keys.S));
            PluginBase.SetCommand(7, "Indent Bullet ► (Ct+At+Sh+D)", IndentBulletThree, new ShortcutKey(true, true, true, Keys.D));
            PluginBase.SetCommand(8, "Indent Bullet ☼ (Ct+At+Sh+F)", IndentBulletFour, new ShortcutKey(true, true, true, Keys.F));
            PluginBase.SetCommand(9, "Indent Bullet › (Ct+At+Sh+G)", IndentBulletFive, new ShortcutKey(true, true, true, Keys.G));
            PluginBase.SetCommand(10, "RKJ Note&book Settings", ShowSettingDialog); idMyDlg = 1;

            if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder);
            if (!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);
            if (!Directory.Exists(NotebookFolder)) Directory.CreateDirectory(NotebookFolder);
        }

        internal static void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }
        #endregion

        #region "Menu Routines"
        internal static void SearchInNotebooks()
        {
            //(new ScintillaGateway(PluginBase.GetCurrentScintilla())).ShowFindInFilesDlg(NotebookFolder, "*.*");
            (new NotepadPPGateway()).ShowFindInFilesDlg(NotebookFolder, "*.*");
        }

        internal static void CreateNewNotebook()
        {
            string crrfile = NotebookFolder + "\\Notebook-" + CreateUniqueId();
            FileStream flstrm = File.Create(crrfile);

            string str = DayStartText();
            flstrm.Write(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));
            flstrm.Close();

            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DOOPEN, Win32.MAX_PATH, crrfile);

            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            tmp.GotoLine(9);
            int crpos = tmp.GetCurrentPos();
            tmp.SetSelection(crpos + 42, crpos + 23);
        }

        internal static void MarkDayStart()
        {
            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());

            tmp.LineEnd();
            tmp.NewLine();
            tmp.NewLine();

            //tmp.AddText(strb.Length, DayStartText());
            //tmp.InsertText(tmp.GetCurrentPos(), DayStartText());
            tmp.InsertTextAndMoveCursor(DayStartText());
            tmp.SetSel(tmp.GetAnchor() + 1504, tmp.GetAnchor() + 1523);
        }

        internal static void StartNewIdea()
        {
            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());

            tmp.LineEnd();
            tmp.NewLine();
            int crpos = tmp.GetAnchor();

            tmp.InsertText(crpos, NewIdeaText());
            tmp.GotoLine(tmp.GetCurrentLineNumber() + 4);
            crpos = tmp.GetCurrentPos();
            tmp.SetSelection(crpos + 42, crpos + 23);
        }

        internal static void StartIdeaSubSection()
        {
            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            tmp.LineEnd();

            tmp.InsertTextAndMoveCursor(IdeaSubSectionText());
            tmp.GotoLine(tmp.GetCurrentLineNumber() + 3);
        }

        internal static void IndentBulletOne()
        {
            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            tmp.InsertTextAndMoveCursor(IndentText(1));
            tmp.SetAnchor(tmp.GetAnchor() + 2);
            tmp.SetCurrentPos(tmp.GetCurrentPos() + 2);
        }

        internal static void IndentBulletTwo()
        {
            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            tmp.InsertTextAndMoveCursor(IndentText(2));
            tmp.SetAnchor(tmp.GetAnchor() + 2);
            tmp.SetCurrentPos(tmp.GetCurrentPos() + 2);
        }

        internal static void IndentBulletThree()
        {
            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            tmp.InsertTextAndMoveCursor(IndentText(3));
            tmp.SetAnchor(tmp.GetAnchor() + 1);
            tmp.SetCurrentPos(tmp.GetCurrentPos() + 1);
        }

        internal static void IndentBulletFour()
        {
            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            tmp.InsertTextAndMoveCursor(IndentText(4));
            tmp.SetAnchor(tmp.GetAnchor() + 2);
            tmp.SetCurrentPos(tmp.GetCurrentPos() + 2);
        }

        internal static void IndentBulletFive()
        {
            ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            tmp.InsertTextAndMoveCursor(IndentText(5));
            tmp.SetAnchor(tmp.GetAnchor() + 2);
            tmp.SetCurrentPos(tmp.GetCurrentPos() + 2);
        }

        internal static void ShowSettingDialog()
        {
            frmSettings dlg = new frmSettings();
            dlg.ShowDialog();
            dlg = null;
        }

        internal static void myDockableDialog_RoutineSuspended()
        {
            if (frmMyDlg == null)
            {
                frmMyDlg = new frmSettings();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = frmMyDlg.Handle;
                _nppTbData.pszName = "My dockable dialog";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, frmMyDlg.Handle);
            }
        }
        #endregion

        #region "Common Routines"
        static string CreateUniqueId()
        {
            string strNow = DateTime.Now.ToString("dd-MMM-yy_hh.mm.ss_tt").ToUpper();
            return string.Format("{0}_{1:N}", strNow, Guid.NewGuid());
        }

        internal static string DayStartText()
        {
            string strNow = DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt").ToUpper();
            StringBuilder strb = new StringBuilder();

            strb.AppendLine("█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█");
            strb.AppendLine("█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
            strb.AppendLine("█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
            strb.AppendLine("█▐■ " + strNow + " ■▌▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
            strb.AppendLine("█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
            strb.AppendLine("█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
            strb.AppendLine("█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█");
            strb.AppendLine("");
            strb.AppendLine("╔════════════════════════════════════════════════════════════════════════════════════════════════╗");
            strb.AppendLine("╚══════< What's in your Mind >══════╝");
            strb.AppendLine("");

            return strb.ToString();
        }

        internal static string NewIdeaText()
        {
            string strNow = DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt").ToUpper();
            StringBuilder strb = new StringBuilder();
            strb.AppendLine("ʅ‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗‗ʃ");
            strb.AppendLine("                ╚═══════════════════════════════════════════════════════════╝                     ");
            strb.AppendLine("                ▓╠▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒ " + strNow + " ▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒╣▓                     ");
            strb.AppendLine("╔════════════════════════════════════════════════════════════════════════════════════════════════╗");
            strb.AppendLine("╚══════< What's in your Mind >══════╝");

            return strb.ToString();
        }

        internal static string IdeaSubSectionText()
        {
            StringBuilder strb = new StringBuilder();
            strb.AppendLine(Environment.NewLine + Environment.NewLine);
            strb.AppendLine("    ﴾────────────────────────────────────────────────────────────────────────────────────────﴿   ");
            strb.AppendLine(Environment.NewLine);

            return strb.ToString();
        }

        internal static string IndentText(int Bullet)
        {
            // » → ► ☼ ›
            switch (Bullet)
            {
                case 1:
                    return "  ► ";
                case 2:
                    return "  → ";
                case 3:
                    return "  » ";
                case 4:
                    return "  › ";
                case 5:
                    return "  ☼ ";
                default:
                    return "";
            }
        }

        public static List<string> AllNotebooks()
        {
            DirectoryInfo di = new DirectoryInfo(NotebookFolder);

            List<string> getAllFiles = di.GetFiles()
                                .Select(file => file.Name).ToList();

            //List getAllCSVFiles = di.GetFiles("*.csv")
            //                    .Where(file => file.Name.EndsWith(".csv"))
            //                    .Select(file => file.Name).ToList();

            //return Directory.GetFiles(NotebookFolder);
            return getAllFiles;
        }

        public static List<string> SearchStringInAllNotebooks(string searchtext)
        {
            List<string> getAllFiles = new List<string>();

            // Take a snapshot of the file system.  
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(NotebookFolder);

            // This method assumes that the application has discovery permissions  
            // for all folders under the specified path.  
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            // Search the contents of each file.  
            // A regular expression created with the RegEx class  
            // could be used instead of the Contains method.  
            // queryMatchingFiles is an IEnumerable<string>.  
            var queryMatchingFiles =
                from file in fileList
                //where file.Extension == "*.*"
                let fileText = GetFileText(file.FullName)
                where fileText.Contains(searchtext)
                select file.Name;

            foreach (string filename in queryMatchingFiles)
            {
                getAllFiles.Add(filename);
            }

            return getAllFiles;
        }

        // Read the contents of the file.  
        static string GetFileText(string name)
        {
            string fileContents = String.Empty;

            // If the file has been deleted since we took
            // the snapshot, ignore it and return the empty string.  
            if (System.IO.File.Exists(name))
            {
                fileContents = System.IO.File.ReadAllText(name);
            }
            return fileContents;
        }
        #endregion
    }
}






//strb.AppendLine("╔================================================================================================╗");
//strb.AppendLine("╠≡≡╖╓≡≡≡≡≡╖╓≡≡≡≡≡╖╓≡≡≡≡≡≡╖╓≡≡≡≡≡≡╖╓≡≡≡≡≡╖╓≡≡≡≡≡╖╓≡≡≡≡≡≡╖╓≡≡≡≡≡≡╖╓≡≡≡≡≡╖╓≡≡≡≡≡╖╓≡≡≡≡≡≡╖╓≡≡≡≡≡≡╖╓≡≡╣");
//strb.AppendLine("╚================================================================================================╝");
//tmp.AddText(strb.Length, strb.ToString());
//tmp.InsertText(tmp.GetCurrentPos(), strb.ToString());
//strb.AppendLine("╔================================================================================================╗");
//strb.AppendLine("╫────────────────────────────────────────────────────────────────────────────────────────────────╫");
//strb.AppendLine("﴾════════════════════════════════════════════════════════════════════════════════════════════════﴿");
//strb.AppendLine("﴾≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡﴿");





//internal static void StartTodaysNotes1()
//{
//    string strNow = DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt").ToUpper();
//    ScintillaGateway tmp = new ScintillaGateway(PluginBase.GetCurrentScintilla());

//    tmp.LineEnd();
//    tmp.NewLine();
//    tmp.NewLine();

//    StringBuilder strb = new StringBuilder();
//    strb.AppendLine("█▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀█");
//    strb.AppendLine("█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
//    strb.AppendLine("█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
//    strb.AppendLine("█▐■ " + strNow + " ■▌▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
//    strb.AppendLine("█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
//    strb.AppendLine("█▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒█");
//    strb.AppendLine("█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄█");
//    tmp.NewLine();
//    tmp.NewLine();
//    strb.AppendLine("╔════════════════════════════════════════════════════════════════════════════════════════════════╗");
//    tmp.NewLine();
//    tmp.NewLine();

//    //tmp.AddText(strb.Length, strb.ToString());
//    //tmp.InsertText(tmp.GetCurrentPos(), strb.ToString());
//    tmp.InsertTextAndMoveCursor(strb.ToString());

//}

