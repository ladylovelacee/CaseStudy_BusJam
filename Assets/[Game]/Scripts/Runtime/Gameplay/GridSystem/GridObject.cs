using UnityEngine;

namespace Runtime.Gameplay
{
    public class GridObject : MonoBehaviour
    {
        [HideInInspector] public Vector2Int Position { get; protected set; }
    }
}