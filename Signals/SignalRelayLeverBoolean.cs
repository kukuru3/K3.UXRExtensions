using UnityEngine;

namespace K3.Mech.Signals {
    public class SignalRelayLeverBoolean : MonoBehaviour {
        [SerializeField] string signalID;
        [SerializeField] internal TransformAxes sourceAxis;
        [SerializeField] internal bool greaterThan;
        [SerializeField] internal float reference;
        private Signal<bool> signal;

        private void Start() {
            signal = new Signal<bool> { key = signalID };
        }

        private void LateUpdate() {
            var extractedValue = AxisMappingUtility.ReadLocalTransform(transform, sourceAxis);
            var result = extractedValue > reference;
            if (greaterThan) result = !result;
            if (signal.value != result) {
                signal.value = result;
                Dispatcher.Send(signal);
            }
        }
    }
}
