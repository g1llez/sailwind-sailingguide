using MelonLoader;
using UnityEngine;

namespace CaptureOrtho
{
    internal static class NorthAlignHelper
    {
        // Vue vers le sol, nord en haut de l'écran (180° corrigé vs boussole au sol).
        private static readonly Vector3 NorthDownEuler = new Vector3(90f, 0f, 0f);

        internal static void AlignNorthLookingDown(float heightAboveGround, bool adjustHeightFromGround)
        {
            var camera = CaptureCameraHelper.FindCaptureCamera();
            if (camera == null)
            {
                MelonLogger.Warning("Nord — aucune caméra");
                return;
            }

            var position = camera.transform.position;
            if (adjustHeightFromGround && TryGetGroundPoint(position, out var groundPoint))
            {
                position = groundPoint + Vector3.up * heightAboveGround;
            }

            var rotation = Quaternion.Euler(NorthDownEuler);
            CueCameraReflection.SyncFreecamTransform(position, rotation);

            MelonLogger.Msg(
                "Nord — haut=carte nord, vue vers le sol | pos "
                + position.ToString("F1")
                + " rot "
                + NorthDownEuler.ToString("F1"));
        }

        private static bool TryGetGroundPoint(Vector3 nearPosition, out Vector3 groundPoint)
        {
            var origin = new Vector3(nearPosition.x, nearPosition.y + 10000f, nearPosition.z);
            if (Physics.Raycast(origin, Vector3.down, out var hit, 20000f))
            {
                groundPoint = hit.point;
                return true;
            }

            groundPoint = default;
            return false;
        }
    }
}
