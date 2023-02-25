using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rage;
using Rage.Native;

[assembly: Rage.Attributes.Plugin("Vector3Grabber", Description = "Helps Dev find locations", Author = "Roheat")]
namespace Vector3Grabber
{
    internal static class EntryPoint
    {
        internal static Ped Player = Game.LocalPlayer.Character;
        internal static List<(Vector3 PlayerVector, float heading)> VectorsRead = new List<(Vector3 PlayerVector, float heading)>();
        internal static int GlobalIndexForArray = 0;
        
        static string fullPath = @"Plugins\VectorGrabber\locations.txt";
        private static string readingFilePath = @"Plugins\VectorGrabber\FileToBeRead.txt";

        internal enum direction
        {
            LEFT,
            RIGHT
        }
        
        internal static void Main()
        {
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath);
            }

            if (!File.Exists(readingFilePath))
            {
                File.Create(readingFilePath);
            }
            while (true)
            {
                GameFiber.Yield();
                if (Game.IsKeyDown(Settings.SaveKey)) 
                {
                    AppendToFile(getCoordsAndFormat(),fullPath);
                    AppendToFile(GetCoordsAndHeading(),readingFilePath);
                    Game.DisplayHelp("Coordinates were saved to both text files.");
                }

                if (Game.IsKeyDown(Settings.NextKey) && Game.IsKeyDown(Settings.ModifierKeyForTeleporting))
                {
                    HandleArrow(direction.RIGHT);
                }

                if (Game.IsKeyDown(Settings.BackKey)&& Game.IsKeyDown(Settings.ModifierKeyForTeleporting))
                {
                    HandleArrow(direction.LEFT);
                }
            }
        }

        internal static void AppendToFile(string str, string path)
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(str);
            }
            AddLatestVectorToFile();
        }

        internal static void ReadFile()
        {
            string[] Vectors = File.ReadAllLines(readingFilePath);
            foreach (string Vector in Vectors)
            {
                string[] indivCoords = Vector.Split(',');
                Vector3 VectorToBeAdded = new Vector3(float.Parse(indivCoords[0]),float.Parse(indivCoords[1]),float.Parse(indivCoords[2]));
                VectorsRead.Add((VectorToBeAdded,float.Parse(indivCoords[3])));
            }
        }

        internal static void AddLatestVectorToFile()
        {
            string[] Vectors = File.ReadAllLines(readingFilePath);
            string Vector = Vectors[Vectors.Length - 1];
            string[] indivCoords = Vector.Split(',');
            Vector3 VectorToBeAdded = new Vector3(float.Parse(indivCoords[0]),float.Parse(indivCoords[1]),float.Parse(indivCoords[2]));
            VectorsRead.Add((VectorToBeAdded,float.Parse(indivCoords[3])));
        }

        internal static void HandleArrow(direction direction)
        {
            
            if (direction == direction.LEFT)
            {
                if (GlobalIndexForArray == 0)
                {
                    Game.DisplayHelp("No more Vectors.");
                }
                else
                {
                    GlobalIndexForArray--;
                }
            }

            if (direction == direction.RIGHT)
            {
                if (GlobalIndexForArray == VectorsRead.Count - 1)
                {
                    Game.DisplayHelp("No more Vectors.");
                }
                else
                {
                    GlobalIndexForArray++;
                }
            }
            World.TeleportLocalPlayer(VectorsRead[GlobalIndexForArray].PlayerVector,false);
            Player.Heading = VectorsRead[GlobalIndexForArray].heading;
        }

        internal static string getCoordsAndFormat()
        {
            string str = "";
            string title = OpenTextInput("Vector3Grabber", "",100);
            
            if (title.Equals(null))
            {
                title = "";
            }

            if (Settings.IncludeHeading)
            {
                str += $"(new Vector3({Player.Position.X}f, {Player.Position.Y}f, {Player.Position.Z}f), {Player.Heading}f);";
            }
            else
            {
                str += $"new Vector3({Player.Position.X}f, {Player.Position.Y}f, {Player.Position.Z}f);";
            }
            if (!title.Equals(""))
            {
                str += $"  // {title}";
            }
            Game.LogTrivial($"The string is {str}");
            return str;
        }

        internal static string GetCoordsAndHeading()
        {
            string str = $"{Player.Position.X},{Player.Position.Y}{Player.Position.Z},{Player.Heading}";
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
            return NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
        }
    }
    
}
