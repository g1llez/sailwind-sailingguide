using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace SailingGuide
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class SailingGuidePlugin : BaseUnityPlugin
    {
        public static SailingGuidePlugin Instance { get; private set; }

        internal static ConfigEntry<bool> ReplaceTutorialScroll;
        internal static ConfigEntry<string> PagesDirectory;
        internal static ConfigEntry<string> PageFileNames;
        internal static ConfigEntry<string> ScrollDisplayName;

        internal static void LogInfo(string message)
        {
            Instance.Logger.LogInfo(message);
        }

        internal static void LogWarning(string message)
        {
            Instance.Logger.LogWarning(message);
        }

        internal static string[] GetPageFileNames()
        {
            string raw = PageFileNames.Value;
            if (string.IsNullOrWhiteSpace(raw))
            {
                return new string[0];
            }

            return raw.Split(',')
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToArray();
        }

        private void Awake()
        {
            Instance = this;

            ReplaceTutorialScroll = Config.Bind(
                "Guide",
                "ReplaceTutorialScroll",
                true,
                "Replace the starting Sailing Manual with custom PNG pages from the pages folder.");

            PagesDirectory = Config.Bind(
                "Guide",
                "PagesDirectory",
                "pages/en",
                "Language/content root under the plugin folder (e.g. pages/en, pages/fr). Absolute paths supported.");

            PageFileNames = Config.Bind(
                "Guide",
                "PageFileNames",
                "alankh/01-cover.png,alankh/02-legend.png,alankh/15-lionsfang.png,alankh/18-neverdin-1.png,alankh/19-neverdin-2.png,alankh/20-oldankhtown-1.png,alankh/21-oldankhtown-2.png,alankh/22-oldankhtown-3.png",
                "Comma-separated PNG paths relative to PagesDirectory (page order).");

            ScrollDisplayName = Config.Bind(
                "Guide",
                "ScrollDisplayName",
                "The Sailor's Diary — Al'Ankh",
                "Display name on the scroll item when replacement is enabled.");

            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();

            string pagesDir = PageLoader.GetPagesDirectory();
            Logger.LogInfo(
                "Loaded. ReplaceTutorialScroll="
                + ReplaceTutorialScroll.Value
                + ", pages dir="
                + pagesDir);
        }
    }
}
