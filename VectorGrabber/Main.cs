﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;
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

            if (!Directory.Exists(CsharpFileDirectory)) { Directory.CreateDirectory(CsharpFileDirectory); }

            using (FileStream fs = new FileStream(CsharpFilePath,FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (!File.Exists(CsharpFilePath))
                {
                    File.Create(CsharpFilePath);
                }
                else FileHelper.ReadFile();
            }

            while (true)
            {
                GameFiber.Yield();



                ////////TEMP

                if (Game.IsKeyDownRightNow(Keys.LShiftKey) && Game.IsKeyDown(Keys.L))
                {
                    foreach (Blip blip in World.GetAllBlips())
                    {
                        if (blip.Exists()) { blip.Delete(); }
                    }
                }

                ////////TEMP


                if (Player.IsValid() && Game.IsKeyDown(Settings.SaveKey) && HelperMethods.CheckModifierKey())
                {
                    string locationTitle;
                    FileHelper.AppendToFile(HelperMethods.getCoordsAndFormat(out locationTitle,Player),CsharpFilePath);
                    FileHelper.AddVectorAndHeadingToList(locationTitle,Player);
                    Game.DisplayNotification("Coordinates were saved to text file.");
                }

                if (Player.IsValid() && Game.IsKeyDown(Settings.TeleportNextKey) && HelperMethods.CheckModifierKey())
                {
                    TeleportHelper.HandleArrow(TeleportHelper.direction.RIGHT);
                }

                if (Player.IsValid() && Game.IsKeyDown(Settings.TeleportBackKey) && HelperMethods.CheckModifierKey())
                {
                    TeleportHelper.HandleArrow(TeleportHelper.direction.LEFT);
                }

                if (Player.IsValid() && Game.IsKeyDown(Settings.TeleportKey) && HelperMethods.CheckModifierKey())
                {
                    TeleportHelper.TeleportToSpecificCoordinate(Player);
                }

                if (Player.IsValid() && Game.IsKeyDown(Settings.RereadFile) && HelperMethods.CheckModifierKey())
                {
                   FileHelper.RereadFile();
                }

                if (Player.IsValid() && Game.IsKeyDown(Settings.ClipboardKey) && HelperMethods.CheckModifierKey())
                {
                    FileHelper.CopyCurrCoordToClipboard();
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