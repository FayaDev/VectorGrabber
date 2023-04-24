using System.Net;
using System.Reflection;
using Rage;

namespace VectorGrabber
{
    internal static class VersionChecker
    {
        internal static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        internal static void CheckForUpdates()
        {
            var webClient = new WebClient();
            var pluginUpToDate = false;
            var webSuccess = false;
            try
            {
                var receivedVersion = webClient
                    .DownloadString(
                        "https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=43016&textOnly=1")
                    .Trim();
                Game.LogTrivial(
                    $"Vector Grabber: Online Version: {receivedVersion} | Local VectorGrabber Version: {CurrentVersion}");
                pluginUpToDate = receivedVersion == CurrentVersion;
                webSuccess = true;
            }
            catch (WebException)
            {
                HelperMethods.Notify(
                    "~y~Warning", "Please make sure you are connected to proper WIFI Network.");
            }
            finally
            {
                HelperMethods.Notify(
                    $"~y~{CurrentVersion}~s~ by Roheat", $"Version is {(webSuccess ? pluginUpToDate ? "~g~Up To Date" : "~r~Out Of Date" : "~o~Version Check Failed")}");
                if (!pluginUpToDate)
                {
                    Game.LogTrivial(
                        "Vector Grabber: [VERSION OUTDATED] Please update to latest version here: https://www.lcpdfr.com/downloads/gta5mods/scripts/43016-vectorgrabber/");
                }
            }
        }
    }
}