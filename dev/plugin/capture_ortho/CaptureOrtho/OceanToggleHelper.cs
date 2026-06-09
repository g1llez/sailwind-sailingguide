using Crest;
using MelonLoader;
using UnityEngine;

namespace CaptureOrtho
{
    internal static class OceanToggleHelper
    {
        private static bool? _lastKnownState;

        internal static void ToggleOceanRenderer()
        {
            var ocean = ResolveOceanRenderer();
            if (ocean == null)
            {
                MelonLogger.Warning("F10 ocean — OceanRenderer introuvable");
                return;
            }

            var next = !ocean.enabled;
            ApplyOceanEnabled(ocean, next);
            MelonLogger.Msg(
                "F10 ocean — " + ocean.gameObject.name + ".enabled = " + next
                + " (état lu: " + ocean.enabled + ")");
        }

        internal static void SetOceanRendererEnabled(bool enabled)
        {
            var ocean = ResolveOceanRenderer();
            if (ocean == null)
            {
                return;
            }

            ApplyOceanEnabled(ocean, enabled);
        }

        private static void ApplyOceanEnabled(OceanRenderer ocean, bool enabled)
        {
            _lastKnownState = enabled;
            ocean.enabled = enabled;
        }

        private static OceanRenderer ResolveOceanRenderer()
        {
            if (RefsDirectory.instance != null && RefsDirectory.instance.oceanRenderer != null)
            {
                return RefsDirectory.instance.oceanRenderer;
            }

            if (OceanRenderer.Instance != null)
            {
                return OceanRenderer.Instance;
            }

            return UnityEngine.Object.FindObjectOfType<OceanRenderer>();
        }
    }
}
