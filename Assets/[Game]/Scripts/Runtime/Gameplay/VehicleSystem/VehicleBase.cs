using DG.Tweening;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleBase : MonoBehaviour
    {
        private ColorIDs _id;

        private const float MoveDuration = 2.0f;
        public void Initialize(ColorIDs color)
        {
            _id = color;
            Move(VehicleManager.Instance.targetPoints[1]);
        }

        private void Move(Vector3 target)
        {
            transform.DOMove(target, MoveDuration);
        }
    }
}