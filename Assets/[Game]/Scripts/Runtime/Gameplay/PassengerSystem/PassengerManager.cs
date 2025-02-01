using DG.Tweening;
using Runtime.Core;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class PassengerManager : Singleton<PassengerManager>
    {
        #region Events
        public event Action<PassengerBase> OnPassengerBoarded;
        public event Action<PassengerBase> OnPassengerMovedToWaitingArea;
        #endregion

        public ObjectPoolBase<PassengerBase> PassengerPool { get; private set; }

        private List<PassengerBase> _passengers = new();
        private GridManager GridManager => GridManager.Instance;
        private int waitingAreaSlots = 3;
        private int currentWaitingCount = 0;

        private void Awake()
        {
            PassengerPool = new ObjectPoolBase<PassengerBase>(DataManager.Instance.InstanceContainer.Passenger);
        }

        public void Initialize(LevelData data)
        {
            if(_passengers.Count != 0)
                _passengers.Clear();

            List<StickmanData> stickmen = data.stickmen;
            foreach (StickmanData stickman in stickmen)
            {
                CreatePassenger(stickman);
            }

            CheckSelectables();
        }

        public void AddPassenger(PassengerBase passenger)
        {
            if (_passengers.Contains(passenger)) return;
            _passengers.Add(passenger);
        }

        public void RemovePassenger(PassengerBase passenger)
        {
            if (!_passengers.Contains(passenger)) return;
            _passengers.Remove(passenger);
            //if (_passengers.Count <= 0)
            //    LevelManager.Instance.CompleteLevel(true); //Can be ERROR!
        }

        public void CreatePassenger(StickmanData stickman)
        {
            Vector3 position = GridManager.GetWorldPosition(stickman.position.x, stickman.position.y);
            PassengerBase passenger = PassengerPool.Get();
            passenger.transform.position = position;
            passenger.Initialize(stickman.stickmanColor, stickman.position);
            AddPassenger(passenger);

            GridManager.SetCellWalkable(stickman.position.x, stickman.position.y, false);
        }

        public void HandlePassengerSelection(PassengerBase passenger)
        {
            passenger.transform.DOPath(FindPath(passenger.Position, passenger.TargetPos).ToArray(), 2f)
                .OnComplete(() =>
                {
                    if (CanBoardVehicle(passenger))
                    {
                        BoardPassenger(passenger);
                    }
                    else if (currentWaitingCount < waitingAreaSlots)
                    {
                        MoveToWaitingArea(passenger);
                    }
                    else
                    {
                        LevelManager.Instance.CompleteLevel(false);
                    }
                });

            CheckSelectables();           
        }

        #region Movement Control
        private List<Vector3> FindPath(Vector2Int startPos,Vector2Int targetPos)
        {
            JobHandle handle = Pathfinding.FindPath(new int2(GridManager.width, GridManager.height), new int2(startPos.x, startPos.y), new int2(targetPos.x, targetPos.y), out NativeList<int2> path);
            handle.Complete();
            List<Vector3> findedPath = new();
            for (int i = path.Length - 1; i >= 0; i--)
            {
                findedPath.Add(GridManager.GetWorldPosition(path[i].x, path[i].y));
            }
            path.Dispose();
            return findedPath;
        }

        private bool IsPathAvailable(Vector2Int startPos, Vector2Int targetPos)
        {
            JobHandle handle = Pathfinding.FindPath(new int2(GridManager.width, GridManager.height), new int2(startPos.x, startPos.y), new int2(targetPos.x, targetPos.y), out NativeList<int2> path);
            handle.Complete();
            return path.Length == 0 ? false : true;
        }
        #endregion

        #region Selectable Control
        private void CheckSelectables()
        {
            List<PassengerBase> passengersTemp = new(_passengers);
            for (int i = 0; passengersTemp.Count > i; i++)
            {
                PassengerBase passenger = _passengers[i];
                Vector2Int passengerPos = passenger.Position;
                if (passenger.IsSelectable)
                    continue;

                for (int j = 0; j < GridManager.width; j++)
                {
                    Vector2Int gridPos = new(j, GridManager.height - 1);
                    if (IsPathAvailable(passengerPos, gridPos))
                    {
                        passenger.SetPassengerSelectable(true);
                        passenger.TargetPos = gridPos;
                        break;
                    }
                }
            }
        }
        #endregion
        private bool CanBoardVehicle(PassengerBase passenger)=> VehicleManager.Instance.CanPassengerBoard(passenger);

        private void BoardPassenger(PassengerBase passenger)
        {
            OnPassengerBoarded?.Invoke(passenger);
            passenger.transform.SetParent(VehicleManager.Instance.CurrentVehicle.transform);
            passenger.transform.DOMove(VehicleManager.Instance.CurrentVehicle.transform.position, .5f)
                .OnComplete(VehicleManager.Instance.CurrentVehicle.AddPassenger);
        }

        private void MoveToWaitingArea(PassengerBase passenger)
        {
            currentWaitingCount++;
            OnPassengerMovedToWaitingArea?.Invoke(passenger);
            if(currentWaitingCount >= waitingAreaSlots)
                LevelManager.Instance.CompleteLevel(false);
        }
    }
}