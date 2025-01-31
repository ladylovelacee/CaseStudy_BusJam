using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    [CreateAssetMenu(fileName ="Color Container", menuName = "Game/Create Color Container")]
    public class ColorContainer : ScriptableObject
    {
        [SerializeField] private List<ColorData> _colors = new();
        private Dictionary<ColorIDs, Color> _colorsDic = new();

        public void InitializeContainer()
        {
            foreach (var c in _colors)
            {
                _colorsDic.Add(c.Id, c.Color);
            }
        }

        public Color GetColorById(ColorIDs id) => _colorsDic[id];
    }

    [Serializable]
    public class ColorData
    {
        [field: SerializeField] public ColorIDs Id {  get; private set; }
        [field: SerializeField] public Color Color { get; private set; } = Color.white;
    }
}