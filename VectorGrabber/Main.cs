using System.IO;
using Rage;
using static VectorGrabber.FileHelper;

[assembly: Rage.Attributes.Plugin("VectorGrabber", Description = "Helps developers find locations for callouts/ambient events", Author = "Roheat", PrefersSingleInstance = true)]
namespace VectorGrabber
{
    internal static class EntryPoint
    {
        internal static Ped Player => Game.LocalPlayer.Character;

        internal static void Main()
        {
            GameFiber.StartNew(Menu.CreateMainMenu);
            VersionChecker.CheckForUpdates();
            Settings.Initialize();

            if (!Directory.Exists(CSharpFileDirectory)) { Directory.CreateDirectory(CSharpFileDirectory); }

            using (FileStream fs = new FileStream(CSharpFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (!File.Exists(CSharpFilePath))
                {
                    File.Create(CSharpFilePath);
                }
                else FileHelper.ReadFile();
            }
            using (FileStream fs = new FileStream(DeletedVectors, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                if (!File.Exists(DeletedVectors))
                {
                    File.Create(DeletedVectors);
                }
            }

            while (true)
            {
                GameFiber.Yield();

                if (Player.IsValid() && HelperMethods.CheckModifierKey())
                {
                    if (Game.IsKeyDown(Settings.SaveKey))
                    {
                        FileHelper.AppendToFile(HelperMethods.GetCoordsAndFormat(out string locationTitle, Player), CSharpFilePath);
                        FileHelper.AddVectorAndHeadingToList(locationTitle, Player);
                        Game.DisplayNotification("Coordinates were saved to text file.");
                    }

                    if (Game.IsKeyDown(Settings.TeleportNextKey))
                    {
                        TeleportHelper.HandleArrow(TeleportHelper.Direction.RIGHT);
                    }

                    if (Game.IsKeyDown(Settings.TeleportBackKey))
                    {
                        TeleportHelper.HandleArrow(TeleportHelper.Direction.LEFT);
                    }

                    if (Game.IsKeyDown(Settings.TeleportKey))
                    {
                        TeleportHelper.TeleportToSpecificCoordinate(Player);
                    }

                    if (Game.IsKeyDown(Settings.RereadFile))
                    {
                        FileHelper.RereadFile();
                    }

                    if (Game.IsKeyDown(Settings.ClipboardKey))
                    {
                        FileHelper.CopyCurrCoordToClipboard();
                    }
                }
            }
        }

        internal static void OnUnload(bool Exit)
        {
            Menu.DeleteBlips();
            Settings.UpdateINI();
            HelperMethods.Notify("~y~Unloaded", "Vector Grabber was unloaded.");
            Game.LogTrivial("Vector Grabber Unloaded.");
        }
    }
}