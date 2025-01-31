using UnityEditor;
using Runtime.Gameplay;

public class LevelEditorUtility
{
    public static void SaveLevel(LevelData levelData)
    {
        string path = "Assets/Levels/" + levelData.name + ".asset";
        AssetDatabase.CreateAsset(levelData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
