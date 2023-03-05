using System.Drawing;
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
                EntryPoint.RereadFile();
            }
            else if (selectedItem.Equals(CopyClipboard))
            {
                EntryPoint.CopyCurrCoordToClipboard();
            }
            else if (selectedItem.Equals(AddLocation))
            {
                EntryPoint.AddLocation();
            }
        }
        
        internal static void OnBlipCheckboxEvent(UIMenuCheckboxItem sender, bool IsChecked)
        {
            if (IsChecked)
            {
                AddBlips();
            }
            else
            {
                DeleteBlips();
            }
        }
        
        internal static void AddBlips()
        {
            foreach (SavedLocation s in EntryPoint.VectorsRead)
            {
                Blip newBlip = new Blip(new Vector3(s.X, s.Y, s.Z));
                newBlip.Color = Color.Green; 
                newBlip.Name = s.Title;
                EntryPoint.Blips.Add(newBlip);
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
            EntryPoint.Blips.Add(newBlip);
        }
        internal static void DeleteBlips()
        {
            foreach (Blip blip in EntryPoint.Blips)
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

                if (Game.IsKeyDown(Settings.MenuKey) && Game.IsControlKeyDownRightNow && !UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
                {
                    mainMenu.Visible = true;
                }
            }
        }
    }
    
    
    
}   