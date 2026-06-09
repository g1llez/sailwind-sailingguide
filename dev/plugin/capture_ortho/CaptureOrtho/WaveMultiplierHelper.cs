using System.Collections.Generic;
using System.Reflection;
using Crest;
using MelonLoader;
using UnityEngine;

namespace CaptureOrtho
{
    internal static class WaveMultiplierHelper
    {
        private static readonly string[] SpectrumObjectNames =
        {
            "SailwindWavesInertial",
            "SailwindWindChange"
        };

        internal static void ZeroLegacyWaveMultipliers(List<string> warnings)
        {
            var found = 0;

            foreach (var objectName in SpectrumObjectNames)
            {
                if (TryZeroSpectrumOnGameObject(objectName))
                {
                    found++;
                    MelonLogger.Msg("Waves " + objectName + "._multiplier = 0");
                }
                else
                {
                    warnings.Add(objectName);
                }
            }

            if (Ocean.Singleton != null)
            {
                Ocean.Singleton.scale = 0f;
                Ocean.Singleton.choppy_scale = 0f;
                Ocean.Singleton.windx = 0f;
                Ocean.Singleton.windy = 0f;
                found++;
            }

            if (found == 0)
            {
                warnings.Add("wave multipliers");
            }
        }

        private static bool TryZeroSpectrumOnGameObject(string objectName)
        {
            var gameObject = GameObject.Find(objectName);
            if (gameObject == null)
            {
                return false;
            }

            var spectrum = gameObject.GetComponent<OceanWaveSpectrum>();
            if (spectrum == null)
            {
                return false;
            }

            return TrySetFloatField(spectrum, "_multiplier", 0f);
        }

        private static bool TrySetFloatField(object target, string fieldName, float value)
        {
            if (target == null)
            {
                return false;
            }

            var field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field == null || field.FieldType != typeof(float))
            {
                return false;
            }

            field.SetValue(target, value);
            return true;
        }
    }
}
