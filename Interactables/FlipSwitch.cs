using UltimateXR.Avatar;
using UltimateXR.Haptics;
using UltimateXR.UI;
using UnityEngine;

namespace K3.UXRExtensions { 
    public class FlipSwitch : BaseFingerable<FlipSwitch.PerFingerData> {

        public event System.Action<FlipSwitch> StateUpdated;
        public bool CurrentState { get; private set; }

        public bool InteractionEnabled { get; set; } = true;

        const float FINGER_RADIUS = 0.01f;

        [SerializeField] private float interactionZoneCenter;
        [SerializeField] private float broadInteractionRadius = 0.1f;
        [SerializeField] private float angleOn;
        [SerializeField] private float angleOff;
        [SerializeField] private float interactionCylinderRadius = 0.02f;
        float smallerAngle;
        float largerAngle;
        float midpoint;

        public class PerFingerData {
            internal bool previousInteraction;
            internal int  entryDirection;
        }
        protected override void UpdateForFingertips() {
            base.UpdateForFingertips();

            foreach (var tip in fingerData.Keys) {
                if (!InteractionEnabled) {
                    OnNoInteraction(tip);
                } else {
                    var dist = Vector3.Magnitude(transform.position - tip.WorldPos);
                    if (dist > broadInteractionRadius) {
                        OnNoInteraction(tip);
                    } else {
                        ReceiveFingerInteraction(tip, tip.WorldPos, FINGER_RADIUS);
                    }
                }
            }

        }

        protected override PerFingerData CreateData(UxrFingerTip tip) {
            // tip.WorldPos
            return new PerFingerData();
        }

        void OnNoInteraction(UxrFingerTip tip) {
            if (fingerData[tip].previousInteraction) Snap();
            fingerData[tip].previousInteraction = false;
        }

        void Start() {
            smallerAngle = Mathf.Min(angleOff, angleOn);
            largerAngle = Mathf.Max(angleOff, angleOn);
            midpoint = (smallerAngle + largerAngle)/2;
            ForceState(false);
        }
        public void ForceState(bool flipped) => SetRotationAngle(flipped ? angleOn : angleOff);
        void Snap() {
            var currentRotation = Mathf.DeltaAngle(0f, transform.localRotation.eulerAngles.x);
            
            if (Mathf.Abs(currentRotation - angleOn) < Mathf.Abs(currentRotation - angleOff))
                SetRotationAngle(angleOn);
            else
                SetRotationAngle(angleOff);
        }

        float GetTangentAngle(Vector2 center, float radius) {
            var d = center.magnitude;
            return Mathf.DeltaAngle(0f, Mathf.Asin(radius / d) * Mathf.Rad2Deg);
        }

        internal void ReceiveFingerInteraction(UxrFingerTip finger, Vector3 fingerCenter, float fingerRadius) {
            // finger center starts in worldspace; we immediately convert it to localspace.
            fingerCenter = transform.InverseTransformPoint(fingerCenter);
            // project finger center on the interactionCircle, defined by center (0,0, interactionZoneCenter) with normal (0,1,0).
            var fingerCircleProj = Vector3.ProjectOnPlane(fingerCenter, Vector3.up);

            var d = Vector3.Distance(fingerCircleProj, new Vector3(0,0, interactionZoneCenter));
            if (d <= interactionCylinderRadius) {
                ProcessFingerInsideInteractionZone(finger, fingerCenter.z, fingerCenter.y, fingerRadius);
                // inside cylinder
            } else {
                OnNoInteraction(finger);
                // outside cylinder
            }
        }

        private void ProcessFingerInsideInteractionZone(UxrFingerTip finger, float z, float y, float fingerRadius) {
            var justEnteredZone = !fingerData[finger].previousInteraction;
            fingerData[finger].previousInteraction = true;
            if (justEnteredZone) fingerData[finger].entryDirection = (y >= 0f) ? 1 : -1;

            var baseAngle = Mathf.DeltaAngle(0f, Mathf.Atan2(y, z) * Mathf.Rad2Deg);
            var tangentAngle = GetTangentAngle(new Vector2(z,y), fingerRadius);
            var dir = fingerData[finger].entryDirection;
            var mostPenetratingAngle = baseAngle - tangentAngle * dir;
            if (mostPenetratingAngle * dir < 0) {
                var nextX = CurrentAngle() - mostPenetratingAngle;
                nextX = Mathf.Clamp(nextX, smallerAngle, largerAngle);

                var oldState = CurrentState;
                SetRotationAngle(nextX);
                if (CurrentState != oldState) 
                    UxrAvatar.LocalAvatar.ControllerInput.SendHapticFeedback(finger.HandGrabber.Side, UxrHapticClipType.Click, 0.5f);
            }
        }

        float CurrentAngle() => Mathf.DeltaAngle(0f, transform.localRotation.eulerAngles.x);

        void SetRotationAngle(float nextAngle) {
            var prevAngle = CurrentAngle();
            var finalAngle = Mathf.Clamp(nextAngle, smallerAngle, largerAngle);
            transform.localRotation = Quaternion.Euler(finalAngle, 0f, 0f);
            if ((finalAngle - midpoint) * (prevAngle - midpoint) <= 0f) { // meaning, they are on opposite sides of midpoint
                var prevState = CurrentState;
                CurrentState = (finalAngle - midpoint) * (angleOn - midpoint) >= 0f;
                if (prevState != CurrentState)
                    StateUpdated?.Invoke(this);
            }
        }
    }
}


#if UNITY_EDITOR
namespace K3.Editor {
    using UnityEditor;
    using K3.UXRExtensions;

    [CustomEditor(typeof(FlipSwitch))]
    class FlipSwitchInspector : Editor {
        FlipSwitch Target => (FlipSwitch)target;
        private void OnSceneGUI() {
            var t = Target.transform;
            var f = serializedObject.FindProperty("interactionZoneCenter").floatValue;
            var rLarge = serializedObject.FindProperty("broadInteractionRadius").floatValue;
            var span0 = serializedObject.FindProperty("angleOff").floatValue;
            var span1 = serializedObject.FindProperty("angleOn").floatValue;
            var rCylinder = serializedObject.FindProperty("interactionCylinderRadius").floatValue;
            Handles.matrix = t.localToWorldMatrix;
            Handles.color = new Color(1f, 0.6f, 0.2f, 0.04f);
            Handles.SphereHandleCap(0, Vector3.zero, Quaternion.identity, rLarge * 2, EventType.Repaint);
            Handles.color = new Color(1f, 0.6f, 0.2f);
            Handles.DrawWireDisc(new Vector3(0, 0, f), new Vector3(0, 1, 0), rCylinder);
            Handles.DrawDottedLine(new Vector3(0, 0, 0), new Vector3(0f, Mathf.Sin(-span1 * Mathf.Deg2Rad), Mathf.Cos(-span1 * Mathf.Deg2Rad)) * (f * 2 + 0.01f), 2f);
            Handles.DrawDottedLine(new Vector3(0, 0, 0), new Vector3(0f, Mathf.Sin(-span0 * Mathf.Deg2Rad), Mathf.Cos(-span0 * Mathf.Deg2Rad)) * (f * 2 + 0.01f), 2f);
            Handles.DrawWireArc(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0f, Mathf.Sin(-span0 * Mathf.Deg2Rad), Mathf.Cos(-span0 * Mathf.Deg2Rad)), (span1 - span0), f * 2);
        }
    }
}
#endif