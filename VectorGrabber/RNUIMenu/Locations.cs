using RAGENativeUI;
using RAGENativeUI.Elements;

namespace VectorGrabber
{
    public class Locations
    {
        internal static UIMenu LocationMenu = new UIMenu("Locations", "Select Option");
        internal static UIMenuItem ShowAllLocations = new UIMenuItem("Teleport to location", "Teleport to any of your saved locations");
       
        internal static void SetupLocationMenu()
        {
            Menu.mainMenu.AddItem(ShowAllLocations, 3);
            Menu.mainMenu.BindMenuToItem(LocationMenu, ShowAllLocations);
            LocationMenu.ParentMenu = Menu.mainMenu;
            Menu.menuPool.Add(LocationMenu);
            

            LocationMenu.OnItemSelect += OnLocationSelect;
            LocationMenu.MouseControlsEnabled = false;
            LocationMenu.AllowCameraMovement = true;
        }

        internal static void OnLocationSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            TeleportHelper.TeleportBasedOnIndexAndDisplay(index,EntryPoint.Player);
        }
    }
}