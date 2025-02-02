using Runtime.Core;

namespace Runtime.Gameplay
{
    public class LevelLoadingPanel : PanelBase
    {
        private void OnEnable()
        {
            LevelManager.Instance.OnLevelCompleted += onLevelCompleted;
            LevelManager.Instance.LevelLoader.OnLevelLoaded += onLevelLoaded;
        }

        private void onLevelCompleted()
        {
            OpenPanel();
            LevelManager.Instance.LevelLoader.LoadLevel();
        }

        private void onLevelLoaded()
        {
            ClosePanel();
        }

        public override void Dispose()
        {
            base.Dispose();
            LevelManager.Instance.OnLevelCompleted -= onLevelCompleted;
            LevelManager.Instance.LevelLoader.OnLevelLoaded -= onLevelLoaded;
        }
    }
}