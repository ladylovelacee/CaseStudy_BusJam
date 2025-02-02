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
        public event Action OnLevelCompleted;

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
            CurrentLevel++;

            GameplaySaveSystem.CurrentSaveData = null;
            OnLevelCompleted?.Invoke();
        }

        public void RestartLevel()
        {
            IsLevelStarted = false;
            GameplaySaveSystem.CurrentSaveData = null;

            OnLevelRestart?.Invoke();
        }
    }
}