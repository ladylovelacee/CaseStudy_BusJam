using DG.Tweening;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleBase : MonoBehaviour
    {
        public ColorIDs ColorID { get; private set; }
        public bool IsFull => false; // TODO : check empty seat count

        private const float MoveDuration = 2.0f;
        public void Initialize(ColorIDs color)
        {
            ColorID = color;
            Move(VehicleManager.Instance.targetPoints[1]);
        }
        private void Move(Vector3 target)
        {
            transform.DOMove(target, MoveDuration);
        }
    }
}