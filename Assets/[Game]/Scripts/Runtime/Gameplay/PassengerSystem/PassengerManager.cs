using Runtime.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class PassengerManager : Singleton<PassengerManager>
    {
        public event Action<PassengerBase> OnPassengerBoarded;
        public event Action<PassengerBase> OnPassengerMovedToWaitingArea;

        private List<PassengerBase> _passengers = new();
        private Dictionary<int, PassengerBase> _passengerDic = new();
        private int _index;
        private GridManager GridManager => GridManager.Instance;
        private int waitingAreaSlots = 3;
        private int currentWaitingCount = 0;

        private void Start()
        {
            for (int i = 0; i < 20; i++)
            {
                CreatePassenger();
            }
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
            if (_passengers.Count <= 0)
                LevelManager.Instance.CompleteLevel(true);
        }

        public void CreatePassenger()
        {
            GridManager.Board.GetXYFromIndex(_index, out int x, out int y);
            PassengerBase passenger = Instantiate(DataManager.Instance.InstanceContainer.Passenger,
                GridManager.GetWorldPosition(x,y), 
                Quaternion.identity); // TODO: Get from pool
            if(y == 1) // TODO: set 1 to board height
                passenger.SetPassengerSelectable(true);
            _index++;
        }

        public void HandlePassengerSelection(PassengerBase passenger)
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
        }

        private bool CanBoardVehicle(PassengerBase passenger)=> VehicleManager.Instance.CanPassengerBoard(passenger);

        private void BoardPassenger(PassengerBase passenger)
        {
            Debug.Log($"{passenger.name} boarded the bus!");
            OnPassengerBoarded?.Invoke(passenger);
        }

        private void MoveToWaitingArea(PassengerBase passenger)
        {
            currentWaitingCount++;
            OnPassengerMovedToWaitingArea?.Invoke(passenger);
        }
    }
}