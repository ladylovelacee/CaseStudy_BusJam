using UnityEngine;

namespace Runtime.Core
{
    public class PanelBase : MonoBehaviour, IPanel
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private PanelIds panelId;
        public void Initialize()
        {
            UIManager.Instance.AddPanel(panelId, this);
        }

        public void ClosePanel()
        {
            SetPanelVisibility(false);
        }

        public void OpenPanel()
        {
            SetPanelVisibility(true);
        }

        public void Dispose()
        {
            UIManager.Instance.RemovePanel(panelId);
        }

        private void SetPanelVisibility(bool visible)
        {
            canvasGroup.alpha = visible ? 1 : 0;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}