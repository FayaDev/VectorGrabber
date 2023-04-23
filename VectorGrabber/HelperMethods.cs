using System;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI;

namespace VectorGrabber
{
    public class HelperMethods
    {
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
        
        internal static bool CheckModifierKey() => Settings.ModifierKey == Keys.None ? true : Game.IsKeyDownRightNow(Settings.ModifierKey);
        
        internal static string getCoordsAndFormat(out string title, Ped Player)
        {
            string str = "";
            Localization.SetText("TITLE","Enter title for save location");
            title = OpenTextInput("TITLE", "",100);
            //str += $"(new Vector3({Player.Position.X}f, {Player.Position.Y}f, {Player.Position.Z}f), {Player.Heading}f);";
            str += String.Format(Settings.CustomNotation, Player.Position.X,Player.Position.Y,Player.Position.Z, Player.Heading);
            if (!title.Equals(""))
            {
                str += $"  // {title}";
            }
            Game.LogTrivial($"The string is {str}");
            return str;
        }

        internal static string getCoordsAndFormat(SavedLocation s)
        {
            string str = "";
            //str += $"(new Vector3({Player.Position.X}f, {Player.Position.Y}f, {Player.Position.Z}f), {Player.Heading}f);";
            str += String.Format(Settings.CustomNotation, s.X,s.Y,s.Z, s.Heading);
            if (!s.Title.Equals(""))
            {
                str += $"  // {s.Title}";
            }
            return str;
        }
    }
}