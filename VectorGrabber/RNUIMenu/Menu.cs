using System;
using System.Drawing;
using Rage;
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

        internal static UIMenuListScrollerItem<string> FileActions = new UIMenuListScrollerItem<string>("~y~File Actions", "", new[] { "~g~Save Changes", "~p~Re-read", "~r~Clear File", "~o~Make Backup" });
        internal static UIMenuCheckboxItem EnableBlips = new UIMenuCheckboxItem("Enable Blips", Settings.EnableVectorBlips, "Enables blips for all saved vectors");
        internal static UIMenuItem CopyClipboard = new UIMenuItem("Copy Coordinates", "Copies current player's coordinate to user's computer clipboard");
        internal static UIMenuItem AddLocation = new UIMenuItem("Add Location", "Adds current location to saved locations");

        internal static void CreateMainMenu()
        {
            menuPool = new MenuPool();
            mainMenu = new UIMenu("VectorGrabber", $"Main Menu - ~y~v{VersionChecker.CurrentVersion}~s~ {versionState}");

            menuPool.Add(mainMenu);
            mainMenu.AddItems(EnableBlips, CopyClipboard, AddLocation, FileActions);

            mainMenu.AllowCameraMovement = true;
            mainMenu.MouseControlsEnabled = false;
            FileActions.Description = "Should be used after making a lot of deletions.";

            mainMenu.OnItemSelect += MainMenuItemSelect;
            FileActions.Activated += FileActionsItemActivated;
            EnableBlips.CheckboxEvent += OnBlipCheckboxEvent;

            FileActions.IndexChanged += (sender, menu, item) =>
            {
                switch (FileActions.SelectedItem.Remove(0, 3))
                {
                    case "Save Changes":
                        FileActions.Description = "Should be used after making a lot of deletions.";
                        break;
                    case "Re-read":
                        FileActions.Description = "Rereads file and updates menu.";
                        break;
                    case "Clear File":
                        FileActions.Description = "Clears files of all vectors.";
                        break;
                    case "Make Backup":
                        FileActions.Description = "Makes a backup of the file.";
                        break;
                };

            };

            Locations.SetupLocationMenu();
            DeleteLocations.SetupDeleteLocationMenu();

            GameFiber.StartNew(ProcessMenus);
        }

        internal static void MainMenuItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            try
            {                
                if (selectedItem.Equals(CopyClipboard))
                {
                    CopyCurrCoordToClipboard();
                }
                else if (selectedItem.Equals(AddLocation))
                {
                    AddLocation();
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial(ex.ToString());
            }
        }

        internal static void FileActionsItemActivated(UIMenu sender, UIMenuItem selectedItem)
        {
            try
            {
                switch (FileActions.SelectedItem.Remove(0, 3))
                {
                    case "Save Changes":
                        UpdateTextFile();
                        break;
                    case "Re-read":
                        RereadFile();
                        break;
                    case "Clear File":
                        ClearFile();
                        break;
                    case "Make Backup":
                        CopyFile();
                        break;
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