using System.Runtime.CompilerServices;
using Rage;
using System.Windows.Forms;


namespace Vector3Grabber
{
    internal class Settings
    {
        internal static Keys SaveKey = Keys.Y;
        internal static Keys NextKey = Keys.Right;
        internal static Keys BackKey = Keys.Left;

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