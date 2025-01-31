using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Runtime.Gameplay;

public class LevelEditor : EditorWindow
{
    private LevelData levelData;
    private int gridSize = 8;
    private float cellSize = 50f;

    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);

        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

        if (levelData == null) return;

        DrawGrid();
        DrawBusQueueEditor();
    }

    private void DrawGrid()
    {
        GUILayout.Space(10);
        for (int y = 0; y < levelData.height; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < levelData.width; x++)
            {
                if (GUILayout.Button(GetCellSymbol(x, y), GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    PlaceObjectAt(x, y);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
    private void DrawBusQueueEditor()
    {
        GUILayout.Space(10);
        GUILayout.Label("Bus Queue", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Bus"))
        {
            levelData.busQueue.Add(new VehicleData { colorId = Color.white, arrivalOrder = levelData.busQueue.Count + 1 });
            EditorUtility.SetDirty(levelData);
        }

        for (int i = 0; i < levelData.busQueue.Count; i++)
        {
            GUILayout.BeginHorizontal();

            // Otobüs rengi seçme
            levelData.busQueue[i].colorId = EditorGUILayout.ColorField(levelData.busQueue[i].colorId, GUILayout.Width(50));

            // Sýra numarasý gösterme
            GUILayout.Label($"Order: {levelData.busQueue[i].arrivalOrder}");

            // Otobüsü silme butonu
            if (GUILayout.Button("Remove"))
            {
                levelData.busQueue.RemoveAt(i);
                EditorUtility.SetDirty(levelData);
            }

            GUILayout.EndHorizontal();
        }
    }


    private string GetCellSymbol(int x, int y)
    {
        foreach (var stickman in levelData.stickmen)
            if (stickman.position == new Vector2Int(x, y))
                return "S";

        foreach (var cabin in levelData.cabins)
            if (cabin.position == new Vector2Int(x, y))
                return "C";

        if (levelData.obstacles.Contains(new Vector2Int(x, y)))
            return "X";

        return ".";
    }

    private void PlaceObjectAt(int x, int y)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Stickman"), false, () => AddStickman(x, y));
        menu.AddItem(new GUIContent("Add Cabin"), false, () => AddCabin(x, y));
        menu.AddItem(new GUIContent("Add Obstacle"), false, () => AddObstacle(x, y));
        menu.ShowAsContext();
    }

    private void AddStickman(int x, int y)
    {
        levelData.stickmen.Add(new StickmanData { position = new Vector2Int(x, y), stickmanColor = Color.blue });
        EditorUtility.SetDirty(levelData);
    }

    private void AddCabin(int x, int y)
    {
        levelData.cabins.Add(new CabinData { position = new Vector2Int(x, y), containedColors = new List<Color> { Color.blue, Color.red } });
        EditorUtility.SetDirty(levelData);
    }

    private void AddObstacle(int x, int y)
    {
        levelData.obstacles.Add(new Vector2Int(x, y));
        EditorUtility.SetDirty(levelData);
    }
}
