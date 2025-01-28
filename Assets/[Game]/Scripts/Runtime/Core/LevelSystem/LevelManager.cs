using System;

namespace Runtime.Core
{
    public class LevelManager : Singleton<LevelManager>
    {
        public event Action OnLevelStarted;

        public bool IsLevelStarted {  get; private set; }

        public void StartLevel()
        {
            if (IsLevelStarted) return;
            IsLevelStarted = true;
            OnLevelStarted?.Invoke();
        }

        public void CompleteLevel(bool isSuccess)
        {
            if (!IsLevelStarted) return;
            IsLevelStarted = false;
        }
    }
}