using System;
using UltimateXR.Manipulation;
using UnityEngine;

namespace K3.UXRExtensions {

    public class SnapbackWhenNotGrabbed : MonoBehaviour {
        private UxrGrabbableObject grabbable;

        [SerializeField] bool snapbackRotation;
        [SerializeField] bool snapbackTranslation;

        [SerializeField] float snapbackSpeedRotation = 100f;
        [SerializeField] float snapbackSpeedTranslation = 1f;
        private void Start() {
            grabbable = GetComponent<UxrGrabbableObject>();
            if (grabbable == null) throw new Exception("Expected a UXR Grabbable");
        }

        private void LateUpdate() {
            if (!grabbable.IsBeingGrabbed) {
                if (snapbackRotation) { 
                    var localRot = transform.localRotation;
                    var targetRot = Quaternion.identity;
                    var angle = Quaternion.Angle(localRot, targetRot);
                    var tspaceDelta = snapbackSpeedRotation * Time.deltaTime;
                    if (angle > 0.01f) { 
                        tspaceDelta /= angle;
                        var newRot = Quaternion.Slerp(localRot, targetRot, tspaceDelta);
                        transform.localRotation = newRot;
                    }
                } 

                if (snapbackTranslation) {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, snapbackSpeedTranslation * Time.deltaTime);
                }
            }
        }
    }
}