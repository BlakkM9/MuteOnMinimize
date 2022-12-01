using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuteOnMinimize.Utilities
{
    public static class Utils

    {

        public static readonly string AUTOSTART_PATH = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        public static void CreateStartupShortcut(string shortcutName, string executablePath)
        {
            WshShell wsh = new WshShell();
            IWshShortcut shortcut = wsh.CreateShortcut($"{AUTOSTART_PATH}\\{shortcutName}.lnk") as IWshShortcut;

            shortcut.TargetPath = executablePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(executablePath);
            shortcut.Description = $"{shortcutName} - Startup Shortcut";
            shortcut.IconLocation = shortcut.WorkingDirectory + "\\" + shortcutName + ".ico";
            shortcut.Save();
        }


        public static void RemoveStartupShortcut(string shortcutName, string executablePath)
        {
            System.IO.File.Delete($"{AUTOSTART_PATH}\\{shortcutName}.lnk");
        }
    }
}
