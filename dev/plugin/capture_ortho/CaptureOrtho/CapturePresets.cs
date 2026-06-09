using System.Collections.Generic;
using System.Text;
using Crest;
using MelonLoader;
using UnityEngine;

namespace CaptureOrtho
{
    internal static class CapturePresets
    {
        internal static void ApplyEnvironment()
        {
            var warnings = new List<string>();

            if (Sun.sun != null)
            {
                Sun.sun.initialTimescale = 0f;
                Sun.sun.globalTime = 12f;
            }
            else
            {
                warnings.Add("Sun");
            }

            if (PlayerNeeds.instance != null)
            {
                PlayerNeeds.instance.godMode = true;
            }
            else
            {
                warnings.Add("PlayerNeeds");
            }

            Settings.tutorialHintsEnabled = false;
            if (Hints.instance != null)
            {
                Hints.instance.gameObject.transform.localScale = Vector3.zero;
            }
            else
            {
                warnings.Add("Hints");
            }

            if (Weather.instance != null)
            {
                Weather.instance.enabled = false;
            }
            else
            {
                warnings.Add("Weather");
            }

            if (Wind.instance != null)
            {
                Wind.instance.minimumMagnitude = 0f;
                Wind.instance.maximumMagnitude = 0f;
                Wind.instance.ForceNewWind(Vector3.zero);
                Wind.instance.enabled = false;
            }
            else
            {
                warnings.Add("Wind");
            }

            var wavesInertia = GameObject.Find("waves inertia");
            if (wavesInertia != null)
            {
                var inertia = wavesInertia.GetComponent<WavesInertia>();
                if (inertia != null)
                {
                    inertia.enabled = false;
                    inertia.currentMagnitude = 1f;
                    inertia.currentInertia = 1f;
                }
                else
                {
                    wavesInertia.GetComponent<Behaviour>().enabled = false;
                }
            }

            SetOceanSpecularZero(warnings);
            WaveMultiplierHelper.ZeroLegacyWaveMultipliers(warnings);
            DisableWaveSystems(warnings);

            LogResult("F8 — preset environnement", warnings);
        }

        internal static void ApplyOrtho(
            float orthographicSize,
            bool includeAllNonOvrCameras,
            bool alignNorth,
            float northAlignHeight,
            bool northAlignAdjustHeight)
        {
            var warnings = new List<string>();

            if (alignNorth)
            {
                NorthAlignHelper.AlignNorthLookingDown(northAlignHeight, northAlignAdjustHeight);
            }

            var cameraCount = CaptureCameraHelper.ApplyOrthoToCaptureCameras(orthographicSize, includeAllNonOvrCameras);

            if (cameraCount == 0)
            {
                warnings.Add("aucune caméra active");
            }

            OceanToggleHelper.SetOceanRendererEnabled(false);
            if (OceanRenderer.Instance == null)
            {
                warnings.Add("OceanRenderer");
            }

            DisableWaveSystems(warnings);
            LogResult(
                "F9 — nord + ortho (size " + orthographicSize + ", " + cameraCount + " cam)",
                warnings);
        }

        private static void SetOceanSpecularZero(List<string> warnings)
        {
            if (OceanColorBlender.instance == null)
            {
                warnings.Add("OceanColorBlender");
                return;
            }

            var palette = OceanColorBlender.instance.GetCurrentPalette();
            palette.oceanSpecular = 0f;
            OceanColorBlender.instance.ApplyPalette(palette);
        }

        private static void DisableWaveSystems(List<string> warnings)
        {
            var crestUpdater = UnityEngine.Object.FindObjectOfType<OceanUpdaterCrest>();
            if (crestUpdater != null)
            {
                crestUpdater.enabled = false;
            }

            var oceanUpdater = UnityEngine.Object.FindObjectOfType<OceanWavesUpdater>();
            if (oceanUpdater != null)
            {
                oceanUpdater.enabled = false;
            }

            var gerstnerCount = 0;
            foreach (var shape in UnityEngine.Object.FindObjectsOfType<ShapeGerstnerBatched>())
            {
                shape._weight = 0f;
                gerstnerCount++;
            }

            if (gerstnerCount == 0)
            {
                warnings.Add("ShapeGerstnerBatched");
            }
        }

        private static void LogResult(string label, List<string> warnings)
        {
            if (warnings.Count == 0)
            {
                MelonLogger.Msg(label + " OK");
                return;
            }

            var message = new StringBuilder(label);
            message.Append(" — manquant: ");
            message.Append(string.Join(", ", warnings));
            MelonLogger.Warning(message.ToString());
        }
    }
}
