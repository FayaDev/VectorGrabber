using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        static string fullPath = @"Plugins\LSPDFR\ImmersiveAmbientEvents\locations.txt";
        internal static void Main()
        {
            if (!File.Exists(fullPath))
            {
                File.Create(fullPath);
            }
            while (true)
            {
                GameFiber.Yield();
                if (Game.IsKeyDown(Keys.Y)) //change key?
                {
                    AppendToFile(getCoordsAndFormat());
                    Game.DisplayHelp("Coordinates were saved to text file.");
                }
            }
        }

        internal static void AppendToFile(string str)
        {
            using (StreamWriter sw = File.AppendText(fullPath))
            {
                sw.WriteLine(str);
            }
        }

        internal static string getCoordsAndFormat()
        {
            string str = "";
            string title = OpenTextInput("Vector3Grabber", "",100);
            if (title.Equals(""))
            {
                str += $"new Vector3({Player.Position.X}f, {Player.Position.Y}f, {Player.Position.Z}f), {Player.Heading}f;";
                Game.LogTrivial($"string is {str}");
            }
            else
            {
                str += $"new Vector3({Player.Position.X}f, {Player.Position.Y}f, {Player.Position.Z}f), {Player.Heading}f; // {title}";
                Game.LogTrivial($"string is {str}");
            }
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
