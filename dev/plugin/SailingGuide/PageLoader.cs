using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using UnityEngine;

namespace SailingGuide
{
    internal static class PageLoader
    {
        private static Texture2D[] _cached;

        internal static Texture2D[] LoadPages(string pagesDirectory, string[] fileNames)
        {
            if (fileNames == null || fileNames.Length == 0)
            {
                SailingGuidePlugin.LogWarning("No page file names configured.");
                return null;
            }

            var list = new List<Texture2D>(fileNames.Length);
            foreach (string fileName in fileNames)
            {
                string path = Path.Combine(pagesDirectory, fileName.Trim());
                if (!File.Exists(path))
                {
                    SailingGuidePlugin.LogWarning("Page not found: " + path);
                    continue;
                }

                Texture2D tex = LoadTexture(path);
                if (tex != null)
                {
                    list.Add(tex);
                    SailingGuidePlugin.LogInfo(
                        "Loaded page: " + fileName + " (" + tex.width + "x" + tex.height + ")");
                }
            }

            if (list.Count == 0)
            {
                return null;
            }

            return list.ToArray();
        }

        internal static Texture2D[] GetOrLoad(string pagesDirectory, string[] fileNames)
        {
            if (_cached != null && _cached.Length > 0)
            {
                return _cached;
            }

            _cached = LoadPages(pagesDirectory, fileNames);
            return _cached;
        }

        private static Texture2D LoadTexture(string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (!tex.LoadImage(data))
                {
                    UnityEngine.Object.Destroy(tex);
                    SailingGuidePlugin.LogWarning("LoadImage failed: " + path);
                    return null;
                }

                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.name = Path.GetFileNameWithoutExtension(path);
                return tex;
            }
            catch (Exception ex)
            {
                SailingGuidePlugin.LogWarning("Failed to read " + path + ": " + ex.Message);
                return null;
            }
        }

        internal static string GetPagesDirectory()
        {
            string configured = SailingGuidePlugin.PagesDirectory.Value;
            if (string.IsNullOrWhiteSpace(configured))
            {
                return Path.Combine(Paths.PluginPath, PluginInfo.PLUGIN_NAME, "pages");
            }

            configured = configured.Trim();
            if (Path.IsPathRooted(configured))
            {
                return configured;
            }

            return Path.Combine(Paths.PluginPath, PluginInfo.PLUGIN_NAME, configured);
        }

        internal static string[] DiscoverPageFiles(string pagesDirectory, string guideId)
        {
            if (string.IsNullOrWhiteSpace(guideId))
            {
                SailingGuidePlugin.LogWarning("GuideId is empty — cannot discover pages.");
                return new string[0];
            }

            guideId = guideId.Trim().Replace('\\', '/').Trim('/');
            string guideFolder = Path.Combine(pagesDirectory, guideId.Replace('/', Path.DirectorySeparatorChar));
            if (!Directory.Exists(guideFolder))
            {
                SailingGuidePlugin.LogWarning("Guide folder not found: " + guideFolder);
                return new string[0];
            }

            string[] files = Directory.GetFiles(guideFolder, "*.png", SearchOption.TopDirectoryOnly);
            Array.Sort(files, StringComparer.OrdinalIgnoreCase);

            var list = new List<string>(files.Length);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                if (string.IsNullOrEmpty(name) || name.StartsWith("_"))
                {
                    continue;
                }

                list.Add(guideId + "/" + name);
            }

            SailingGuidePlugin.LogInfo(
                "Auto-discovered " + list.Count + " page(s) in " + guideFolder);
            return list.ToArray();
        }
    }
}
