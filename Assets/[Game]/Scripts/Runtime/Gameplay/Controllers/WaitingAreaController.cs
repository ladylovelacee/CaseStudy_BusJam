using Runtime.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class WaitingAreaController : IController
    {
        private GameplayArea _gameplayArea;
        private LevelManager LevelManager => LevelManager.Instance;
        private List<StickmanGridObj> _waitingStickmen = new();
        private Vector3 _waitingPosition;

        private const int MaxCapacity = 5;
        private const float Offset = .5f;

        #region Constructor
        public WaitingAreaController(GameplayArea gameplayArea)
        {
            _gameplayArea = gameplayArea;
            _waitingPosition = _gameplayArea.WaitingAreaFirstTile.position;
        }
        #endregion

        #region Methods From Interfaces
        public void Dispose()
        {
            _gameplayArea.OnVehicleBoarded -= onVehicleBoarded;
        }

        public void Initialize()
        {
            _gameplayArea.OnVehicleBoarded += onVehicleBoarded;
        }

        public void Reset()
        {
            _gameplayArea.OnVehicleBoarded -= onVehicleBoarded;
            foreach (var obj in _waitingStickmen)
            {
                _gameplayArea.StickmanPool.Release(obj);
            }
            _waitingStickmen.Clear();
        }
        #endregion

        public bool AddToWaitingArea(StickmanGridObj stickman)
        {
            if(_waitingStickmen.Contains(stickman)) return false;
            if (_waitingStickmen.Count < MaxCapacity)
            {
                _waitingStickmen.Add(stickman);
                Vector3 stickmanPos = _waitingPosition + new Vector3(_waitingStickmen.Count * Offset, 0, 0);
                return true;
            }
            else
            {
                LevelManager.FinishLevel(false, 1f);
                return false;
            }
        }

        private void RemoveFromWaitingArea(StickmanGridObj stickman)
        {
            if (!_waitingStickmen.Contains(stickman)) return;
            _waitingStickmen.Remove(stickman);
        }

        private void onVehicleBoarded()
        {
            CheckWaitersState();
        }

        private void CheckWaitersState()
        {
            foreach (StickmanGridObj stickman in _waitingStickmen)
            {
                if (_gameplayArea.BusController.BoardToCurrentBus(stickman))
                {
                    RemoveFromWaitingArea(stickman);
                }
            }
        }
    }
}