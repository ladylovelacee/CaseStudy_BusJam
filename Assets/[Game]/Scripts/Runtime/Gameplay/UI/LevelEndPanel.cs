using Runtime.Core;

namespace Runtime.Gameplay
{
    public class LevelEndPanel : PanelBase
    {
        private void OnEnable()
        {
            LevelManager.Instance.OnLevelCompleted += onLevelCompleted;
        }

        private void onLevelCompleted()
        {
            OpenPanel();
            LevelManager.Instance.LevelLoader.LoadLevel();
        }

        public override void Dispose()
        {
            base.Dispose();
            LevelManager.Instance.OnLevelCompleted -= onLevelCompleted;
        }
    }
}