using DG.Tweening;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class VehicleBase : MonoBehaviour
    {
        [SerializeField] Renderer m_Renderer;
        public ColorIDs ColorID { get; private set; }
        public bool IsFull => currentPassengers >= maxCapacity; // TODO : check empty seat count
        
        private VehicleManager Manager => VehicleManager.Instance;

        private int maxCapacity = 3; // TODO: make generic
        private int currentPassengers = 0;

        private const float MoveDuration = 2.0f;
        public void Initialize(ColorIDs color)
        {
            ColorID = color;
            VisualSetup();
            Move(Manager.WaitPoint.position);
        }
        private void Move(Vector3 target)
        {
            transform.DOMove(target, MoveDuration);
        }
        
        private void VisualSetup()
        {
            Color color = DataManager.Instance.ColorContainer.GetColorById(ColorID);
            MaterialPropertyBlock block = new();
            block.SetColor("_Color", color);
            m_Renderer.SetPropertyBlock(block);
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