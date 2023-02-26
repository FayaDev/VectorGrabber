using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Rage;
using Rage.Native;

[assembly: Rage.Attributes.Plugin("VectorGrabber", Description = "Helps developers find locations for callouts/ambient events", Author = "Roheat")]
namespace VectorGrabber
{
    internal static class EntryPoint
    {
        internal static Ped Player => Game.LocalPlayer.Character;
        internal static List<(Vector3 PlayerVector, float heading)> VectorsRead = new List<(Vector3 PlayerVector, float heading)>();
        internal static int GlobalIndexForArray = 0;
        
        static string CsharpFilePath = @"Plugins\VectorGrabber\VectorsInCsharpNotation.txt";
        internal enum direction
        {
            LEFT,
            RIGHT
        }
        
        internal static void Main()
        {
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
                if (Player.IsValid() &&Game.IsKeyDown(Settings.SaveKey) ) 
                {
                    AppendToFile(getCoordsAndFormat(),CsharpFilePath);
                    AddVectorAndHeadingToList();
                    Game.DisplayHelp("Coordinates were saved to text file.");
                }

                if (Player.IsValid()&&Game.IsKeyDown(Settings.NextKey) && Game.IsControlKeyDownRightNow)
                {
                    HandleArrow(direction.RIGHT);
                }

                if (Player.IsValid()&&Game.IsKeyDown(Settings.BackKey) && Game.IsControlKeyDownRightNow)
                {
                    HandleArrow(direction.LEFT);
                }

                if (Player.IsValid() && Game.IsKeyDown(Settings.TeleportKey) && Game.IsControlKeyDownRightNow)
                {
                    TeleportToSpecificCoordinate();
                }
            }
        }

        internal static void AddVectorAndHeadingToList()
        {
            VectorsRead.Add((new Vector3(Player.Position.X,Player.Position.Y,Player.Position.Z),Player.Heading));
        }

        internal static void AppendToFile(string str, string path)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(str);
            }
        }
        internal static void ReadFile()
        {
            string[] Vectors = File.ReadAllLines(CsharpFilePath);
            foreach (string Vector in Vectors)
            {
                string[] values = Regex.Replace(Vector.Trim(), "Vector3|[^0-9,-.]", "").Split(',');
                Vector3 VectorToBeAdded = new Vector3(Convert.ToSingle(values[0]), Convert.ToSingle(values[1]), Convert.ToSingle(values[2]));
                VectorsRead.Add((VectorToBeAdded, Convert.ToSingle(values[3])));
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
            float x = VectorsRead[GlobalIndexForArray].PlayerVector.X;
            float y = VectorsRead[GlobalIndexForArray].PlayerVector.Y;
            float z = VectorsRead[GlobalIndexForArray].PlayerVector.Z;
            float heading = VectorsRead[GlobalIndexForArray].heading;
            World.TeleportLocalPlayer(VectorsRead[GlobalIndexForArray].PlayerVector,false);
            Player.Heading = heading;
            Game.DisplayHelp($"Vector: ({x},{y},{z})" +
                             $"\nHeader: {heading}" +
                             $"\nLine Number: {GlobalIndexForArray + 1}");
        }

        internal static string getCoordsAndFormat()
        {
            string str = "";
            string title = OpenTextInput("VectorGrabber", "",100);
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
                        World.TeleportLocalPlayer(VectorsRead[index].PlayerVector, false);
                        Player.Heading = VectorsRead[index].heading;
                        Game.DisplayNotification($"Player teleported to line number: {input}");
                    }
                }
            }
        }

        
    }
    
}
