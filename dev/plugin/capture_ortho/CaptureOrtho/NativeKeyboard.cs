using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CaptureOrtho
{
    /// <summary>
    /// CinematicUnityExplorer patches UnityEngine.Input — poll Windows directly for hotkeys.
    /// </summary>
    internal static class NativeKeyboard
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int virtualKey);

        private static readonly Dictionary<int, bool> PreviousDown = new Dictionary<int, bool>();

        internal static bool GetKeyDown(KeyCode key)
        {
            var virtualKey = KeyCodeToVirtualKey(key);
            if (virtualKey < 0)
            {
                return Input.GetKeyDown(key);
            }

            var isDown = (GetAsyncKeyState(virtualKey) & 0x8000) != 0;
            var wasDown = PreviousDown.TryGetValue(virtualKey, out var previous) && previous;
            PreviousDown[virtualKey] = isDown;
            return isDown && !wasDown;
        }

        private static int KeyCodeToVirtualKey(KeyCode key)
        {
            if (key >= KeyCode.F1 && key <= KeyCode.F15)
            {
                return 0x70 + (key - KeyCode.F1);
            }

            if (key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9)
            {
                return 0x30 + (key - KeyCode.Alpha0);
            }

            if (key >= KeyCode.Keypad0 && key <= KeyCode.Keypad9)
            {
                return 0x60 + (key - KeyCode.Keypad0);
            }

            return -1;
        }
    }
}
