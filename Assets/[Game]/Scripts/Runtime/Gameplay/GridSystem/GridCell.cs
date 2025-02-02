using Runtime.Core;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridCell : MonoBehaviour 
    {
        // TODO: set grid visual
        private void OnEnable()
        {
            LevelManager.Instance.LevelLoader.OnLevelStartLoading += onLevelStartLoading;
        }

        private void OnDisable()
        {
            LevelManager.Instance.LevelLoader.OnLevelStartLoading -= onLevelStartLoading;
        }

        private void onLevelStartLoading()
        {
            BoardManager.Instance.CellPool.Release(this);
        }
    }
}