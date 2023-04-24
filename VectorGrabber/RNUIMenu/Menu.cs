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
        internal static MenuPool pool;
        internal static UIMenu mainMenu;
        internal static UIMenuItem ClearFile = new UIMenuItem("Clear File", "Clears files of all vectors");
        internal static UIMenuItem UpdateTextFile = new UIMenuItem("~y~Update Text File",
            "Updates text file. Should be used after making a lot of deletions");
        internal static UIMenuItem RereadFile = new UIMenuItem("Reread file", "Rereads file and updates menu"); 
        internal static UIMenuCheckboxItem EnableBlips = new UIMenuCheckboxItem("Enable Blips", Settings.EnableVectorBlips,"Enables blips for all saved vectors");
        internal static UIMenuItem CopyClipboard = new UIMenuItem("Copy Coordinates",
            "Copies current player's coordinate to user's computer clipboard");
        internal static UIMenuItem AddLocation =
            new UIMenuItem("Add Location", "Adds current location to saved locations");

        internal static UIMenuItem MakeCopyOfFile = new UIMenuItem("Make Copy", "Makes copy of text file");

        internal static void CreateMainMenu()
        {
            pool = new MenuPool();
            mainMenu = new UIMenu("VectorGrabber", "Main Menu");

            mainMenu.AddItems(EnableBlips, RereadFile, MakeCopyOfFile, ClearFile, CopyClipboard, AddLocation);

            mainMenu.AllowCameraMovement = true;
            mainMenu.MouseControlsEnabled = false;
            
            mainMenu.OnItemSelect += MainMenuItemSelect;
            EnableBlips.CheckboxEvent += OnBlipCheckboxEvent;
            pool.Add(mainMenu);
            Locations.setupLocationMenu();
            DeleteLocations.SetupDeleteLocationMenu();
            mainMenu.AddItem(UpdateTextFile);
            GameFiber.StartNew(ProcessMenus);
        }

        internal static void MainMenuItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            try
            {
                if (selectedItem.Equals(RereadFile))
                {
                    FileHelper.RereadFile();
                }
                else if (selectedItem.Equals(CopyClipboard))
                {
                    FileHelper.CopyCurrCoordToClipboard();
                }
                else if (selectedItem.Equals(AddLocation))
                {
                    FileHelper.AddLocation();
                }
                else if (selectedItem.Equals(ClearFile))
                {
                    FileHelper.ClearFile();
                }
                else if (selectedItem.Equals(UpdateTextFile))
                {
                    FileHelper.UpdateTextFile();
                }
                else if (selectedItem.Equals(MakeCopyOfFile))
                {
                    FileHelper.CopyFile();
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
            foreach (SavedLocation s in FileHelper.VectorsRead)
            {
                Blip newBlip = new Blip(new Vector3(s.X, s.Y, s.Z));
                newBlip.Color = Color.Green; 
                newBlip.Name = s.Title;

                FileHelper.Blips.Add(newBlip);
                blipList.Add((newBlip, new Vector3(s.X, s.Y, s.Z)));
            }
        }

        internal static void AddBlip(SavedLocation s)
        {
            Blip newBlip = new Blip(new Vector3(s.X, s.Y, s.Z));
            newBlip.Color = Color.Green; 
            newBlip.Name = s.Title;

            FileHelper.Blips.Add(newBlip);
            blipList.Add((newBlip, new Vector3(s.X, s.Y, s.Z)));
        }

        internal static void DeleteBlips()
        {
            foreach (Blip blip in FileHelper.Blips)
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

                pool.ProcessMenus();

                if (Game.IsKeyDown(Settings.MenuKey) && HelperMethods.CheckModifierKey() && !UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
                {
                    mainMenu.Visible = true;
                }
            }
        }
    }
}   