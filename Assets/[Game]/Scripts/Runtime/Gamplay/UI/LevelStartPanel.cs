using Runtime.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Gameplay
{
    public class LevelStartPanel : PanelBase, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("level start");
        }
    }
}