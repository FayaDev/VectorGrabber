using System;
using System.Drawing;
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
        internal static UIMenuItem ShowAllLocations = new UIMenuItem("Teleport to location", "Teleport to any of your saved locations");
       
        internal static void setupLocationMenu()
        {
            Menu.mainMenu.AddItem(ShowAllLocations);
            Menu.mainMenu.BindMenuToItem(LocationMenu,ShowAllLocations);
            LocationMenu.ParentMenu = Menu.mainMenu;
            Menu.pool.Add(LocationMenu);
            

            LocationMenu.OnItemSelect += OnLocationSelect;
            LocationMenu.MouseControlsEnabled = false;
            LocationMenu.AllowCameraMovement = true;
        }

        internal static void AddItems()
        {
            foreach (SavedLocation s in SavedLocationList.VectorsRead)
            {
                LocationMenu.AddItem(new UIMenuItem($"{s.Title}",$"x: {s.X} | y: {s.Y} | z: {s.Z} | heading: {s.Heading}")); 
                
            }

            if (Settings.EnableVectorBlips)
            {
                Menu.AddBlips();
            }
        }
        
        internal static void AddItem(SavedLocation s)
        {
            LocationMenu.AddItem(new UIMenuItem($"{s.Title}",$"x: {s.X} | y: {s.Y} | z: {s.Z} | heading: {s.Heading}"));
        }

        

        internal static void OnLocationSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            TeleportHelper.TeleportBasedOnIndexAndDisplay(index,EntryPoint.Player);
        }
    }
}