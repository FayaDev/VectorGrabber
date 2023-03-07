using System.Windows.Forms;
using Rage;
using Rage.Native;

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
        
        internal static bool CheckClipboardModifierKey() => Settings.ModifierKey == Keys.None ? true : Game.IsKeyDownRightNow(Settings.ModifierKey);
        
        internal static string getCoordsAndFormat(out string title, Ped Player)
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
    }
    
}