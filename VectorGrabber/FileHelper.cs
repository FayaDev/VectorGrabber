using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Rage;

namespace VectorGrabber
{
    public class FileHelper
    {
        internal static List<SavedLocation> VectorsRead = new List<SavedLocation>();
        internal static List<Blip> Blips = new List<Blip>();

        internal static string CSharpFilePath = @"Plugins\VectorGrabber\VectorsInCsharpNotation.txt";
        internal static string DeletedVectors = @"Plugins\VectorGrabber\DeletedVectors.txt";
        internal static string CSharpFileDirectory = @"Plugins\VectorGrabber\";
        
        internal static void AddLocation()
        {
            AppendToFile(HelperMethods.GetCoordsAndFormat(out string locationTitle, EntryPoint.Player), CSharpFilePath);
            AddVectorAndHeadingToList(locationTitle, EntryPoint.Player);
            HelperMethods.Notify("~y~Saved", "The coordinates were saved to the text file.");
        }
        
        internal static void AppendToFile(string str, string path)
        {
            using (FileStream fs = new FileStream(path,FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(str);
            }
        }
        
        internal static void ReadFile()
        {
            try
            {
                ValidateCustomNotation();
                List<string> Vectors = new List<string>();

                using (FileStream fileStream = new FileStream(CSharpFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Read the file content
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        // Loop through the lines and split each line into an array
                        while (!reader.EndOfStream)
                        {
                            Vectors.Add(reader.ReadLine());
                        }
                    }
                }

                string[] titleSeps = new string[] { "//" };
                for(int i = 0; i < Vectors.Count; i++)
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

                    try
                    {
                        SavedLocation s = new SavedLocation(Convert.ToSingle(values[0]),
                            Convert.ToSingle(values[1]),
                            Convert.ToSingle(values[2]), Convert.ToSingle(values[3]), title);
                        VectorsRead.Add(s);
                    }
                    catch (Exception e)
                    {
                        Game.LogTrivial($"Line does not contain 4 numbers: {e.Message}");
                    }
                }

                HelperMethods.AddItems(Locations.LocationMenu);
                HelperMethods.AddItems(DeleteLocations.DeleteLocationMenu);
            }
            catch (Exception e)
            {
                HelperMethods.Notify("~y~Warning", "Error occured while reading the file\n~w~Blame yourself. ~g~git gud kid. jk");
                Game.LogTrivial($"Error occurred while reading the file: {e}");
            }
        }
        
        internal static void RereadFile()
        {
            VectorsRead.Clear();
            Locations.LocationMenu.Clear();
            DeleteLocations.DeleteLocationMenu.Clear();
            Menu.DeleteBlips();
            ReadFile();
            HelperMethods.Notify("~y~Re-read", "~g~Text file was reread.");
        }

        internal static void ClearFile()
        {
            string defaultCopyFilePath = $@"{CSharpFileDirectory}FileSave-{DateTime.Now.Millisecond}.txt";
            Game.LogTrivial(defaultCopyFilePath);
            string text  = File.ReadAllText(CSharpFilePath);
            File.WriteAllText(defaultCopyFilePath, text);

            if (File.Exists(CSharpFilePath)) { File.Delete(CSharpFilePath); }

            File.Create(CSharpFilePath);
            HelperMethods.Notify("~y~Cleared", "~g~Text file was cleared. Save file was created.");
        }

        internal static void CopyFile()
        {
            string defaultCopyFilePath = $@"{CSharpFileDirectory}FileCopy-{DateTime.Now.Millisecond}.txt";
            string text  = File.ReadAllText(CSharpFilePath);
            File.WriteAllText(defaultCopyFilePath, text);
            HelperMethods.Notify("~y~Copied", "~g~Text file was copied.");
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

            HelperMethods.AddItem(Locations.LocationMenu, s);
            HelperMethods.AddItem(DeleteLocations.DeleteLocationMenu, s);
            
            Menu.AddBlip(s);
        }

        internal static void DeleteFile()
        {
            File.Delete(CSharpFilePath);
        }

        internal static void UpdateTextFile()
        {
            Menu.ToggleAccessToLocations();
            DeleteFile();
            File.Create(CSharpFilePath).Close();
            foreach (SavedLocation s in VectorsRead)
            {
                string str = HelperMethods.GetCoordsAndFormat(s);
                AppendToFile(str, CSharpFilePath);
            }
            Menu.ToggleAccessToLocations();

            HelperMethods.Notify("~y~Updated", "~g~Updated the text file.");
        }

        internal static void ValidateCustomNotation()
        {
            string[] stringCheck = { "{0}","{1}","{2}","{3}"};
            string customNotation = Settings.CustomNotation;
            string defaultNotation = "(new Vector3({0}f, {1}f, {2}f), {3}f);";
            foreach(string check in stringCheck)
            {
                if (!customNotation.Contains(check))
                {
                    Settings.CustomNotation = defaultNotation;
                    HelperMethods.Notify("~y~Warning", "~r~The notation in the ini is invalid.~y~ It must contain the 3 {} with numbers going from 0-3 inside of them. Follow default notation for help.");
                    Game.DisplayNotification("~y~Defaulting to original notation");
                    break;
                }
            }
        }
        
        internal static void CopyCurrCoordToClipboard()
        {
            Game.SetClipboardText(HelperMethods.GetCoordsAndFormat(out _,EntryPoint.Player));
            HelperMethods.Notify("~y~Saved to clipboard", "The coordinates were saved to your clipboard.");
        }
    }
}