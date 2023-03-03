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
        internal static UIMenuItem ShowAllLocations = new UIMenuItem("Saved Locations", "List of Saved Locations");
        internal static UIMenuCheckboxItem EnableBlips = new UIMenuCheckboxItem("Enable Blips", Settings.EnableVectorBlips,"Enables blips for all saved vectors"); 
        
        internal static void setupLocationMenu()
        {
            Menu.mainMenu.AddItem(ShowAllLocations);
            Menu.mainMenu.AddItem(EnableBlips);
            Menu.mainMenu.BindMenuToItem(LocationMenu,ShowAllLocations);
            LocationMenu.ParentMenu = Menu.mainMenu;
            Menu.pool.Add(LocationMenu);

            EnableBlips.CheckboxEvent += OnBlipCheckboxEvent;
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

            if (Settings.EnableVectorBlips)
            {
                AddBlips();
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
        internal static void AddItem(SavedLocation s)
        {
            LocationMenu.AddItem(new UIMenuItem($"{s.Title}",$"x: {s.X} | y: {s.Y} | z: {s.Z} | heading: {s.Heading}"));
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

        internal static void OnLocationSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            EntryPoint.TeleportBasedOnIndexAndDisplay(index);
        }
    }
}