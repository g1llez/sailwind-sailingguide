using System;
using System.Reflection;
using UnityEngine;

namespace CaptureOrtho
{
    internal static class CueCameraReflection
    {
        private static Camera _cachedCamera;
        private static FieldInfo _ourCameraField;
        private static Type _freeCamPanelType;
        private static MethodInfo _setCameraPositionMethod;
        private static MethodInfo _setCameraRotationMethod;

        internal static Camera TryGetFreecam()
        {
            if (_cachedCamera != null && _cachedCamera)
            {
                return _cachedCamera;
            }

            EnsureResolved();
            if (_ourCameraField == null)
            {
                return null;
            }

            _cachedCamera = _ourCameraField.GetValue(null) as Camera;
            return _cachedCamera;
        }

        internal static void SyncFreecamTransform(Vector3 position, Quaternion rotation)
        {
            var camera = TryGetFreecam();
            if (camera != null)
            {
                camera.transform.SetPositionAndRotation(position, rotation);
            }

            EnsureResolved();
            if (_setCameraPositionMethod == null || _setCameraRotationMethod == null)
            {
                return;
            }

            _setCameraPositionMethod.Invoke(null, new object[] { position, true });
            _setCameraRotationMethod.Invoke(null, new object[] { rotation, true });
        }

        private static void EnsureResolved()
        {
            if (_ourCameraField != null)
            {
                return;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var assemblyName = assembly.GetName().Name;
                if (assemblyName == null
                    || assemblyName.IndexOf("CinematicUnityExplorer", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                _freeCamPanelType = assembly.GetType("UnityExplorer.UI.Panels.FreeCamPanel");
                if (_freeCamPanelType == null)
                {
                    continue;
                }

                _ourCameraField = _freeCamPanelType.GetField(
                    "ourCamera",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                _setCameraPositionMethod = _freeCamPanelType.GetMethod(
                    "SetCameraPosition",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(Vector3), typeof(bool) },
                    null);

                _setCameraRotationMethod = _freeCamPanelType.GetMethod(
                    "SetCameraRotation",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(Quaternion), typeof(bool) },
                    null);

                return;
            }
        }
    }
}
