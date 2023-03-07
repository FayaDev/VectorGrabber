using System;
using Rage;

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
            float x = SavedLocationList.VectorsRead[GlobalIndexForArray].X;
            float y = SavedLocationList.VectorsRead[GlobalIndexForArray].Y;
            float z = SavedLocationList.VectorsRead[GlobalIndexForArray].Z;
            float heading = SavedLocationList.VectorsRead[GlobalIndexForArray].Heading;
            World.TeleportLocalPlayer(new Vector3(x,y,z),false);
            Player.Heading = heading;
            Game.DisplayNotification($"Vector: ({x},{y},{z})" +
                                     $"\nHeader: {heading}" +
                                     $"\nLine Number: {GlobalIndexForArray + 1}");
        }
        
        internal static void TeleportBasedOnIndexAndDisplay(int index, Ped Player)
        {
            float x = SavedLocationList.VectorsRead[index].X;
            float y = SavedLocationList.VectorsRead[index].Y;
            float z = SavedLocationList.VectorsRead[index].Z;
            float heading = SavedLocationList.VectorsRead[index].Heading;
            World.TeleportLocalPlayer(new Vector3(x,y,z),false);
            Player.Heading = heading;
            Game.DisplayNotification($"Vector: ({x},{y},{z})" +
                                     $"\nHeader: {heading}" +
                                     $"\nLine Number: {index + 1}");
        }
        
        internal static void TeleportToSpecificCoordinate(Ped Player)
        {
            string input = HelperMethods.OpenTextInput("VectorGrabber", "", 10);
            if (input.Equals(""))
            {
                Game.DisplayNotification("No input given.");
            }
            else
            {
                if (HelperMethods.isInputValid(input))
                {
                    int index = (Int32.Parse(input)) - 1;
                    if (index >= 0 && index < SavedLocationList.VectorsRead.Count)
                    {
                        float x = SavedLocationList.VectorsRead[index].X;
                        float y = SavedLocationList.VectorsRead[index].Y;
                        float z = SavedLocationList.VectorsRead[index].Z;
                        float heading = SavedLocationList.VectorsRead[index].Heading;
                        World.TeleportLocalPlayer(new Vector3(x,y,z), false);
                        Player.Heading = heading;
                        Game.DisplayNotification($"Player teleported to line number: {input}");
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
                    Game.DisplayNotification("No More Vectors!");
                }
                else
                {
                    GlobalIndexForArray--;
                    TeleportAndDisplay(EntryPoint.Player);
                }
            }

            if (directionGiven == direction.RIGHT)
            {
                int lastIndex = SavedLocationList.VectorsRead.Count - 1;
                if (GlobalIndexForArray >= lastIndex)
                {
                    Game.LogTrivial($"Vector Grabber:Next Key pressed when array was at its end.");
                    Game.DisplayNotification("No More Vectors!");
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