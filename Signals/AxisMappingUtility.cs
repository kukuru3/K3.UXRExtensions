using K3;
using UnityEngine;

namespace K3.Mech.Signals {
    static class AxisMappingUtility {

        static internal float AdvancedRemap(float rawValue, float from, float to, SignalMapping mappingSpace = SignalMapping.ZeroToOne, float deadzone = 0f, float endzone = 1f, float curveFactor = 1f) {            
            (var mapFrom, var mapTo) = mappingSpace switch {
                SignalMapping.ZeroToOne => (0f, 1f),
                SignalMapping.MinusOneToOne => (-1f, 1f),
                _ => throw new System.ArgumentException($"Unhandled mapping space : {mappingSpace}")
            };

            var value = rawValue.Map(from, to, mapFrom, mapTo);
            return Remap(value, deadzone, endzone, curveFactor);
        }

        static internal float Remap(float value, float deadzone = 0f, float endzone = 1f, float curveFactor = 1f) {
            var sgn = Mathf.Sign(value);
            var len = Mathf.Abs(value);
            len = len.Map(deadzone, endzone, 0f, 1f);
            len = Mathf.Pow(len, curveFactor);
            return sgn * len;
        }

        static internal float ReadLocalTransform(Transform transform, TransformAxes axis) => axis switch {
            TransformAxes.TranslateX => transform.localPosition.x,
            TransformAxes.TranslateY => transform.localPosition.y,
            TransformAxes.TranslateZ => transform.localPosition.z,
            TransformAxes.RotateX => Mathf.DeltaAngle(0f, transform.localEulerAngles.x),
            TransformAxes.RotateY => Mathf.DeltaAngle(0f, transform.localEulerAngles.y),
            TransformAxes.RotateZ => Mathf.DeltaAngle(0f, transform.localEulerAngles.z),
            _ => throw new System.ArgumentException($"Unhandled axis : `{axis}`")
        };
    }
}
