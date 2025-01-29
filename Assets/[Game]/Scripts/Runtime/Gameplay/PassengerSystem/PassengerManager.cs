using Runtime.Core;
using System.Collections.Generic;

namespace Runtime.Gameplay
{
    public class PassengerManager : Singleton<PassengerManager>
    {
        private List<PassengerBase> _passengers = new();

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
    }
}