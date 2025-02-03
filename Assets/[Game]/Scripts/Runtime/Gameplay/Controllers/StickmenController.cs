using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Runtime.Gameplay
{
    public class StickmenController : IController
    {
        public event Action<StickmanGridObj> OnStickmanSelected;
        private List<StickmanGridObj> _stickmen = new();

        private GameplayArea _gameplayArea;
        public StickmenController(GameplayArea gameplayArea)
        {
            _gameplayArea = gameplayArea;
        }

        #region Methods From Interfaces

        public void Initialize()
        {
            _gameplayArea.OnTileObjectSelected += onTileObjectSelected;
        }

        public void Reset()
        {
            _gameplayArea.OnTileObjectSelected -= onTileObjectSelected;

            foreach (StickmanGridObj obj in _stickmen)
            {
                _gameplayArea.StickmanPool.Release(obj);
            }

            _stickmen.Clear();
        }

        public void Dispose()
        {
            _gameplayArea.OnTileObjectSelected -= onTileObjectSelected;
        }
        #endregion

        public void AddStickman(StickmanGridObj stickman)
        {
            if (_stickmen.Contains(stickman)) return;
            _stickmen.Add(stickman);

            stickman.IsSelectable = CheckStickmanSelectable(stickman.Position);
        }

        public void RemoveStickman(StickmanGridObj stickman)
        {
            if (!_stickmen.Contains(stickman)) return;
            _stickmen.Remove(stickman);
        }

        private void onTileObjectSelected(Tile tile)
        {
            if (tile.GridObject is StickmanGridObj)
            {
                StickmanGridObj stickman = tile.GridObject as StickmanGridObj;

                if (_gameplayArea.BusController.BoardToCurrentBus(stickman))
                    MoveStickmanToExit(stickman);
                else if (_gameplayArea.WaitingArea.AddToWaitingArea(stickman))
                    MoveStickmanToExit(stickman);
            }
        }

        private void MoveStickmanToExit(StickmanGridObj selectedStickman)
        {
            Tile startPoint = _gameplayArea.GetTile(selectedStickman.Position);
            startPoint.ClearTileObject();
            RemoveStickman(selectedStickman);

            List<Vector2Int> exists = new(_gameplayArea.exitPoints);
            exists = exists.OrderBy((x) => Vector2Int.Distance(x, selectedStickman.Position)).ToList();

            Debug.Log("moving");
            // TODO: Find path later
        }
        private bool CheckStickmanSelectable(Vector2Int boardPos) => _gameplayArea.exitPoints.Contains(boardPos);
    }

}
