using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UltimateXR.Avatar;

namespace K3.UXRExtensions {
    public static class XRUtils {

        public static Vector3? TryGetRightEyeOffset() {
            
            var cam = UxrAvatar.LocalAvatar.CameraTransform;
            if (cam == null) return null;

            var wpos = WorldspacePositionOfRightEye();
            if (wpos == null) return null;

            var pt = cam.InverseTransformPoint(wpos.Value);
            return pt;
        }

        static List<XRNodeState> states = new();

        static Vector3? TryGetRightEyePosition() {
            InputTracking.GetNodeStates(states);

            foreach (var state in states) {
                if (state.nodeType == XRNode.RightEye) {
                    if (state.TryGetPosition(out var pos))
                        return pos;
                }
            }
            return default;
        }

        public static Vector3? WorldspacePositionOfRightEye() {
            var pos = TryGetRightEyePosition();
            if (!pos.HasValue) return null;
            if (UxrAvatar.LocalAvatar == null) return null;
            return UxrAvatar.LocalAvatar.transform.TransformPoint(pos.Value);
        }
    }
}