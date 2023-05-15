using System;
using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using static VectorGrabber.FileHelper;

namespace VectorGrabber
{
    public class DeleteLocations
    {
        internal static UIMenu DeleteLocationMenu = new UIMenu("Locations", "Select Option");
        internal static UIMenuItem LocationsThatCanBeDeleted = new UIMenuItem("~r~Delete a location", "Delete any of your saved locations");
        
        internal static void SetupDeleteLocationMenu()
        {
            Menu.mainMenu.AddItem(LocationsThatCanBeDeleted);
            Menu.mainMenu.BindMenuToItem(DeleteLocationMenu, LocationsThatCanBeDeleted);
            DeleteLocationMenu.ParentMenu = Menu.mainMenu;
            Menu.menuPool.Add(DeleteLocationMenu);
                
            DeleteLocationMenu.OnItemSelect += OnDeleteLocationSelect;
            DeleteLocationMenu.MouseControlsEnabled = false;
            DeleteLocationMenu.AllowCameraMovement = true;
        }

        internal static void OnDeleteLocationSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            try
            {
                if (DeleteLocationMenu.MenuItems.Count == 1)
                {
                    DeleteLocationMenu.Clear();
                    Locations.LocationMenu.Clear();
                }
                else
                {
                    DeleteLocationMenu.RemoveItemAt(index);
                    Locations.LocationMenu.RemoveItemAt(index);
                }
                
                AppendToFile(HelperMethods.GetCoordsAndFormat(VectorsRead[index]), DeletedVectors);
                VectorsRead.RemoveAt(index);
                Blips.RemoveAt(index);

                Menu.DeleteBlips();
                Menu.AddBlips();
            }
            catch (Exception ex)
            {
                Game.LogTrivial(ex.ToString());
            }
        }
    }
}