using System;
using Rage;
using RAGENativeUI;

namespace VectorGrabber
{
    public class TeleportHelper
    {
        internal static int GlobalIndexForArray = 0;
        internal enum direction
        {
            LEFT,
            RIGHT
        }

        internal static void TeleportAndDisplay(Ped Player)
        {
            float x = FileHelper.VectorsRead[GlobalIndexForArray].X;
            float y = FileHelper.VectorsRead[GlobalIndexForArray].Y;
            float z = FileHelper.VectorsRead[GlobalIndexForArray].Z;
            float heading = FileHelper.VectorsRead[GlobalIndexForArray].Heading;
            World.TeleportLocalPlayer(new Vector3(x,y,z),false);
            Player.Heading = heading;
            if (Settings.TeleportNotification)
            {
                Game.DisplayNotification($"Vector: ({x},{y},{z})" +
                                         $"\nHeader: {heading}" +
                                         $"\nLine Number: {GlobalIndexForArray + 1}");
            }
        }
        
        internal static void TeleportBasedOnIndexAndDisplay(int index, Ped Player)
        {
            float x = FileHelper.VectorsRead[index].X;
            float y = FileHelper.VectorsRead[index].Y;
            float z = FileHelper.VectorsRead[index].Z;
            float heading = FileHelper.VectorsRead[index].Heading;
            World.TeleportLocalPlayer(new Vector3(x,y,z),false);
            Player.Heading = heading;
            if (Settings.TeleportNotification)
            {
                Game.DisplayNotification($"Vector: ({x},{y},{z})" +
                                         $"\nHeader: {heading}" +
                                         $"\nLine Number: {index + 1}");
            }
        }
        
        internal static void TeleportToSpecificCoordinate(Ped Player)
        {
            Localization.SetText("TITLE","Enter Line Number that you want to be teleported to");
            string input = HelperMethods.OpenTextInput("TITLE", "", 10);
            if (input.Equals(""))
            {
                Game.DisplayNotification("No input given.");
            }
            else
            {
                if (HelperMethods.isInputValid(input))
                {
                    int index = (Int32.Parse(input)) - 1;
                    if (index >= 0 && index < FileHelper.VectorsRead.Count)
                    {
                        float x = FileHelper.VectorsRead[index].X;
                        float y = FileHelper.VectorsRead[index].Y;
                        float z = FileHelper.VectorsRead[index].Z;
                        float heading = FileHelper.VectorsRead[index].Heading;
                        World.TeleportLocalPlayer(new Vector3(x,y,z), false);
                        Player.Heading = heading;
                        Game.DisplayNotification($"~g~Player teleported to line number: {input}");
                    }
                }
            }
        }
        
        internal static void HandleArrow(direction directionGiven)
        {
            if (directionGiven == direction.LEFT)
            {
                if (GlobalIndexForArray == 0)
                {
                    Game.LogTrivial($"Vector Grabber:Back Key pressed when index was 0.");
                    Game.DisplayNotification("~y~No More Vectors!");
                }
                else
                {
                    GlobalIndexForArray--;
                    TeleportAndDisplay(EntryPoint.Player);
                }
            }

            if (directionGiven == direction.RIGHT)
            {
                int lastIndex = FileHelper.VectorsRead.Count - 1;
                if (GlobalIndexForArray >= lastIndex)
                {
                    Game.LogTrivial($"Vector Grabber:Next Key pressed when array was at its end.");
                    Game.DisplayNotification("~y~No More Vectors!");
                }
                else
                {
                    GlobalIndexForArray++;
                    TeleportAndDisplay(EntryPoint.Player);
                }
            }
        }
    }
}