using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

[assembly: Rage.Attributes.Plugin("VectorGrabber", Description = "Helps developers find locations for callouts/ambient events", Author = "Roheat")]
namespace VectorGrabber
{
    internal static class EntryPoint
    {
        internal static Ped Player => Game.LocalPlayer.Character;
        internal static List<SavedLocation> VectorsRead = new List<SavedLocation>();
        internal static List<Blip> Blips = new List<Blip>();
        internal static int GlobalIndexForArray = 0;
        
        static string CsharpFilePath = @"Plugins\VectorGrabber\VectorsInCsharpNotation.txt";
        static string CsharpFileDirectory = @"Plugins\VectorGrabber\";
        internal enum direction
        {
            LEFT,
            RIGHT
        }
        
        internal static void Main()
        {
            GameFiber.StartNew(Menu.CreateMainMenu);
            VersionChecker.CheckForUpdates();
            Settings.Initialize();
            if (!Directory.Exists(CsharpFileDirectory))
            {
                Directory.CreateDirectory(CsharpFileDirectory);
            }
            if (!File.Exists(CsharpFilePath))
            {
                File.Create(CsharpFilePath);
            }
            else
            {
                ReadFile();
            }
            while (true)
            {
                GameFiber.Yield();
                if (Player.IsValid() &&Game.IsKeyDown(Settings.SaveKey) && Game.IsKeyDown(Settings.ModifierKey))
                {
                    string locationTitle;
                    AppendToFile(getCoordsAndFormat(out locationTitle),CsharpFilePath);
                    AddVectorAndHeadingToList(locationTitle);
                    Game.DisplayHelp("Coordinates were saved to text file.");
                }

                if (Player.IsValid()&& Game.IsKeyDown(Settings.TeleportNextKey) && Game.IsKeyDown(Settings.ModifierKey))
                {
                    HandleArrow(direction.RIGHT);
                }

                if (Player.IsValid()&&Game.IsKeyDown(Settings.TeleportBackKey) && Game.IsKeyDown(Settings.ModifierKey))
                {
                    HandleArrow(direction.LEFT);
                }

                if (Player.IsValid() && Game.IsKeyDown(Settings.TeleportKey) && Game.IsKeyDown(Settings.ModifierKey))
                {
                    TeleportToSpecificCoordinate();
                }
                if (Player.IsValid() && Game.IsKeyDown(Settings.RereadFile) && Game.IsKeyDown(Settings.ModifierKey))
                {
                   RereadFile();
                }
                if (Player.IsValid() && Game.IsKeyDown(Settings.ClipboardKey) && Game.IsKeyDown(Settings.ModifierKey))
                {
                    CopyCurrCoordToClipboard();
                }
            }
        }

        internal static void RereadFile()
        {
            VectorsRead.Clear();
            Locations.LocationMenu.Clear();
            Locations.DeleteBlips();
            ReadFile();
            Game.DisplayHelp("Text file was reread.");
        }

        internal static void AddVectorAndHeadingToList(string title)
        {
            if (title.Equals(""))
            {
                title = $"Location at Line Number: {VectorsRead.Count + 1}";
            }
            SavedLocation s =
                new SavedLocation(Player.Position.X, Player.Position.Y, Player.Position.Z, Player.Heading,title);
            VectorsRead.Add(s);
            Locations.AddItem(s);
            Locations.AddBlip(s);
        }

        internal static void CopyCurrCoordToClipboard()
        {
            Game.SetClipboardText(getCoordsAndFormat(out _));
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
                    SavedLocation s =  new SavedLocation(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]),Convert.ToSingle(values[3]),title);
                    VectorsRead.Add(s);
                }
                Locations.AddItems();
            }
            catch (Exception e)
            {
                Game.DisplayHelp("Error occurred while reading the file. Blame yourself. git gud kid. jk");
                Game.LogTrivial($"Error occurred while reading the file: {e.Message}");
            }
            
        }

        internal static void HandleArrow(direction directionGiven)
        {
            if (directionGiven == direction.LEFT)
            {
                if (GlobalIndexForArray == 0)
                {
                    Game.LogTrivial($"Vector Grabber:Back Key pressed when index was 0.");
                    Game.DisplayNotification("No More Vectors!");
                }
                else
                {
                    GlobalIndexForArray--;
                    TeleportAndDisplay();
                }
            }

            if (directionGiven == direction.RIGHT)
            {
                int lastIndex = VectorsRead.Count - 1;
                if (GlobalIndexForArray >= lastIndex)
                {
                    Game.LogTrivial($"Vector Grabber:Next Key pressed when array was at its end.");
                    Game.DisplayNotification("No More Vectors!");
                }
                else
                {
                    GlobalIndexForArray++;
                    TeleportAndDisplay();
                }
            }
        }

        internal static void TeleportAndDisplay()
        {
            float x = VectorsRead[GlobalIndexForArray].X;
            float y = VectorsRead[GlobalIndexForArray].Y;
            float z = VectorsRead[GlobalIndexForArray].Z;
            float heading = VectorsRead[GlobalIndexForArray].Heading;
            World.TeleportLocalPlayer(new Vector3(x,y,z),false);
            Player.Heading = heading;
            Game.DisplayHelp($"Vector: ({x},{y},{z})" +
                             $"\nHeader: {heading}" +
                             $"\nLine Number: {GlobalIndexForArray + 1}");
        }
        
        internal static void TeleportBasedOnIndexAndDisplay(int index)
        {
            float x = VectorsRead[index].X;
            float y = VectorsRead[index].Y;
            float z = VectorsRead[index].Z;
            float heading = VectorsRead[index].Heading;
            World.TeleportLocalPlayer(new Vector3(x,y,z),false);
            Player.Heading = heading;
            Game.DisplayHelp($"Vector: ({x},{y},{z})" +
                             $"\nHeader: {heading}" +
                             $"\nLine Number: {index + 1}");
        }

        internal static string getCoordsAndFormat(out string title)
        {
            string str = "";
            title = OpenTextInput("VectorGrabber", "",100);
            str += $"(new Vector3({Player.Position.X}f, {Player.Position.Y}f, {Player.Position.Z}f), {Player.Heading}f);";
            if (!title.Equals(""))
            {
                str += $"  // {title}";
            }
            Game.LogTrivial($"The string is {str}");
            return str;
        }
        internal static string OpenTextInput(string windowTitle, string defaultText, int maxLength)
        {
            NativeFunction.Natives.DISABLE_ALL_CONTROL_ACTIONS(2);
            NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(true, windowTitle, 0, defaultText, 0, 0, 0, maxLength);

            while (NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>() == 0)
            {
                GameFiber.Yield();
            }

            NativeFunction.Natives.ENABLE_ALL_CONTROL_ACTIONS(2);
            return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>() ?? "";
        }
        internal static bool isInputValid(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    Game.DisplayNotification("Invalid input");
                    return false;
                }
            }

            return true;
        }

        internal static void TeleportToSpecificCoordinate()
        {
            string input = OpenTextInput("VectorGrabber", "", 10);
            if (input.Equals(""))
            {
                Game.DisplayNotification("No input given.");
            }
            else
            {
                if (isInputValid(input))
                {
                    int index = (Int32.Parse(input)) - 1;
                    if (index >= 0 && index < VectorsRead.Count)
                    {
                        float x = VectorsRead[index].X;
                        float y = VectorsRead[index].Y;
                        float z = VectorsRead[index].Z;
                        float heading = VectorsRead[index].Heading;
                        World.TeleportLocalPlayer(new Vector3(x,y,z), false);
                        Player.Heading = heading;
                        Game.DisplayNotification($"Player teleported to line number: {input}");
                    }
                }
            }
        }
        
        
        internal static bool CheckClipboardModifierKey()
        {
            if (Settings.ModifierKey == Keys.None)
            {
                return true;
            }
            else
            {
                return Game.IsKeyDownRightNow(Settings.ModifierKey);
            }
        }

        
    }
}