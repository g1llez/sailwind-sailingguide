using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(CaptureOrtho.CaptureOrthoMod), CaptureOrtho.PluginInfo.PLUGIN_NAME, CaptureOrtho.PluginInfo.PLUGIN_VERSION, CaptureOrtho.PluginInfo.PLUGIN_AUTHOR)]
[assembly: MelonGame("Raw Lion Workshop", "Sailwind")]

namespace CaptureOrtho
{
    public class CaptureOrthoMod : MelonMod
    {
        private MelonPreferences_Category _config;
        private MelonPreferences_Entry<float> _orthographicSize;
        private MelonPreferences_Entry<float> _northAlignHeight;
        private MelonPreferences_Entry<bool> _northAlignAdjustHeight;
        private MelonPreferences_Entry<bool> _orthoAlignNorth;
        private MelonPreferences_Entry<KeyCode> _environmentKey;
        private MelonPreferences_Entry<KeyCode> _orthoKey;
        private MelonPreferences_Entry<KeyCode> _oceanToggleKey;
        private MelonPreferences_Entry<bool> _requirePlayingState;
        private MelonPreferences_Entry<bool> _orthoAllNonOvrCameras;

        public override void OnInitializeMelon()
        {
            _config = MelonPreferences.CreateCategory("CaptureOrtho");
            _orthographicSize = _config.CreateEntry("OrthographicSize", 250f, "Taille ortho (F9)");
            _northAlignHeight = _config.CreateEntry("NorthAlignHeight", 800f, "Hauteur au-dessus du sol si ajuste Y");
            _northAlignAdjustHeight = _config.CreateEntry(
                "NorthAlignAdjustHeight",
                false,
                "F9: garder Y actuel si false, sinon raycast sol + hauteur");
            _orthoAlignNorth = _config.CreateEntry(
                "OrthoAlignNorth",
                true,
                "F9: aligner nord + vue vers le sol avant ortho");
            _environmentKey = _config.CreateEntry("EnvironmentKey", KeyCode.F8, "Preset environnement");
            _orthoKey = _config.CreateEntry("OrthoKey", KeyCode.F9, "Nord + ortho");
            _oceanToggleKey = _config.CreateEntry("OceanToggleKey", KeyCode.F10, "Toggle Crest ocean ON/OFF");
            _requirePlayingState = _config.CreateEntry(
                "RequirePlayingState",
                false,
                "Si false, touches actives en freecam CUE");
            _orthoAllNonOvrCameras = _config.CreateEntry(
                "OrthoAllNonOvrCameras",
                false,
                "F9: ortho sur toutes les cam non-VR si true");

            LoggerInstance.Msg(
                "Loaded — "
                + _environmentKey.Value + " env, "
                + _orthoKey.Value + " nord+ortho, "
                + _oceanToggleKey.Value + " ocean");
        }

        public override void OnLateUpdate()
        {
            HandleHotkeys();
        }

        private void HandleHotkeys()
        {
            if (_requirePlayingState.Value && !GameState.playing)
            {
                return;
            }

            if (NativeKeyboard.GetKeyDown(_environmentKey.Value))
            {
                LoggerInstance.Msg("Touche env: " + _environmentKey.Value);
                CapturePresets.ApplyEnvironment();
            }

            if (NativeKeyboard.GetKeyDown(_orthoKey.Value))
            {
                LoggerInstance.Msg("Touche ortho: " + _orthoKey.Value);
                CapturePresets.ApplyOrtho(
                    _orthographicSize.Value,
                    _orthoAllNonOvrCameras.Value,
                    _orthoAlignNorth.Value,
                    _northAlignHeight.Value,
                    _northAlignAdjustHeight.Value);
            }

            if (NativeKeyboard.GetKeyDown(_oceanToggleKey.Value))
            {
                OceanToggleHelper.ToggleOceanRenderer();
            }
        }
    }
}
