using Runtime.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Gameplay
{
    public class InputManager : Singleton<InputManager>
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
            TouchControls.Touch.TouchPosition.performed += HandleFingerPosition;
        }

        private void OnDisable()
        {
            TouchControls.Disable();
            TouchControls.Touch.TouchPosition.performed -= HandleFingerPosition;
        }

        private void HandleFingerPosition(InputAction.CallbackContext context)
        {
            CurrentFingerPosition = TouchControls.Touch.TouchPosition.ReadValue<Vector2>();
        }
    }
}