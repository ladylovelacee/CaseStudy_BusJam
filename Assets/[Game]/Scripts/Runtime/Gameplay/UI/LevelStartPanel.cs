using Runtime.Core;
using UnityEngine.EventSystems;

namespace Runtime.Gameplay
{
    public class LevelStartPanel : PanelBase, IPointerClickHandler
    {
        private void OnEnable()
        {
            LevelManager.Instance.LevelLoader.OnLevelLoaded += onLevelLoaded;
        }

        public override void Dispose()
        {
            base.Dispose();
            LevelManager.Instance.LevelLoader.OnLevelLoaded -= onLevelLoaded;
        }
        
        private void onLevelLoaded()
        {
            OpenPanel();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            LevelManager.Instance.StartLevel();
            ClosePanel();
        }
    }
}