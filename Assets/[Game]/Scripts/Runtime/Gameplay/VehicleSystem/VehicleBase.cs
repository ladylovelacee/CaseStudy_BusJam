using DG.Tweening;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleBase : MonoBehaviour
    {
        public ColorIDs ColorID { get; private set; }
        public bool IsFull => currentPassengers >= maxCapacity; // TODO : check empty seat count
        
        private VehicleManager Manager => VehicleManager.Instance;

        private int maxCapacity = 3; // TODO: make generic
        private int currentPassengers = 0;

        private const float MoveDuration = 2.0f;
        public void Initialize(ColorIDs color)
        {
            ColorID = color;
            Move(Manager.WaitPoint.position);
        }
        private void Move(Vector3 target)
        {
            transform.DOMove(target, MoveDuration);
        }

        public void AddPassenger()
        {
            currentPassengers++;

            if (IsFull)
            {
                Manager.OnBusFilled();
            }
        }
    }
}