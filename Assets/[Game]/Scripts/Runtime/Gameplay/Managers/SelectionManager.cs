using UnityEngine;

namespace Runtime.Gameplay
{
    public class SelectionManager : MonoBehaviour
    {
        private float MaxCastDistance = 500f;
        [SerializeField] private LayerMask selectableLayer;
        private void OnEnable()
        {
            InputManager.Instance.TouchControls.Touch.TouchPress.started += onTouchStarted;
        }

        private void OnDisable()
        {
            InputManager.Instance.TouchControls.Touch.TouchPress.started -= onTouchStarted;
        }

        private void onTouchStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {;
            CheckSelectable(InputManager.Instance.CurrentFingerPosition);
        }

        private void CheckSelectable(Vector2 fingerPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(fingerPos);

            if(Physics.Raycast(ray, out RaycastHit hitInfo, MaxCastDistance, selectableLayer))
            {
                ISelectable selectable = hitInfo.collider.GetComponentInParent<ISelectable>();
                if(selectable != null && selectable.IsSelectable)
                    selectable.Select();
            }
        }
    }
}