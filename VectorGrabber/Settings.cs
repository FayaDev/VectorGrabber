using System.Runtime.CompilerServices;
using Rage;
using System.Windows.Forms;


namespace VectorGrabber
{
    internal class Settings
    {
        internal static Keys SaveKey = Keys.Y;
        internal static Keys NextKey = Keys.Right;
        internal static Keys BackKey = Keys.Left;
        internal static Keys TeleportKey = Keys.T;
        internal static Keys ModifierKey = Keys.Control;
        internal static Keys RereadFile = Keys.R;
        internal static Keys ClipboardKey = Keys.C;
        internal static InitializationFile iniFile;

        internal void Initialize()
        {
            try
            {
                iniFile = new InitializationFile(@"Plugins/VectorGrabber.ini");
                iniFile.Create();
                SaveKey = iniFile.ReadEnum("Keybinds", "Savekey",SaveKey);
                NextKey = iniFile.ReadEnum("Keybinds", "NextKey", NextKey);
                BackKey = iniFile.ReadEnum("Keybinds", "BackKey", BackKey);
                TeleportKey = iniFile.ReadEnum("Keybinds", "TeleportKey", TeleportKey);
                ModifierKey = iniFile.ReadEnum("Keybinds", "ModifierKey", ModifierKey);
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