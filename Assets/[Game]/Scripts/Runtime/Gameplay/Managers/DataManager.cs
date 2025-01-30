using Runtime.Core;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class DataManager : Singleton<DataManager>
    {
        [field : SerializeField] public ColorContainer ColorContainer {  get; private set; }
        [field: SerializeField] public InstanceContainer InstanceContainer { get; private set; } = new();

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            ColorContainer.InitializeContainer();
        }
    }

    [System.Serializable]
    public class InstanceContainer
    {
        [field: SerializeField] public PassengerBase Passenger { get; private set; }
        [field: SerializeField] public VehicleBase Vehicle { get; private set; }
        [field: SerializeField] public GridCell Cell { get; private set; }
    }
}