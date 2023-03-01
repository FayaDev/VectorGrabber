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
        
        
        
        internal static void CreateMainMenu()
        {
            pool = new MenuPool();

            mainMenu = new UIMenu("VectorGrabber", "Main Menu");
            
            pool.Add(mainMenu);

            GameFiber.StartNew(ProcessMenus);


        }
        
        
        private static void ProcessMenus()
        {
            // draw the menu banners (only needed if UIMenu.SetBannerType(Rage.Texture) is used)
            // Game.RawFrameRender += (s, e) => pool.DrawBanners(e.Graphics);

            while (true)
            {
                GameFiber.Yield();

                pool.ProcessMenus();

                if (Game.IsKeyDown(Keys.Decimal) && !UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
                {
                    mainMenu.Visible = true;
                }
            }
        }
    }
    
    
    
}   