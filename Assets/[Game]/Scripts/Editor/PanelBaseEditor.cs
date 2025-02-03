using Runtime.Core;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PanelBase))]
public class PanelBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); 
        PanelBase panelBase = (PanelBase)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Show Panel"))
        {
            panelBase.OpenPanel();
        }

        if (GUILayout.Button("Hide Panel"))
        {
            panelBase.ClosePanel();
        }
    }
}
