using Runtime.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class PassengerManager : Singleton<PassengerManager>
    {
        private List<PassengerBase> _passengers = new();
        private Dictionary<int, PassengerBase> _passengerDic = new();
        private int _index;
        private GridManager GridManager => GridManager.Instance;

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
    }
}