using K3.UXRExtensions;
using UnityEngine;

namespace K3.Mech.Signals {
    [SerializeField]
    public class SignalRelayIntegerFromTweaker : MonoBehaviour {
        [SerializeField] string signalID;
        Signal<int> signal;
        private RadialSnapSteps snapper;

        private void Start() {
            signal = new Signal<int>() { key = signalID };
            snapper = GetComponent<RadialSnapSteps>();
        }

        void LateUpdate() {
            if (signal.value != snapper.CurrentSnapIndex) {
                signal.value = snapper.CurrentSnapIndex;
                Dispatcher.Send(signal);
            }
        }
    }
}
