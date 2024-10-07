using UltimateXR.Core;
using UltimateXR.Manipulation;
using UnityEngine;
using UltimateXR.Avatar;
using System.Linq;
using UltimateXR.Devices;

namespace K3.UXRExtensions {
    [RequireComponent(typeof(UxrGrabbableObject))]
    public class LeverButtonSignals : MonoBehaviour {

        private void Start() {
            isGrabbed = K3.Enums.MapToArray<bool, UxrHandSide>();
            var grabbable = GetComponent<UxrGrabbableObject>();

            grabbable.Grabbed += OnGrabbableGrabbed;
            grabbable.Released += OnGrabbableReleased;
        }

        public enum Buttons {
            ButtonA, 
            ButtonB, 
            IndexTrigger,
            ThumbstickPress,
        }

        UxrHandSide[] allSides = Enums.IterateValues<UxrHandSide>().ToArray();
        bool[] isGrabbed;

        UxrInputButtons ToUxrButton(Buttons btn) {
            return btn switch { 
                Buttons.ButtonA => UxrInputButtons.Button1, 
                Buttons.ButtonB => UxrInputButtons.Button2,
                Buttons.IndexTrigger => UxrInputButtons.Trigger, 
                Buttons.ThumbstickPress => UxrInputButtons.ThumbCapSense,
                _ => UxrInputButtons.None,
            };
        }

        public Vector2 ThumbstickDirection() {
            foreach (var side in allSides) if (isGrabbed[(int)side]) 
                    return UxrAvatar.LocalAvatarInput.GetInput2D(side, UxrInput2D.Joystick);
            return default;
        }

        public float TriggerPressure() {
            foreach (var side in allSides) if (isGrabbed[(int)side]) return UxrAvatar.LocalAvatarInput.GetInput1D(side, UxrInput1D.Trigger, false);
            return 0f;
        }

        public bool IsDown(Buttons button) {
            foreach (var side in allSides) if (isGrabbed[(int)side]) return UxrAvatar.LocalAvatarInput.GetButtonsPress(side, ToUxrButton(button));
            return false;
        }
        public bool JustPressed(Buttons button) {
            foreach (var side in allSides) if (isGrabbed[(int)side]) return UxrAvatar.LocalAvatarInput.GetButtonsPressDown(side, ToUxrButton(button));
            return false;
        }

        public bool JustReleased(Buttons button) {
            foreach (var side in allSides) if (isGrabbed[(int)side]) return UxrAvatar.LocalAvatarInput.GetButtonsPressUp(side, ToUxrButton(button));
            return false;
        }

        private void OnGrabbableGrabbed(object sender, UxrManipulationEventArgs e) {
            isGrabbed[(int)e.Grabber.Side] = true;
        }

        private void OnGrabbableReleased(object sender, UxrManipulationEventArgs e) {
            isGrabbed[(int)e.Grabber.Side] = false;
        }
    }
}