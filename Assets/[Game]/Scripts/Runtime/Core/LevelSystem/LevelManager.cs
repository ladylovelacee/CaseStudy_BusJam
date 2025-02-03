using Runtime.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Core
{
    public class LevelManager : Singleton<LevelManager>
    {
        public event Action OnLevelStarted;
        public event Action OnLevelRestart;
        public event Action<bool> OnLevelEnd;

        [field: SerializeField] 
        public List<LevelData> Levels = new();

        [SerializeField]
        public LevelLoader LevelLoader;
        
        public static int CurrentLevel
        {
            get
            {
                return PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentLevel, 1);
            }
            private set
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, value);
            }
        }

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

            if (isSuccess)
                CurrentLevel++;

            GameplaySaveSystem.RemoveGameplayData();
            OnLevelEnd?.Invoke(isSuccess);
        }

        public void RestartLevel()
        {
            IsLevelStarted = false;
            GameplaySaveSystem.RemoveGameplayData();

            OnLevelRestart?.Invoke();
        }

        public void FinishLevel(bool isSuccess, float delay = 0)
        {
            if (!IsLevelStarted) return;
            IsLevelStarted = false;

            if (isSuccess)
                CurrentLevel++;

            GameplaySaveSystem.RemoveGameplayData();
            this.Wait(delay, ()=> OnLevelEnd?.Invoke(isSuccess));
        }
    }
}