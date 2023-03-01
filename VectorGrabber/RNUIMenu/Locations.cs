using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;

namespace VectorGrabber
{
    public class Locations
    {
        internal static UIMenu LocationMenu = new UIMenu("Locations", "Select Option");
        internal static UIMenuItem ShowAllLocations = new UIMenuItem("Saved Locations", "List of Saved Locations");

        internal static void setupLocationMenu()
        {
            Menu.mainMenu.AddItem(ShowAllLocations);
            Menu.mainMenu.BindMenuToItem(LocationMenu,ShowAllLocations);
            LocationMenu.ParentMenu = Menu.mainMenu;
            AddItems();
            Menu.pool.Add(LocationMenu);
            

            LocationMenu.OnItemSelect += OnLocationSelect;
            LocationMenu.MouseControlsEnabled = false;
            LocationMenu.AllowCameraMovement = true;
        }

        internal static void AddItems()
        {
            foreach (SavedLocation s in EntryPoint.VectorsRead)
            {
                LocationMenu.AddItem(new UIMenuItem($"{s.Title}",$"x: {s.X} | y: {s.Y} | z: {s.Z} | heading: {s.Heading}")); 
                
            }
        }

        internal static void OnLocationSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            EntryPoint.TeleportBasedOnIndexAndDisplay(index);
        }
    }
}