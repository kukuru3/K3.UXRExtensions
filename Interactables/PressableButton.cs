using System.Collections.Generic;
using System.Linq;
using UltimateXR.Avatar;
using UltimateXR.UI;
using UnityEngine;

namespace K3.UXRExtensions { 

    public abstract class BaseFingerable<TData> : MonoBehaviour {

        private void LateUpdate() {
            if (UxrAvatar.LocalAvatar == null) return;
            UpdateForFingertips();
        }

        protected virtual void UpdateForFingertips() {
            EnsureFingerDataBuilt();
        }

        private void EnsureFingerDataBuilt() {
            if (fingerData != null) return;
            fingerData = UxrAvatar.LocalAvatar.FingerTips.ToDictionary(ft => ft, ft => CreateData(ft));
        }

        protected abstract TData CreateData(UxrFingerTip tip);

        protected Dictionary<UxrFingerTip, TData> fingerData;
    }

    public class PressableButton : BaseFingerable<PressData> {
        [SerializeField] protected float fingerRadius;
        [SerializeField] protected float cylinderVolumeRadius;
        [SerializeField] protected float cylinderVolumeHeight;
        [SerializeField] Transform buttonTransform;

        [SerializeField] float pressMax;
        [SerializeField] float springbackSpeed;

        [SerializeField] bool isToggle;
        [SerializeField] float toggleIdleDepth;

        bool toggleLatch;

        protected override void UpdateForFingertips() {
            base.UpdateForFingertips();
            var snapbackDepth = 0f;
            foreach (var ft in fingerData) {
                var localDepth = Process(ft.Key, ft.Value);
                snapbackDepth = Mathf.Max(snapbackDepth, localDepth);
            }

            if (isToggle && toggleLatch) {
                if (snapbackDepth < toggleIdleDepth) snapbackDepth = toggleIdleDepth;
            }

            if (snapbackDepth < CurrentPressDepth)
                TrySpringBack(snapbackDepth);
        }

        float defaultButtonY;
        float CurrentPressDepth => defaultButtonY - buttonTransform.localPosition.y;

        void SetPressDepth(float d) {
            var oldDepth = CurrentPressDepth;
            d = Mathf.Clamp(d, 0f, pressMax);
            var p = buttonTransform.localPosition;
            p.y = defaultButtonY - d;
            buttonTransform.localPosition = p;

            HandleNewPressDepth(oldDepth, d);
        }

        private void HandleNewPressDepth(float old, float @new) {
            if (isToggle) {
                // Debug.Log($"Depth: {old:F6} => {@new:F6}");
                if (Mathf.Approximately(@new, pressMax)  && !Mathf.Approximately(old, pressMax)) {
                    toggleLatch = !toggleLatch;
                    Debug.Log($"Toggling latch to {toggleLatch}");
                }
            }    
        }

        private float Process(UxrFingerTip ft, PressData pd) {
            var cposNew = EvaluateFingertip(ft);
            var cposOld = pd.currentCylinderPos;
            var depth = 0f;

            if (cposNew.insideCylinder && !cposOld.insideCylinder) {
                if (cposNew.pressDepth < CurrentPressDepth) {
                    cposNew.insideCylinder = false;
                }
            } else if (cposNew.insideCylinder && cposOld.insideCylinder) {
                if (cposNew.pressDepth > CurrentPressDepth) {
                    SetPressDepth(cposNew.pressDepth);
                    depth = CurrentPressDepth;
                } else {
                    depth = cposNew.pressDepth;
                }
            } 

            pd.currentCylinderPos = cposNew;
            return depth;
        }
    
        private void Start() {
            defaultButtonY = buttonTransform.localPosition.y;
        }
        private void TrySpringBack(float target) {
            if (target > CurrentPressDepth) return;
            var h = Mathf.MoveTowards(CurrentPressDepth, target, Time.deltaTime * springbackSpeed);
            SetPressDepth(h);
        }

        CylinderPos EvaluateFingertip(UxrFingerTip tip) {
            var localPos = transform.InverseTransformPoint(tip.WorldPos);
            var ypos = localPos.y - fingerRadius;
            var xzdist = new Vector2(localPos.x, localPos.z).magnitude;

            if (xzdist < cylinderVolumeRadius) {
                if (ypos < cylinderVolumeHeight && ypos > -cylinderVolumeHeight) {
                    return new CylinderPos { insideCylinder = true, pressDepth = defaultButtonY - ypos };
                }
            } 
            return new CylinderPos {insideCylinder = false };
        }

        protected override PressData CreateData(UxrFingerTip tip) => new PressData();
    }

    internal struct CylinderPos {
        internal bool insideCylinder;
        internal float pressDepth;
    }

    public class PressData {
        internal CylinderPos currentCylinderPos;
        internal CylinderPos prevCylinderPos;
    }
}