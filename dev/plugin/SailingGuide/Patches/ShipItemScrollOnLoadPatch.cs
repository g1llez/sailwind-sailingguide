using HarmonyLib;
using UnityEngine;

namespace SailingGuide.Patches
{
    [HarmonyPatch(typeof(ShipItemScroll), "OnLoad")]
    internal static class ShipItemScrollOnLoadPatch
    {
        private static void Postfix(ShipItemScroll __instance)
        {
            if (!SailingGuidePlugin.ReplaceTutorialScroll.Value)
            {
                return;
            }

            var traverse = Traverse.Create(__instance);
            bool tutorialScroll = traverse.Field("tutorialScroll").GetValue<bool>();
            if (!tutorialScroll)
            {
                return;
            }

            string[] fileNames = SailingGuidePlugin.GetPageFileNames();
            string pagesDir = PageLoader.GetPagesDirectory();
            Texture2D[] pages = PageLoader.GetOrLoad(pagesDir, fileNames);
            if (pages == null || pages.Length == 0)
            {
                SailingGuidePlugin.LogWarning(
                    "Tutorial scroll unchanged — no pages loaded from " + pagesDir);
                return;
            }

            traverse.Field("pages").SetValue(pages);
            traverse.Field("pageCount").SetValue(pages.Length);
            traverse.Field("currentPage").SetValue(0);

            var pageRenderer = traverse.Field("page").GetValue<Renderer>();
            if (pageRenderer != null && pageRenderer.material != null)
            {
                pageRenderer.material.mainTexture = pages[0];
            }

            string displayName = SailingGuidePlugin.ScrollDisplayName.Value;
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                __instance.name = displayName;
            }

            SailingGuidePlugin.LogInfo(
                "Replaced tutorial scroll with " + pages.Length + " custom page(s).");
        }
    }
}
