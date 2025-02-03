using Runtime.Core;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class SelectionController : IController
    {
        private GameplayArea _gamePlayArea;
        private InputManager InputManager => InputManager.Instance;
        private LevelManager LevelManager => LevelManager.Instance;

        private Camera mainCamera;

        public SelectionController(GameplayArea gameplayArea)
        {
            _gamePlayArea = gameplayArea;
            mainCamera = Camera.main;
        }

        #region Methods From Interfaces
        public void Dispose()
        {
            InputManager.OnTapDown -= onTapDown;
            LevelManager.OnLevelStarted -= onLevelStarted;
        }

        public void Initialize()
        {
            LevelManager.OnLevelStarted += onLevelStarted;
        }

        public void Reset()
        {
            StopListenToTapInput();
        }
        #endregion

        private void onLevelStarted()
        {
            StartListenToTapInput();
        }

        private void onTapDown()
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(mousePos.x), Mathf.RoundToInt(mousePos.z));

            Tile tile = _gamePlayArea.GetTile(gridPos);
            if (tile != null && tile.HasObject())
            {
                if (tile.GridObject is ISelectable && ((ISelectable)tile.GridObject).IsSelectable)
                    _gamePlayArea.OnTileObjectSelected?.Invoke(tile);
            }
        }

        private void StartListenToTapInput()
        {
            InputManager.OnTapDown += onTapDown;
        }

        private void StopListenToTapInput()
        {
            InputManager.OnTapDown -= onTapDown;
        }
    }
}