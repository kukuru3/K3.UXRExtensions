using K3.UXRExtensions;
using UnityEngine;

namespace K3.Mech.Signals {
    public class LeverButtonSignalRelay : MonoBehaviour {
        [SerializeField] LeverButtonSignals.Buttons button;
        [SerializeField] string signalID;
        Signal<bool> signal;

        LeverButtonSignals _lever;

        private void Start() {
            signal = new Signal<bool>() { key = signalID };
            _lever = gameObject.GetComponent<LeverButtonSignals>()
                ?? gameObject.AddComponent<LeverButtonSignals>();
        }

        private void LateUpdate() {
            Process();
        }

        void Process() {
            var currentValue = _lever.IsDown(button);
            if (signal.value != currentValue) {
                signal.value = currentValue;
                // Debug.Log($"{signal.key} => {signal.value}");
                Dispatcher.Send(signal);    
            }
        }
    }
}
