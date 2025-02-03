using DG.Tweening;
using Runtime.Core;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Gameplay
{
    public class LevelLoader : Singleton<LevelLoader>
    {
        public event Action OnLevelStartLoading;
        public event Action OnLevelLoaded;

        public static LevelData CurrentLevelData;
        private LevelManager LevelManager => LevelManager.Instance;

        private void Start()
        {
            LoadLevel();
        }

        private void InitializeLevel()
        {
            BoardManager.Instance.Initialize();
            PassengerManager.Instance.Initialize();
            VehicleManager.Instance.Initialize();
            WaitingAreaManager.Instance.Initialize();

            DOVirtual.DelayedCall(.5f,()=> OnLevelLoaded?.Invoke());
        }

        public void LoadLevel() 
        {
            OnLevelStartLoading?.Invoke();

            if (LevelManager.CurrentLevel - 1 > LevelManager.Levels.Count - 1)
            {
                int index = Random.Range(0, LevelManager.Instance.Levels.Count);
                CurrentLevelData = LevelManager.Instance.Levels[index];
            }
            else
                CurrentLevelData = LevelManager.Instance.Levels[LevelManager.CurrentLevel - 1];

            InitializeLevel();
        }
    }
}