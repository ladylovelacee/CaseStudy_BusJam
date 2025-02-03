using UnityEngine;

namespace Runtime.Gameplay
{
    public class Bus : MonoBehaviour
    {
        [SerializeField] Renderer m_Renderer;
        [SerializeField] DummyPassenger[] m_DummyPassengerArray;
        public ColorIDs busColor { get; private set; }
        
        private int _dummyPassengerIndex = 0;
        private void OnDisable()
        {
            Dispose();
        }

        private void Dispose()
        {
            transform.SetParent(null);
            for (int i = 0; i < m_DummyPassengerArray.Length; i++)
            {
                m_DummyPassengerArray[i].gameObject.SetActive(false);
            }
        }

        public void Initialize(ColorIDs color)
        {
            busColor = color;
            VisualSetup();
        }

        private void VisualSetup()
        {
            Color color = DataManager.Instance.ColorContainer.GetColorById(busColor);
            MaterialPropertyBlock block = new();
            block.SetColor("_BaseColor", color);
            m_Renderer.SetPropertyBlock(block, 0);

            for (int i = 0; i < m_DummyPassengerArray.Length; i++)
            {
                m_DummyPassengerArray[i].SetColor(color);
                m_DummyPassengerArray[i].gameObject.SetActive(false);
            }
        }
        public void AddPassenger(float checkDelay)
        {
            m_DummyPassengerArray[_dummyPassengerIndex].gameObject.SetActive(true);
            _dummyPassengerIndex++;
        }

    }
}
