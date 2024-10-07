using K3.UXRExtensions;
using UnityEngine;

namespace K3.Mech.Signals {
    public class SignalRelayLeverTrigger : MonoBehaviour {
        [SerializeField] [Range(0f,1f)]     internal float remapDeadzone;
        [SerializeField] [Range(0f,1f)]     internal float remapEndzone; // "endzone" is opposite from deadzone, in case you want 0.8+ to map to 1.0
        [SerializeField] [Range(0.1f, 4f)]  internal float remapCurvePowerFactor;
        [SerializeField] string signalID;

        Signal<float> signal;
        LeverButtonSignals _lever;

        void Start() {
            signal = new Signal<float>() {key = signalID, value = 0f };
            _lever = GetComponent<LeverButtonSignals>() ?? gameObject.AddComponent<LeverButtonSignals>();
        }

        private void LateUpdate() {
            Process();
        }

        void Process() {
            var rawValue = _lever.TriggerPressure();
            var newValue = AxisMappingUtility.Remap(rawValue, remapDeadzone, remapEndzone, remapCurvePowerFactor);
            if (!Mathf.Approximately(signal.value, newValue)) {
                signal.value = newValue;
                Dispatcher.Send(signal);
            }
        }
    }

}
