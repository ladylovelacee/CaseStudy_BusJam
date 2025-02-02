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
        public List<StickmanData> WaitingsData { get; private set; } = new();
        public int _currentAvailableSlotCount = WaitingSlotCount;
        public bool IsFull => _currentAvailableSlotCount <= 0;

        public void Initialize()
        {
            _currentAvailableSlotCount = WaitingSlotCount;

            if (GameplaySaveSystem.CurrentSaveData != null)
            {
                foreach (StickmanData stickman in GameplaySaveSystem.CurrentSaveData.LastWaitingAreaStickmenDataList)
                {
                    AddStickman(stickman);
                }
            }
        }

        public void AddStickman(StickmanData stickmanData)
        {
            WaitingsData.Add(stickmanData);

            if(IsFull)
                LevelManager.Instance.CompleteLevel(false);
        }

        public Vector3 GetAvailableTilePos()=> IsFull ? waitingAreaPos[0].position : waitingAreaPos[_currentAvailableSlotCount-1].position;
    }
}