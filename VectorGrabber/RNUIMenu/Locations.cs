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
            Menu.pool.Add(LocationMenu);
            LocationMenu.ParentMenu = Menu.mainMenu;
            LocationMenu.OnItemSelect += OnLocationSelect;
        }

        internal static void AddItems()
        {
            foreach (SavedLocation s in EntryPoint.VectorsRead)
            {
                LocationMenu.AddItem(new UIMenuItem($"{s.title}")); //TODO: Add vector and heading to description
            }
        }

        internal static void OnLocationSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            EntryPoint.TeleportBasedOnIndexAndDisplay(index);
        }
    }
}