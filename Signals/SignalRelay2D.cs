using UnityEngine;

namespace K3.Mech.Signals {

    public class SignalRelay2D : MonoBehaviour {
        [SerializeField] AxisToFloatMapping xAxis;
        [SerializeField] AxisToFloatMapping yAxis;

        [SerializeField] string signalID;

        Signal<Vector2> signal;

        private void Start() {
            signal = new Signal<Vector2>() { key = signalID };
        }

        private void LateUpdate() {
            Process();
        }

        void Process() {
            var x = AxisMappingUtility.ReadLocalTransform(transform, xAxis.sourceAxis);
            var y = AxisMappingUtility.ReadLocalTransform(transform, yAxis.sourceAxis);

            var mappedX = AxisMappingUtility.AdvancedRemap(x, xAxis.mappingFrom, xAxis.mappingTo, xAxis.mappingSpace, xAxis.remapDeadzone, xAxis.remapEndzone, xAxis.remapCurvePowerFactor);
            var mappedY = AxisMappingUtility.AdvancedRemap(y, yAxis.mappingFrom, yAxis.mappingTo, yAxis.mappingSpace, yAxis.remapDeadzone, yAxis.remapEndzone, yAxis.remapCurvePowerFactor);

            var vector = new Vector2(mappedX, mappedY);
            
            if (!Mathf.Approximately(mappedX, signal.value.x) || !Mathf.Approximately(mappedY, signal.value.y )) { 
                signal.value = vector;
                Dispatcher.Send(signal);
                // Debug.Log($"Signal [{signal.key}]: {signal.value:F2} ({x},{y})");
            }
        }
    }
}
