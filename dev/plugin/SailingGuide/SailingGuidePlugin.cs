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
        internal static ConfigEntry<string> GuideId;
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

        internal static bool UsesAutoPageDiscovery()
        {
            return IsAutoPageList(PageFileNames.Value);
        }

        internal static string[] GetPageFileNames()
        {
            if (UsesAutoPageDiscovery())
            {
                return PageLoader.DiscoverPageFiles(
                    PageLoader.GetPagesDirectory(),
                    GuideId.Value);
            }

            return PageFileNames.Value.Split(',')
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToArray();
        }

        private static bool IsAutoPageList(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return true;
            }

            raw = raw.Trim();
            return raw.Equals("auto", System.StringComparison.OrdinalIgnoreCase) || raw == "*";
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

            GuideId = Config.Bind(
                "Guide",
                "GuideId",
                "alankh",
                "Guide subfolder under PagesDirectory. Used when PageFileNames is empty (auto scan *.png, sorted by name).");

            PageFileNames = Config.Bind(
                "Guide",
                "PageFileNames",
                string.Empty,
                "Comma-separated PNG paths relative to PagesDirectory. Leave empty (or 'auto') to load all PNGs from PagesDirectory/GuideId/ sorted by filename.");

            ScrollDisplayName = Config.Bind(
                "Guide",
                "ScrollDisplayName",
                "The Sailor's Diary — Al'Ankh",
                "Display name on the scroll item when replacement is enabled.");

            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();

            string pagesDir = PageLoader.GetPagesDirectory();
            string[] pages = GetPageFileNames();
            Logger.LogInfo(
                "Loaded. ReplaceTutorialScroll="
                + ReplaceTutorialScroll.Value
                + ", pages dir="
                + pagesDir
                + ", guide="
                + GuideId.Value
                + ", page mode="
                + (UsesAutoPageDiscovery() ? "auto" : "manual")
                + ", page count="
                + pages.Length);
            for (int i = 0; i < pages.Length; i++)
            {
                Logger.LogInfo("  [" + i + "] " + pages[i]);
            }
        }
    }
}
