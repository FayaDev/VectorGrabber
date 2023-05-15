using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;
using static VectorGrabber.FileHelper;

namespace VectorGrabber
{
    internal class Menu
    {
        private static readonly string versionState = VersionChecker.PluginUpToDate ? "(~g~Latest~s~)" : "(~r~Outdated~s~)";

        internal static MenuPool menuPool;
        internal static UIMenu mainMenu;
        internal static UIMenuItem ClearFile = new UIMenuItem("Clear File", "Clears files of all vectors");
        internal static UIMenuItem UpdateTextFile = new UIMenuItem("~y~Update Text File", "Updates text file. Should be used after making a lot of deletions");
        internal static UIMenuItem RereadFile = new UIMenuItem("Reread file", "Rereads file and updates menu"); 
        internal static UIMenuCheckboxItem EnableBlips = new UIMenuCheckboxItem("Enable Blips", Settings.EnableVectorBlips, "Enables blips for all saved vectors");
        internal static UIMenuItem CopyClipboard = new UIMenuItem("Copy Coordinates", "Copies current player's coordinate to user's computer clipboard");
        internal static UIMenuItem AddLocation = new UIMenuItem("Add Location", "Adds current location to saved locations");
        internal static UIMenuItem MakeCopyOfFile = new UIMenuItem("Make Copy", "Makes copy of text file");

        internal static void CreateMainMenu()
        {
            menuPool = new MenuPool();
            mainMenu = new UIMenu("VectorGrabber", $"Main Menu - ~y~v{VersionChecker.CurrentVersion}~s~ {versionState}");

            mainMenu.AddItems(EnableBlips, RereadFile, MakeCopyOfFile, ClearFile, CopyClipboard, AddLocation, UpdateTextFile);
            menuPool.Add(mainMenu);

            mainMenu.AllowCameraMovement = true;
            mainMenu.MouseControlsEnabled = false;
            
            mainMenu.OnItemSelect += MainMenuItemSelect;
            EnableBlips.CheckboxEvent += OnBlipCheckboxEvent;

            Locations.setupLocationMenu();
            DeleteLocations.SetupDeleteLocationMenu();

            GameFiber.StartNew(ProcessMenus);
        }

        internal static void MainMenuItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            try
            {
                if (selectedItem.Equals(RereadFile))
                {
                    RereadFile();
                }
                else if (selectedItem.Equals(CopyClipboard))
                {
                    CopyCurrCoordToClipboard();
                }
                else if (selectedItem.Equals(AddLocation))
                {
                    AddLocation();
                }
                else if (selectedItem.Equals(ClearFile))
                {
                    ClearFile();
                }
                else if (selectedItem.Equals(UpdateTextFile))
                {
                    UpdateTextFile();
                }
                else if (selectedItem.Equals(MakeCopyOfFile))
                {
                    CopyFile();
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial(ex.ToString());
            }
        }
        
        internal static void OnBlipCheckboxEvent(UIMenuCheckboxItem sender, bool IsChecked)
        {
            if (IsChecked)
            {
                AddBlips();
                Settings.EnableVectorBlips = true;
            }
            else
            {
                DeleteBlips();
                Settings.EnableVectorBlips = false;
            }
        }

        internal static void ToggleAccessToLocations()
        {
            if (DeleteLocations.LocationsThatCanBeDeleted.Enabled && Locations.ShowAllLocations.Enabled)
            {
                DeleteLocations.LocationsThatCanBeDeleted.Enabled = false;
                Locations.ShowAllLocations.Enabled = false;
            }
            else
            {
                DeleteLocations.LocationsThatCanBeDeleted.Enabled = true;
                Locations.ShowAllLocations.Enabled = true;
            }
        }

        internal static void AddBlips()
        {
            foreach (SavedLocation s in VectorsRead)
            {
                Blip newBlip = new Blip(new Vector3(s.X, s.Y, s.Z));
                newBlip.Color = Color.Green; 
                newBlip.Name = s.Title;

                if (!EnableBlips.Checked) { newBlip.Alpha = 0f; }

                Blips.Add(newBlip);
            }
        }

        internal static void AddBlip(SavedLocation s)
        {
            Blip newBlip = new Blip(new Vector3(s.X, s.Y, s.Z));
            newBlip.Color = Color.Green; 
            newBlip.Name = s.Title;

            if (!EnableBlips.Checked) { newBlip.Alpha = 0f; }

            Blips.Add(newBlip);
        }

        internal static void DeleteBlips()
        {
            foreach (Blip blip in Blips)
            {
                if (blip.Exists())
                {
                    blip.Delete();
                }
            }
        }
        
        private static void ProcessMenus()
        {
            // Draw the menu banners (only needed if UIMenu.SetBannerType(Rage.Texture) is used)
            // Game.RawFrameRender += (s, e) => pool.DrawBanners(e.Graphics);

            while (true)
            {
                GameFiber.Yield();

                menuPool.ProcessMenus();

                if (HelperMethods.CheckModifierKey() && Game.IsKeyDown(Settings.MenuKey) && !TabView.IsAnyPauseMenuVisible)
                {
                    mainMenu.Visible = !mainMenu.Visible;
                }
            }
        }
    }
}   