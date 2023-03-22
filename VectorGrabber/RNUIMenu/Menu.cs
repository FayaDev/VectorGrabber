using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;


namespace VectorGrabber
{
    internal class Menu
    {
        internal static MenuPool pool;
        internal static UIMenu mainMenu;
        internal static UIMenuItem ClearFile = new UIMenuItem("Clear File", "Clears files of all vectors");
        internal static UIMenuItem MakeCopyOfFile = new UIMenuItem("Copy File", "Saves copy of file");
        internal static UIMenuItem RereadFile = new UIMenuItem("Reread file", "Rereads file and updates menu"); 
        internal static UIMenuCheckboxItem EnableBlips = new UIMenuCheckboxItem("Enable Blips", Settings.EnableVectorBlips,"Enables blips for all saved vectors");
        internal static UIMenuItem CopyClipboard = new UIMenuItem("Copy Coordinates",
            "Copies current player's coordinate to user's computer clipboard");
        internal static UIMenuItem AddLocation =
            new UIMenuItem("Add Location", "Adds current location to saved locations");
        
        internal static void CreateMainMenu()
        {
            pool = new MenuPool();
            mainMenu = new UIMenu("VectorGrabber", "Main Menu");
            mainMenu.AddItem(EnableBlips);
            mainMenu.AddItem(RereadFile);
            mainMenu.AddItem(ClearFile);
            mainMenu.AddItem(CopyClipboard);
            mainMenu.AddItem(AddLocation);
            
            mainMenu.AllowCameraMovement = true;
            mainMenu.MouseControlsEnabled = false;
            
            mainMenu.OnItemSelect += mainMenuItemSelect;
            EnableBlips.CheckboxEvent += OnBlipCheckboxEvent;
            pool.Add(mainMenu);
            Locations.setupLocationMenu();
            GameFiber.StartNew(ProcessMenus);


        }

        internal static void mainMenuItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
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
            else if (selectedItem.Equals(MakeCopyOfFile))
            {
                FileHelper.ClearFile();
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
        
        internal static void AddBlips()
        {
            foreach (SavedLocation s in FileHelper.VectorsRead)
            {
                Blip newBlip = new Blip(new Vector3(s.X, s.Y, s.Z));
                newBlip.Color = Color.Green; 
                newBlip.Name = s.Title;
                FileHelper.Blips.Add(newBlip);
            }
        }
        internal static void AddBlip(SavedLocation s)
        {
            float x = s.X;
            float y = s.Y;
            float z = s.Z;
            Blip newBlip = new Blip(new Vector3(s.X, s.Y, s.Z));
            newBlip.Color = Color.Green; 
            newBlip.Name = s.Title;
            FileHelper.Blips.Add(newBlip);
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
            // draw the menu banners (only needed if UIMenu.SetBannerType(Rage.Texture) is used)
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