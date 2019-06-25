using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GetFileHash
{
    public class MRUManager
    {
        #region Private variables
        private readonly string NameOfProgram;
        private readonly string SubKeyName;
        private readonly Action<object, EventArgs> OnRecentFileClick;
        private readonly Action<object, EventArgs> OnClearRecentFilesClick;
        private readonly ToolStripMenuItem ParentMenuItem;
        #endregion

        /// <summary>
        /// Class constructor for the MRUManager class
        /// </summary>
        /// <param name="parentMenuItem">The name of the parent menu item.</param>
        /// <param name="nameOfProgram">The name of the program which is used to determine the registry location for the MRU list.</param>
        /// <param name="onRecentFileClick">Event handler for adding a file to the list.</param>
        /// <param name="onClearRecentFilesClick">Event handler for removing files from the list.</param>
        /// <exception cref="ArgumentException">If anything is null or nameOfProgram contains a backward slash or is empty.</exception>
        public MRUManager(ToolStripMenuItem parentMenuItem, string nameOfProgram, Action<object, EventArgs> onRecentFileClick, Action<object, EventArgs> onClearRecentFilesClick = null)
        {
            if (parentMenuItem == null || string.IsNullOrEmpty(nameOfProgram) || nameOfProgram.Contains("\\") || onRecentFileClick == null)
            {
                throw new ArgumentException("Bad argument.");
            }

            ParentMenuItem = parentMenuItem;
            NameOfProgram = nameOfProgram;
            OnRecentFileClick = onRecentFileClick;
            OnClearRecentFilesClick = onClearRecentFilesClick;
            SubKeyName = string.Format("Software\\{0}\\MRU", NameOfProgram);

            RefreshRecentFilesMenu();
        }

        #region Private members
        private void OnClearRecentFiles_Click(object obj, EventArgs evt)
        {
            try
            {
                RegistryKey rK = Registry.CurrentUser.OpenSubKey(SubKeyName, true);
                if (rK == null)
                {
                    return;
                }
                string[] values = rK.GetValueNames();
                foreach (string valueName in values)
                {
                    rK.DeleteValue(valueName, true);
                }
                rK.Close();
                ParentMenuItem.DropDownItems.Clear();
                ParentMenuItem.Enabled = false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            OnClearRecentFilesClick?.Invoke(obj, evt);
        }

        private void RefreshRecentFilesMenu()
        {
            RegistryKey rK;
            string s;
            ToolStripItem tSI;

            try
            {
                rK = Registry.CurrentUser.OpenSubKey(SubKeyName, false);
                if (rK == null)
                {
                    ParentMenuItem.Enabled = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Cannot open recent files registry key:\n" + ex.ToString());
                return;
            }

            ParentMenuItem.DropDownItems.Clear();
            string[] valueNames = rK.GetValueNames();
            foreach (string valueName in valueNames)
            {
                s = rK.GetValue(valueName, null) as string;
                if (s == null)
                {
                    continue;
                }
                tSI = ParentMenuItem.DropDownItems.Add(s);
                tSI.Click += new EventHandler(OnRecentFileClick);
            }

            if (ParentMenuItem.DropDownItems.Count == 0)
            {
                ParentMenuItem.Enabled = false;
                return;
            }

            ParentMenuItem.DropDownItems.Add("-");
            tSI = ParentMenuItem.DropDownItems.Add("Clear list");
            tSI.Click += new EventHandler(OnClearRecentFiles_Click);
            ParentMenuItem.Enabled = true;
        }
        #endregion

        #region Public members
        public void AddRecentFile(string fileNameWithFullPath)
        {
            string s;

            try
            {
                RegistryKey rK = Registry.CurrentUser.CreateSubKey(SubKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
                for (int i = 0; true; i++)
                {
                    s = rK.GetValue(i.ToString(), null) as string;
                    if (s == null)
                    {
                        rK.SetValue(i.ToString(), fileNameWithFullPath);
                        rK.Close();
                        break;
                    }
                    else if (s == fileNameWithFullPath)
                    {
                        rK.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
            RefreshRecentFilesMenu();
        }

        public void RemoveRecentFile(string fileNameWithFullPath)
        {
            try
            {
                RegistryKey rK = Registry.CurrentUser.OpenSubKey(SubKeyName, true);
                string[] valuesNames = rK.GetValueNames();
                foreach (string valueName in valuesNames)
                {
                    if ((rK.GetValue(valueName, null) as string) == fileNameWithFullPath)
                    {
                        rK.DeleteValue(valueName, true);
                        RefreshRecentFilesMenu();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
            RefreshRecentFilesMenu();
        }
        #endregion
    }
}