using System;
using System.Text;
using MelonLoader;
using UnityEngine;

namespace CaptureOrtho
{
    internal static class CaptureCameraHelper
    {
        private static readonly string[] CueCameraNameHints =
        {
            "UnityExplorer",
            "Explorer",
            "Freecam",
            "Cinematic",
            "CueCamera"
        };

        internal static Camera FindCaptureCamera()
        {
            var cueCamera = CueCameraReflection.TryGetFreecam();
            if (cueCamera != null)
            {
                return cueCamera;
            }

            Camera best = null;
            var bestDepth = float.MinValue;

            foreach (var camera in UnityEngine.Object.FindObjectsOfType<Camera>())
            {
                if (camera == null || !camera.enabled || !camera.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (MatchesCueCameraName(camera.gameObject.name))
                {
                    return camera;
                }

                if (IsUnderOvrRig(camera.transform))
                {
                    continue;
                }

                if (camera.depth > bestDepth)
                {
                    bestDepth = camera.depth;
                    best = camera;
                }
            }

            if (best != null)
            {
                return best;
            }

            return Camera.main;
        }

        internal static int ApplyOrthoToCaptureCameras(float orthographicSize, bool includeAllNonOvr)
        {
            var count = 0;
            var primary = FindCaptureCamera();

            if (primary != null)
            {
                ApplyOrtho(primary, orthographicSize);
                count++;
                MelonLogger.Msg(
                    "Ortho -> " + DescribeCamera(primary) + " | orthographic=" + primary.orthographic
                    + " size=" + primary.orthographicSize);
            }
            else
            {
                MelonLogger.Warning("Ortho -> aucune caméra trouvée");
                LogAllCameras();
            }

            if (!includeAllNonOvr)
            {
                return count;
            }

            foreach (var camera in UnityEngine.Object.FindObjectsOfType<Camera>())
            {
                if (camera == null || camera == primary || !camera.enabled || !camera.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (IsUnderOvrRig(camera.transform))
                {
                    continue;
                }

                ApplyOrtho(camera, orthographicSize);
                count++;
                MelonLogger.Msg("Ortho -> " + DescribeCamera(camera));
            }

            return count;
        }

        private static void ApplyOrtho(Camera camera, float orthographicSize)
        {
            camera.orthographic = true;
            camera.orthographicSize = orthographicSize;
        }

        private static string DescribeCamera(Camera camera)
        {
            return camera.gameObject.name + " (depth " + camera.depth + ", enabled " + camera.enabled + ")";
        }

        private static void LogAllCameras()
        {
            var report = new StringBuilder("Caméras actives: ");
            var any = false;

            foreach (var camera in UnityEngine.Object.FindObjectsOfType<Camera>())
            {
                if (camera == null || !camera.gameObject.activeInHierarchy)
                {
                    continue;
                }

                any = true;
                report.Append('[').Append(DescribeCamera(camera)).Append("] ");
            }

            if (!any)
            {
                report.Append("(aucune)");
            }

            MelonLogger.Msg(report.ToString());
        }

        private static bool MatchesCueCameraName(string objectName)
        {
            foreach (var hint in CueCameraNameHints)
            {
                if (objectName.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsUnderOvrRig(Transform transform)
        {
            var current = transform;
            while (current != null)
            {
                var name = current.name;
                if (name.IndexOf("OVRCameraRig", StringComparison.OrdinalIgnoreCase) >= 0
                    || name.IndexOf("TrackingSpace", StringComparison.OrdinalIgnoreCase) >= 0
                    || name.IndexOf("CenterEyeAnchor", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }

                current = current.parent;
            }

            return false;
        }
    }
}
