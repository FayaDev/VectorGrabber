using System.Runtime.CompilerServices;
using Rage;
using System.Windows.Forms;


namespace VectorGrabber
{
    internal static class Settings
    {
        internal static Keys SaveKey = Keys.Y;
        internal static Keys TeleportNextKey = Keys.Right;
        internal static Keys TeleportBackKey = Keys.Left;
        internal static Keys TeleportKey = Keys.T;
        internal static Keys RereadFile = Keys.R;
        internal static Keys ClipboardKey = Keys.C;
        internal static Keys MenuKey = Keys.M;
        internal static InitializationFile iniFile;

        internal static void Initialize()
        {
            try
            {
                iniFile = new InitializationFile(@"Plugins/VectorGrabber.ini");
                iniFile.Create();
                SaveKey = iniFile.ReadEnum("Keybinds", "Savekey",SaveKey);
                TeleportNextKey = iniFile.ReadEnum("Keybinds", "NextKey", TeleportNextKey);
                TeleportBackKey = iniFile.ReadEnum("Keybinds", "BackKey", TeleportBackKey);
                TeleportKey = iniFile.ReadEnum("Keybinds", "TeleportKey", TeleportKey);
                MenuKey = iniFile.ReadEnum("Keybinds", "MenuKey", MenuKey);
                RereadFile = iniFile.ReadEnum("Keybinds", "RereadFile", RereadFile);
                ClipboardKey = iniFile.ReadEnum("Keybinds", "ClipboardKey", ClipboardKey);
            }
            catch(System.Exception e)
            {
                string error = e.ToString();
                Game.LogTrivial("Vector Grabber: ERROR IN 'Settings.cs, Initialize()': " + error);
                Game.DisplayNotification("Vector Grabber: Error Occured");
            }
        }

    }
};