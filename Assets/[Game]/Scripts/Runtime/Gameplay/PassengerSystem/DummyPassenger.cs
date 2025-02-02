using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPassenger : MonoBehaviour
{
    #region Properties
    [field: SerializeField] public Renderer Renderer { get; private set; }

    #endregion

    public void SetColor(Color color)
    {
        MaterialPropertyBlock block = new();
        block.SetColor("_BaseColor", color);
        Renderer.SetPropertyBlock(block);
    }
}
