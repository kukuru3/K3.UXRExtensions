using UltimateXR.Manipulation;
using UnityEngine;

namespace K3.Mech.Signals {
    public class SignalRelayGrabbableHeld : MonoBehaviour {
        UxrGrabbableObject grabbable;
        [SerializeField] string signalID;
        Signal<bool> signal = new();
        private void Start() {
            grabbable = GetComponent<UxrGrabbableObject>();
            signal = new Signal<bool> { key = signalID };
        }
        private void LateUpdate() {
            var nextValue = grabbable.IsBeingGrabbed;
            if (signal.value != nextValue) {
                signal.value = nextValue;
                Dispatcher.Send(signal);
            }
        }
    }
}
