using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Gameplay
{
    public class InputManager : MonoBehaviour
    {
        private TouchControls touchControls;
        public TouchControls TouchControls => touchControls ??= new TouchControls();

        /// <summary>
        /// Shows if the finger is currently being held down.
        /// </summary>
        public bool IsFingerDown { get; private set; }

        /// <summary>
        /// Shows the current <b>screen position</b> of finger.
        /// </summary>
        public Vector2 CurrentFingerPosition { get; private set; }

        private void OnEnable()
        {
            TouchControls.Enable();

            TouchControls.Touch.TouchPress.started += StartTouch;
            TouchControls.Touch.TouchPress.canceled += EndTouch;
            TouchControls.Touch.TouchPosition.performed += HandleFingerPosition;
        }

        private void OnDisable()
        {
            TouchControls.Disable();

            TouchControls.Touch.TouchPress.started -= StartTouch;
            TouchControls.Touch.TouchPress.canceled -= EndTouch;
            TouchControls.Touch.TouchPosition.performed -= HandleFingerPosition;
        }

        private void StartTouch(InputAction.CallbackContext context)
        {
            IsFingerDown = true;
            //Publish(CustomManagerEvents.OnFingerDown, TouchControls.Touch.TouchPosition.ReadValue<Vector2>());
        }

        private void EndTouch(InputAction.CallbackContext context)
        {
            IsFingerDown = false;
            //Publish(CustomManagerEvents.OnFingerUp, TouchControls.Touch.TouchPosition.ReadValue<Vector2>());
        }

        private void HandleFingerPosition(InputAction.CallbackContext context)
        {
            CurrentFingerPosition = TouchControls.Touch.TouchPosition.ReadValue<Vector2>();
        }
    }
}