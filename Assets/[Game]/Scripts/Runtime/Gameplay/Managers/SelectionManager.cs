using Runtime.Core;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private LayerMask selectableLayer;

        private InputManager InputManager => InputManager.Instance;
        private Camera mainCamera;
        private const float MaxCastDistance = 500f;

        private void Awake()
        {
            mainCamera = Camera.main;
        }
        private void OnEnable()
        {
            InputManager.OnTapDown += onTapDown;
        }
        private void OnDisable()
        {
            InputManager.OnTapDown -= onTapDown;
        }

        private void onTapDown()
        {
            if(LevelManager.Instance.IsLevelStarted)
                CheckSelectable();
        }

        private void CheckSelectable()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hitInfo, MaxCastDistance, selectableLayer))
            {
                ISelectable selectable = hitInfo.collider.GetComponentInParent<ISelectable>();
                if(selectable != null && selectable.IsSelectable)
                    selectable.Select();
            }
        }
    }
}