using UnityEngine;

namespace K3.Mech.Signals {
    public enum TransformAxes {
        TranslateX,
        TranslateY,
        TranslateZ,
        RotateX,
        RotateY,
        RotateZ,
    }

    public enum SignalMapping {
        ZeroToOne,
        MinusOneToOne,
    }

    public class Signal<T> {
        public string key;
        public T value;
    }

    [System.Serializable] internal struct AxisToFloatMapping {
        [SerializeField] internal TransformAxes sourceAxis;
        [SerializeField] internal float mappingFrom;
        [SerializeField] internal float mappingTo;

        [SerializeField]                    internal SignalMapping mappingSpace;
        [SerializeField] [Range(0f,1f)]     internal float remapDeadzone;
        [SerializeField] [Range(0f,1f)]     internal float remapEndzone; // "endzone" is opposite from deadzone, in case you want 0.8+ to map to 1.0
        [SerializeField] [Range(0.1f, 4f)]  internal float remapCurvePowerFactor;
    }

    public class SignalRelayLever1D : MonoBehaviour {
        [SerializeField] AxisToFloatMapping axis;

        [Header("Output")]
        [SerializeField] string signalID;

        Signal<float> signal;

        void Start() {
            signal = new Signal<float>() {key = signalID, value = 0f };
        }

        private void LateUpdate() {
            Process();
        }

        void Process() {
            // var grab = GetComponent<UltimateXR.Manipulation.UxrGrabbableObject>();
            var extractedValue = AxisMappingUtility.ReadLocalTransform(transform, axis.sourceAxis);
            var mappedValue = AxisMappingUtility.AdvancedRemap(extractedValue, axis.mappingFrom, axis.mappingTo,axis.mappingSpace, axis.remapDeadzone, axis.remapEndzone, axis.remapCurvePowerFactor);

            if (!Mathf.Approximately(mappedValue, signal.value)) { 
                signal.value = mappedValue;
                // Debug.Log($"{signal.key} => {signal.value}");
                Dispatcher.Send(signal);
            }
        }
        
    }
}
