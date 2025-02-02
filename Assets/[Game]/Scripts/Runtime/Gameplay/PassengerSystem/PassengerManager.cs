using DG.Tweening;
using Runtime.Core;
using System;
using System.Collections;
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
        public List<PassengerBase> Passengers { get; private set; } = new();
        private BoardManager GridManager => BoardManager.Instance;
        private WaitingAreaManager WaitingAreaManager => WaitingAreaManager.Instance;

        private LevelData levelData => LevelLoader.CurrentLevelData;

        private void Awake()
        {
            PassengerPool = new ObjectPoolBase<PassengerBase>(DataManager.Instance.InstanceContainer.Passenger);
        }

        public void Initialize()
        {
            if(Passengers.Count != 0)
                Passengers.Clear();

            List<StickmanData> stickmen = new();

            if (GameplaySaveSystem.CurrentSaveData != null)
            {
                stickmen = GameplaySaveSystem.CurrentSaveData.LastStickmenDataList;

                // Load waiting passengers
                for (int i = 0; i < GameplaySaveSystem.CurrentSaveData.LastWaitingAreaStickmenDataList.Count; i++)
                {
                    StickmanData waitingStickmanData = GameplaySaveSystem.CurrentSaveData.LastWaitingAreaStickmenDataList[i];
                    PassengerBase passenger = PassengerPool.Get();
                    passenger.transform.position = waitingStickmanData.worldPosition;
                    passenger.SetStickmanData(waitingStickmanData);
                }
            }
            else
                stickmen = levelData.stickmen;

            foreach (StickmanData stickman in stickmen)
            {
                CreatePassenger(stickman);
            }
            CheckSelectables();
        }

        public void AddPassenger(PassengerBase passenger)
        {
            if (Passengers.Contains(passenger)) return;
            Passengers.Add(passenger);
        }

        public void RemovePassenger(PassengerBase passenger)
        {
            if (!Passengers.Contains(passenger)) return;
            Passengers.Remove(passenger);
        }

        public void CreatePassenger(StickmanData stickman)
        {
            Vector3 position = GridManager.GetWorldPosition(stickman.position.x, stickman.position.y);
            PassengerBase passenger = PassengerPool.Get();
            passenger.transform.position = position;
            passenger.SetStickmanData(stickman);
            AddPassenger(passenger);

            GridManager.SetCellWalkable(stickman.position.x, stickman.position.y, false);
        }

        public void HandlePassengerSelection(PassengerBase passenger)
        {
            if (WaitingAreaManager.IsFull)
                return;

            GridManager.SetCellWalkable(passenger.Data.position.x, passenger.Data.position.y, true);

            //float distance = Vector2Int.Distance(passenger.Data.position, passenger.TargetBoardPos);
            List<Vector3> path = FindPath(passenger.Data.position, passenger.TargetBoardPos);
            Vector3 tilePos = WaitingAreaManager.GetAvailableTilePos();
            path.Add(tilePos);
            bool isBoarded = CanBoardVehicle(passenger);
            if (isBoarded)
            {
                path.Add(VehicleManager.Instance.CurrentVehicle.transform.position);
                VehicleManager.Instance.CurrentVehicle.currentPassengers++;
            }
            else
            {
                passenger.Data.worldPosition = tilePos;
                WaitingAreaManager.Instance._currentAvailableSlotCount--;
            }

            passenger.transform.DOPath(path.ToArray(), 1).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (isBoarded)
                        BoardPassenger(passenger, 1);
                    else
                        MoveToWaitingArea(passenger);
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
        private IEnumerator CoCheckSelectables()
        {
            List<PassengerBase> passengersTemp = new(Passengers);
            for (int i = 0; passengersTemp.Count > i; i++)
            {
                PassengerBase passenger = Passengers[i];
                Vector2Int passengerPos = passenger.Data.position;
                if (passenger.IsSelectable)
                    continue;
                if (!BoardManager.Instance.Board.IsValidGridPosition(passenger.Data.position.x, passenger.Data.position.y + 1))
                    passenger.SetPassengerSelectable(true);

                for (int j = 0; j < GridManager.width; j++)
                {
                    Vector2Int gridPos = new(j, GridManager.height - 1);
                    if (IsPathAvailable(passengerPos, gridPos))
                    {
                        passenger.SetPassengerSelectable(true);
                        passenger.TargetBoardPos = gridPos;
                        break;
                    }
                }
                yield return null;
            }
        }
        Coroutine checkCoroutine;
        private void CheckSelectables()
        {
            if (checkCoroutine != null)
            {
                StopCoroutine(checkCoroutine);
                checkCoroutine = null;
            }

            checkCoroutine = StartCoroutine(CoCheckSelectables());
        }
        #endregion
        public bool CanBoardVehicle(PassengerBase passenger)=> VehicleManager.Instance.CanPassengerBoard(passenger);

        private void BoardPassenger(PassengerBase passenger, float arriveTime)
        {
            OnPassengerBoarded?.Invoke(passenger);
            PassengerPool.Release(passenger);
            VehicleManager.Instance.CurrentVehicle.AddPassenger(arriveTime);
        }

        private void MoveToWaitingArea(PassengerBase passenger)
        {
            WaitingAreaManager.Instance.AddStickman(passenger.Data);
            passenger.StartPeeking();
        }
    }
}