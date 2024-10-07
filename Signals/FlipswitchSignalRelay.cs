using K3.UXRExtensions;
using UnityEngine;

namespace K3.Mech.Signals {
    [RequireComponent(typeof(FlipSwitch))]
    public class FlipswitchSignalRelay : MonoBehaviour {
        private FlipSwitch fs;

        [SerializeField] string id;

        Signal<bool> signal = new();

        private void Start() {
            fs = GetComponent<FlipSwitch>();
            signal = new Signal<bool>() { key = id };
        }

        private void Update() {
            if (fs.CurrentState != signal.value) {
                signal.value = fs.CurrentState;
                Dispatcher.Send(signal);
            }
        }
    }
}
