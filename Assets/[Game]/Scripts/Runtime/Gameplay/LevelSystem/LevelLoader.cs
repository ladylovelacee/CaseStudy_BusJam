using UnityEngine;

namespace Runtime.Gameplay
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField]public LevelData level; // TODO: get level container

        void Start()
        {
            LoadLevel(level);
        }

        void LoadLevel(LevelData levelData)
        {
            BoardManager.Instance.Initialize(level);
            PassengerManager.Instance.Initialize(level);
            VehicleManager.Instance.Initialize(level);
        }
    }
}