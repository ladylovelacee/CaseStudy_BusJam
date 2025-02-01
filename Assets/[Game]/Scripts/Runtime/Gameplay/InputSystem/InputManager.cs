using Runtime.Core;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class InputManager : Singleton<InputManager>
    {
        public event Action OnTapDown;
        public event Action OnTapUp;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTapDown?.Invoke();
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnTapUp?.Invoke();
            }
        }
    }
}