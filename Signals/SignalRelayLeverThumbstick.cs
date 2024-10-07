using K3.UXRExtensions;
using UnityEngine;

namespace K3.Mech.Signals {
    public class SignalRelayLeverThumbstick : MonoBehaviour {
        [SerializeField] string signalID;
        [SerializeField] AxisToFloatMapping map;

        Signal<Vector2> signal;
        LeverButtonSignals _lever;

        void Start() {
            signal = new Signal<Vector2>() {key = signalID, value = default };
            _lever = GetComponent<LeverButtonSignals>() ?? gameObject.AddComponent<LeverButtonSignals>();
        }
        private void LateUpdate() {
            var rawValue = _lever.ThumbstickDirection();
            var x = AxisMappingUtility.Remap(rawValue.x ,map.remapDeadzone, map.remapEndzone, map.remapCurvePowerFactor);
            var y = AxisMappingUtility.Remap(rawValue.y ,map.remapDeadzone, map.remapEndzone, map.remapCurvePowerFactor);

            var vec = new Vector2(x,y);
            if (Vector2.Distance(vec, signal.value) > 0.001f) {
                signal.value = vec;
                Dispatcher.Send(signal);
            }
        }
    }
}
