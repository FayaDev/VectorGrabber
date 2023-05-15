using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;
using static VectorGrabber.FileHelper;

namespace VectorGrabber
{
    public class DeleteLocations
    {
        internal static UIMenu DeleteLocationMenu = new UIMenu("Locations", "Select Option");
        internal static UIMenuItem LocationsThatCanBeDeleted = new UIMenuItem("~r~Delete location", "Delete any of your saved locations");
        
        internal static void SetupDeleteLocationMenu()
        {
            Menu.mainMenu.AddItem(LocationsThatCanBeDeleted);
            Menu.mainMenu.BindMenuToItem(DeleteLocationMenu,LocationsThatCanBeDeleted);
            DeleteLocationMenu.ParentMenu = Menu.mainMenu;
            Menu.menuPool.Add(DeleteLocationMenu);
                
            DeleteLocationMenu.OnItemSelect += OnDeleteLocationSelect;
            DeleteLocationMenu.MouseControlsEnabled = false;
            DeleteLocationMenu.AllowCameraMovement = true;
        }

        internal static void AddItems()
        {
            foreach (SavedLocation s in VectorsRead)
            {
                DeleteLocationMenu.AddItem(new UIMenuItem($"{s.Title}",$"x: {s.X} | y: {s.Y} | z: {s.Z} | heading: {s.Heading}")); 
            }

            if (Settings.EnableVectorBlips)
            {
                Menu.AddBlips();
            }
        }
        
        internal static void AddItem(SavedLocation s)
        {
            DeleteLocationMenu.AddItem(new UIMenuItem($"{s.Title}",$"x: {s.X} | y: {s.Y} | z: {s.Z} | heading: {s.Heading}"));
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
                
                AppendToFile(HelperMethods.GetCoordsAndFormat(VectorsRead[index]),DeletedVectors);
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