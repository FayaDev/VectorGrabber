using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

[assembly: Rage.Attributes.Plugin("VectorGrabber", Description = "Helps developers find locations for callouts/ambient events", Author = "Roheat")]
namespace VectorGrabber
{
    internal static class EntryPoint
    {
        internal static Ped Player => Game.LocalPlayer.Character;

        internal static string CsharpFilePath = @"Plugins\VectorGrabber\VectorsInCsharpNotation.txt";
        internal static string CsharpFileDirectory = @"Plugins\VectorGrabber\";
        
        internal static void Main()
        {
            GameFiber.StartNew(Menu.CreateMainMenu);
            VersionChecker.CheckForUpdates();
            Settings.Initialize();
            if (!Directory.Exists(CsharpFileDirectory))
            {
                Directory.CreateDirectory(CsharpFileDirectory);
            }
            if (!File.Exists(CsharpFilePath))
            {
                File.Create(CsharpFilePath);
            }
            else
            {
                SavedLocationList.ReadFile();
            }
            while (true)
            {
                GameFiber.Yield();
                if (Player.IsValid() &&Game.IsKeyDown(Settings.SaveKey) && HelperMethods.CheckModifierKey())
                {
                    string locationTitle;
                    SavedLocationList.AppendToFile(HelperMethods.getCoordsAndFormat(out locationTitle,Player),CsharpFilePath);
                    SavedLocationList.AddVectorAndHeadingToList(locationTitle,Player);
                    Game.DisplayNotification("Coordinates were saved to text file.");
                }

                if (Player.IsValid()&& Game.IsKeyDown(Settings.TeleportNextKey) && HelperMethods.CheckModifierKey())
                {
                    TeleportHelper.HandleArrow(TeleportHelper.direction.RIGHT);
                }

                if (Player.IsValid()&&Game.IsKeyDown(Settings.TeleportBackKey) && HelperMethods.CheckModifierKey())
                {
                    TeleportHelper.HandleArrow(TeleportHelper.direction.LEFT);
                }

                if (Player.IsValid() && Game.IsKeyDown(Settings.TeleportKey) && HelperMethods.CheckModifierKey())
                {
                    TeleportHelper.TeleportToSpecificCoordinate(Player);
                }
                if (Player.IsValid() && Game.IsKeyDown(Settings.RereadFile) && HelperMethods.CheckModifierKey())
                {
                   SavedLocationList.RereadFile();
                }
                if (Player.IsValid() && Game.IsKeyDown(Settings.ClipboardKey) && HelperMethods.CheckModifierKey())
                {
                    SavedLocationList.CopyCurrCoordToClipboard();
                    
                }
            }
        }

        internal static void OnUnload(bool Exit)
        {
            Menu.DeleteBlips();
            Settings.UpdateINI();
            Game.LogTrivial("Vector Grabber Unloaded.");
        }
        
    }
}