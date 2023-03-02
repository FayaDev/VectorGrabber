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
        
        
        internal static void CreateMainMenu()
        {
            pool = new MenuPool();
            mainMenu = new UIMenu("VectorGrabber", "Main Menu");
            mainMenu.AddItem(RereadFile);
            
            
            mainMenu.AllowCameraMovement = true;
            mainMenu.MouseControlsEnabled = false;

            mainMenu.OnItemSelect += mainMenuItemSelect;
            
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