using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Rage;
using RAGENativeUI.PauseMenu;

namespace VectorGrabber
{
    public class FileHelper
    {
        internal static List<SavedLocation> VectorsRead = new List<SavedLocation>();
        internal static List<Blip> Blips = new List<Blip>();
        internal static string CsharpFilePath = @"Plugins\VectorGrabber\VectorsInCsharpNotation.txt";
        internal static string CsharpFileDirectory = @"Plugins\VectorGrabber\";
        
        internal static void AddLocation()
        {
            string locationTitle;
            AppendToFile(HelperMethods.getCoordsAndFormat(out locationTitle,EntryPoint.Player),EntryPoint.CsharpFilePath);
            AddVectorAndHeadingToList(locationTitle,EntryPoint.Player);
            Game.DisplayNotification("~g~Coordinates were saved to text file.");
        }
        
        
        internal static void AppendToFile(string str, string path)
        {
            using (FileStream fs = new FileStream(path,FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(str);
            }
        }
        
        internal static void ReadFile()
        {
            try
            {
                string[] Vectors = File.ReadAllLines(CsharpFilePath);
                string[] titleSeps = new string[] { "//" };
                for(int i = 0; i < Vectors.Length; i++)
                {
                    if (Vectors[i].Equals(""))
                    {
                        Game.LogTrivial($"Line Number {i+1} was invalid. Skipping line.");
                        continue;
                    }
                    string[] values = Regex.Replace(Vectors[i].Trim(), "Vector3|[^0-9,-.]", "").Split(',');
                    string[] titleSplit = Vectors[i].Split(titleSeps, StringSplitOptions.None);
                    string title;
                    if (titleSplit.Length == 1)
                    {
                        title = $"Location at Line Number: {i + 1}";
                    }
                    else
                    {
                        title = $"{titleSplit[1].Trim()}";
                    }

                    if (values.Length != 4)
                    {
                        Game.LogTrivial($"Line Number {i+1} was invalid. Skipping line.");
                        continue;
                    }
                    else
                    {
                        SavedLocation s = new SavedLocation(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]),
                            Convert.ToSingle(values[2]), Convert.ToSingle(values[3]), title);
                        VectorsRead.Add(s);
                    }
                }
                Locations.AddItems();
                DeleteLocations.AddItems();
            }
            catch (Exception e)
            {
                Game.DisplayNotification("~r~Error occurred while reading the file. ~w~Blame yourself. ~g~git gud kid. jk");
                Game.LogTrivial($"Error occurred while reading the file: {e.Message}");
            }
            
        }
        
        internal static void RereadFile()
        {
            VectorsRead.Clear();
            Locations.LocationMenu.Clear();
            DeleteLocations.DeleteLocationMenu.Clear();
            Menu.DeleteBlips();
            ReadFile();
            Game.DisplayNotification("~g~Text file was reread.");
        }

        internal static void ClearFile()
        {
            string defaultCopyFilePath = $@"{CsharpFileDirectory}FileSave-{DateTime.Now.Millisecond}.txt";
            Game.LogTrivial(defaultCopyFilePath);
            string text  = File.ReadAllText(CsharpFilePath);
            File.WriteAllText(defaultCopyFilePath, text);
            File.Delete(CsharpFilePath);
            File.Create(CsharpFilePath);
            Game.DisplayNotification("~g~Text file was cleared. Save file was created.");
        }
        
        internal static void AddVectorAndHeadingToList(string title, Ped Player)
        {
            if (title.Equals(""))
            {
                title = $"Location at Line Number: {VectorsRead.Count + 1}";
            }
            SavedLocation s =
                new SavedLocation(Player.Position.X, Player.Position.Y, Player.Position.Z, Player.Heading,title);
            VectorsRead.Add(s);
            Locations.AddItem(s);
            Menu.AddBlip(s);
        }

        internal static void UpdateTextFile()
        {
            Menu.ToggleAccessToLocations();
            File.Delete(CsharpFilePath);
            List<string> NewVectors = new List<string>();
            for (int i = 0; i < VectorsRead.Count; i++)
            {
                SavedLocation s = VectorsRead[i];
                NewVectors.Add($"(new Vector3({s.X}f, {s.Y}f, {s.Z}f), {s.Heading}f); // {s.Title}");
            }
            File.WriteAllLines(CsharpFilePath,NewVectors);
            Menu.ToggleAccessToLocations();
        }

        internal static void CopyCurrCoordToClipboard()
        {
            Game.SetClipboardText(HelperMethods.getCoordsAndFormat(out _,EntryPoint.Player));
            Game.DisplayNotification("~g~Coordinates were copied to computer clipboard.");
        }
    }
}