using Runtime.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class WaitingAreaManager : Singleton<WaitingAreaManager>
    {
        public const int WaitingSlotCount = 5;

        [SerializeField]
        private Transform[] waitingAreaPos = new Transform[WaitingSlotCount];
        private List<StickmanData> _stickmanData = new();
        private int _currentAvailableSlotCount = WaitingSlotCount;
        public bool IsFull => _currentAvailableSlotCount <= 0;

        public void Initialize(LevelData level)
        {
            _currentAvailableSlotCount = WaitingSlotCount;
        }

        public void AddStickman(StickmanData stickmanData)
        {
            _currentAvailableSlotCount--;
            _stickmanData.Add(stickmanData);

            if(IsFull)
                LevelManager.Instance.CompleteLevel(false);
        }

        public Vector3 GetAvailableTilePos()=> IsFull ? waitingAreaPos[0].position : waitingAreaPos[_currentAvailableSlotCount-1].position;
    }
}