using System.Runtime.CompilerServices;
using Rage;
using System.Windows.Forms;


namespace Vector3Grabber
{
    internal class Settings
    {
        internal static Keys SaveKey = Keys.Y;
        internal static bool IncludeHeading = true;
        internal static Keys NextKey = Keys.Right;
        internal static Keys BackKey = Keys.Left;
        internal static Keys ModifierKeyForTeleporting = Keys.LControlKey;

        internal static InitializationFile iniFile;

        internal void Initialize()
        {
            try
            {
                iniFile = new InitializationFile(@"Plugins/VectorGrabber.ini");
                iniFile.Create();
                IncludeHeading = iniFile.ReadBoolean("Customization", "IncludeHeading", IncludeHeading);
                SaveKey = iniFile.ReadEnum("Keybinds", "Savekey",SaveKey);
                NextKey = iniFile.ReadEnum("Keybinds", "NextKey", NextKey);
                BackKey = iniFile.ReadEnum("Keybinds", "BackKey", BackKey);
                ModifierKeyForTeleporting = iniFile.ReadEnum("Keybinds", "ModifierKeyForModifiers", ModifierKeyForTeleporting);
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