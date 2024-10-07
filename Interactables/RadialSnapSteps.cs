using System;
using K3;
using UltimateXR.Manipulation;
using UnityEngine;

namespace K3.UXRExtensions {
    public class RadialSnapSteps : MonoBehaviour {
        UxrGrabbableObject grabbable;

        [SerializeField] int numSteps = 5;
        [SerializeField] float snapSpeedDegreesPerSecond = 200;

        float min, max;

        private void Start() {
            grabbable = GetComponent<UxrGrabbableObject>();
            min = grabbable.MinSingleRotationDegrees;
            max = grabbable.MaxSingleRotationDegrees;
        }

        public int CurrentSnapIndex { get; set; } = 0;

        private void Update() {
            if (numSteps < 2) return;

            var t = grabbable.SingleRotationAxisDegrees.Map(min, max, 0f, 1f);
            var nextIndex = Mathf.RoundToInt(t * (numSteps-1));
            if (nextIndex != CurrentSnapIndex) {
                CurrentSnapIndex = nextIndex;
                OnIndexChanged();
            }
            var snappedT = (float)CurrentSnapIndex/(numSteps-1);
            if (!grabbable.IsBeingGrabbed) { 
                var snappedAngle = Mathf.Lerp(min, max, snappedT);
                grabbable.SingleRotationAxisDegrees = Mathf.MoveTowards(
                    grabbable.SingleRotationAxisDegrees, 
                    snappedAngle,
                    snapSpeedDegreesPerSecond * Time.deltaTime
                );
            }
        }

        private void OnIndexChanged() {

        }
    }
}