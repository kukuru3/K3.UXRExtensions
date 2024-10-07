using UnityEngine;

namespace K3.UXRExtensions {

    // a special use case - a UXR lever can have multiple axes.
    // sometimes, you want a parent to control one of these axes.
    // instead of rewiring how UXR works, we can solve this by inverting the parent-child relationship:
    // erstwhile parent becomes a sibling, and matches only the select axes.

    [DefaultExecutionOrder(-10)]
    public class ReverseParentingMatchRotationAxis : MonoBehaviour {

        [SerializeField] Transform child;

        [SerializeField] bool matchXAxis;
        [SerializeField] bool matchYAxis;
        [SerializeField] bool matchZAxis;

        private void Awake() {

            // wrap self:
            child.transform.parent = transform.parent;
        }

        private void LateUpdate() {
            var lrot = transform.localEulerAngles;

            if (matchXAxis) lrot.x = child.localEulerAngles.x;
            if (matchYAxis) lrot.y = child.localEulerAngles.y;
            if (matchZAxis) lrot.z = child.localEulerAngles.z;

            transform.localEulerAngles = lrot;
        }
    }
}