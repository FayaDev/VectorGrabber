using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using Rage;
using Rage.Native;

[assembly: Rage.Attributes.Plugin("VectorGrabber", Description = "Helps developers find locations for callouts/ambient events", Author = "Roheat")]
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
            if (!File.Exists(fullPath) && !File.Exists(readingFilePath))
            {
                File.Create(fullPath);
                File.Create(readingFilePath);
            }
            else if (File.Exists(fullPath) && !File.Exists(readingFilePath))
            {
                File.Create(readingFilePath);
            }
            else if (!File.Exists(fullPath) && File.Exists(readingFilePath))
            {
                File.Delete(readingFilePath);
                File.Create(fullPath);
                File.Create(readingFilePath);
            }
            else
            {
                ReadFile();
            }

            Game.DisplayHelp("Inputs may not work as player is not valid. Try switching characters.");
            while (true)
            {
                GameFiber.Yield();
                if (Player.IsValid() &&Game.IsKeyDown(Settings.SaveKey) ) 
                {
                    AppendToFile(getCoordsAndFormat(),fullPath);
                    AppendToFile(GetCoordsAndHeading(),readingFilePath);
                    ReadFile();
                    Game.DisplayHelp("Coordinates were saved to both text files.");
                }

                if (Player.IsValid()&&Game.IsKeyDown(Settings.NextKey) && Game.IsControlKeyDownRightNow)
                {
                    HandleArrow(direction.RIGHT);
                }

                if (Player.IsValid()&&Game.IsKeyDown(Settings.BackKey)&& Game.IsControlKeyDownRightNow)
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
        }
        internal static void ReadFile()
        {
            string[] Vectors = File.ReadAllLines(readingFilePath);
            foreach (string Vector in Vectors)
            {
                string[] indivCoords = Vector.Split(',');
                Game.LogTrivial($"{indivCoords[0]}   {indivCoords[1]}");
                Vector3 VectorToBeAdded = new Vector3(Convert.ToSingle(indivCoords[0].Trim()),Convert.ToSingle(indivCoords[1].Trim()),Convert.ToSingle(indivCoords[2].Trim()));
                VectorsRead.Add((VectorToBeAdded,Convert.ToSingle(indivCoords[3])));
                
            }
        }
        
        internal static void HandleArrow(direction directionGiven)
        {
            
            
            if (directionGiven == direction.LEFT)
            {
                if (GlobalIndexForArray == 0)
                {
                    Game.LogTrivial($"Vector Grabber:Back Key pressed when index was 0.");
                }
                else
                {
                    GlobalIndexForArray--;
                }
            }

            if (directionGiven == direction.RIGHT)
            {
                int lastIndex = VectorsRead.Count - 1;
                if (GlobalIndexForArray >= lastIndex)
                {
                    Game.LogTrivial($"Vector Grabber:Next Key pressed when array was at its end.");
                }
                else
                {
                    GlobalIndexForArray++;
                }
            }
            World.TeleportLocalPlayer(VectorsRead[GlobalIndexForArray].PlayerVector,false);
            Player.Heading = VectorsRead[GlobalIndexForArray].heading;
            Game.DisplayHelp($"Vector: ({VectorsRead[GlobalIndexForArray].PlayerVector.X},{VectorsRead[GlobalIndexForArray].PlayerVector.Y},{VectorsRead[GlobalIndexForArray].PlayerVector.Z})" +
                             $"\nLine Number: {GlobalIndexForArray + 1}");
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
            string str = $"{Player.Position.X},{Player.Position.Y},{Player.Position.Z},{Player.Heading}";
            Game.LogTrivial(str);
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
