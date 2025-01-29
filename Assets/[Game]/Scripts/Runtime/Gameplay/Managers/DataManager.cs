using Runtime.Core;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class DataManager : Singleton<DataManager>
    {
        [field : SerializeField] public ColorContainer ColorContainer {  get; private set; }
    }
}