using Runtime.Core;
using UnityEngine.EventSystems;

namespace Runtime.Gameplay
{
    public class LevelStartPanel : PanelBase, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            LevelManager.Instance.StartLevel();
            ClosePanel();
        }
    }
}