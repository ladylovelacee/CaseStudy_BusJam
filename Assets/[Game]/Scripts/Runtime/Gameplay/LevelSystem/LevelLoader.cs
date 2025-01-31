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
            Debug.Log("Level Loaded: " + levelData.width + "x" + levelData.height);
        }
    }
}