using UltimateXR.Manipulation;
using UnityEngine;

namespace K3.UXRExtensions {
    // requires a target mesh.
    // the tweaker is not meant to have a mesh renderer, it is in "thin air".
    // its rotation manipulation movements are mirrored by the target mesh.
    // crucially, after we stop the grip, the tweaker's transform RESETS TO ITS ORIGINAL LOCAL POSE
    // but we DO NOT update the transform of the target mesh, thus giving the illusion of an infinite
    // rotation.

    // we could also define custom constraints for our target mesh, and redefine rotation angles offset min/max
    // on every release based on the target mesh's current rotation.
    
    public class InfiniteRotaryTweaker : MonoBehaviour {
        private UxrGrabbableObject grabbable;

        [SerializeField] Transform mirrorMesh;

        Quaternion previousRotation;

        Quaternion defaultRotation;

        public float AccumulatedRotation { get; set; }

        private void Start() {
            grabbable= GetComponent<UxrGrabbableObject>();

            previousRotation = transform.localRotation;
            defaultRotation = transform.localRotation;

            grabbable.Released += OnGrabbableReleased;
        }

        private void OnGrabbableReleased(object sender, UxrManipulationEventArgs e) {
            transform.localRotation = previousRotation = defaultRotation;
        }

        private void LateUpdate() {
            if (grabbable.IsBeingGrabbed) { 
                var deltaRot = Quaternion.Inverse(previousRotation) * transform.localRotation;
                deltaRot.ToAngleAxis(out var angle, out var axis);
                if (angle > Mathf.Epsilon) {
                    mirrorMesh.localRotation *= deltaRot;
                    AccumulatedRotation += angle * axis.z;
                }
                previousRotation = transform.localRotation;
            }
        }
    }
}