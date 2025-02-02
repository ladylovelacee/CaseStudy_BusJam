using UnityEngine;
using UnityEditor;
using Runtime.Gameplay;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;

public class LevelEditor : EditorWindow
{
    private static LevelData _instanceLevelData;
    private float cellSize = 50f;
    private Vector2 _scrollPos;

    private static int _width;
    private static int _height;
    private static string _currentLevelName;
    private static string _lastLoadedLevelName;
    private static int _selectedLevelDataIndex;
    private static LevelData[] _allLevelDataArray;
    private static string[] _levelDataNamesArray = new string[0];

    private readonly static string LevelPathString = "Assets/Resources/Data/LevelData/";
    private readonly static string LevelEditorScenePathString = "Assets/[Game]/Scenes/LevelEditorScene.unity";


    [MenuItem("Tools/Level Editor")]
    public static void ShowWindow()
    {
        EditorWindow levelEditorToolWindow = GetWindow<LevelEditor>("Level Editor");
        EditorSceneManager.OpenScene(LevelEditorScenePathString);
        levelEditorToolWindow.Show();
        ClearEditor();
    }

    private void OnEnable()
    {
        ClearEditor();
    }

    private void OnGUI()
    {
        if (!IsInLevelEditorScene())
            return;

        if (GUILayout.Button("Reload Editor Tool"))
        {
            ClearEditor();
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        //levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

        DrawWidthAndHeight();
        DrawLevelDataTool();
        DrawGrid();
        DrawBusQueueEditor();

        GUILayout.Space(25);

        if (GUILayout.Button("Save Level",GUILayout.Height(100)))
        {
            if (_currentLevelName.Length > 0)
            {
                SaveLevel();
            }
            else
            {
                EditorUtility.DisplayDialog("Level Save Error!", "Please enter level name", "Okay");
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawWidthAndHeight()
    {
        GUILayout.Space(5);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Width");
        EditorGUILayout.LabelField("Height");

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        _width = EditorGUILayout.IntField(_width);
        _height = EditorGUILayout.IntField(_height);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawGrid()
    {
        if (_width > 0 && _height > 0)
        {
            GUILayout.Space(10);
            for (int y = 0; y < _height; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < _width; x++)
                {
                    if (GUILayout.Button(GetCellSymbol(x, y), GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        PlaceObjectAt(x, y);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
    private void DrawBusQueueEditor()
    {
        if (_instanceLevelData == null) return;

        GUILayout.Space(10);
        GUILayout.Label("Bus Queue", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Bus"))
        {
            _instanceLevelData.busQueue.Add(new VehicleData { colorId = ColorIDs.White, arrivalOrder = _instanceLevelData.busQueue.Count + 1 });
        }

        for (int i = 0; i < _instanceLevelData.busQueue.Count; i++)
        {
            GUILayout.BeginHorizontal();

            // Choose bus color
            _instanceLevelData.busQueue[i].colorId = (ColorIDs)EditorGUILayout.EnumPopup(_instanceLevelData.busQueue[i].colorId);

            // Show order count
            GUILayout.Label($"Order: {_instanceLevelData.busQueue[i].arrivalOrder}");

            // Remove the bus button
            if (GUILayout.Button("Remove"))
            {
                _instanceLevelData.busQueue.RemoveAt(i);
            }

            GUILayout.EndHorizontal();
        }
    }


    private string GetCellSymbol(int x, int y)
    {
        foreach (var stickman in _instanceLevelData.stickmen)
            if (stickman.position == new Vector2Int(x, y))
                return "S";

        if (_instanceLevelData.obstacles.Contains(new Vector2Int(x, y)))
            return "X";

        return ".";
    }

    private void PlaceObjectAt(int x, int y)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Stickman"), false, () => AddStickman(x, y));
        menu.AddItem(new GUIContent("Add Obstacle"), false, () => AddObstacle(x, y));
        menu.ShowAsContext();
    }

    private void AddStickman(int x, int y)
    {
        _instanceLevelData.stickmen.Add(new StickmanData { position = new Vector2Int(x, y), stickmanColor = ColorIDs.Blue });
    }

    private void AddObstacle(int x, int y)
    {
        _instanceLevelData.obstacles.Add(new Vector2Int(x, y));
    }

    public static bool IsInLevelEditorScene()
    {
        bool isInCorrectScene = SceneManager.GetActiveScene().name == "LevelEditorScene" && PrefabStageUtility.GetCurrentPrefabStage() == null;
        return isInCorrectScene;
    }

    private static void Initialize()
    {
        if (!IsInLevelEditorScene())
            return;

        _instanceLevelData = CreateInstance<LevelData>();
        LoadLevelData();
    }

    private void DrawLevelDataTool()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Level Data");
        _selectedLevelDataIndex = EditorGUILayout.Popup(_selectedLevelDataIndex, _levelDataNamesArray);

        DrawButtons();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    private void DrawButtons()
    {
        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Level Name");
        _currentLevelName = EditorGUILayout.TextField(_currentLevelName);

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Load Level"))
        {
            LoadLevel();
        }

        if (GUILayout.Button("Clear Level"))
        {
            ClearEditor();
            _selectedLevelDataIndex = 0;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void SaveLevel()
    {
        bool isNewLevel = false;

        LevelData currentLevelData = null;
        LevelData[] levelDataArray = GetAllLevelsDataArray();
        for (int i = 0; i < levelDataArray.Length; i++)
        {
            if (levelDataArray[i].name == _lastLoadedLevelName)
            {
                currentLevelData = levelDataArray[i];
                break;
            }
        }

        isNewLevel = currentLevelData == null;
        _instanceLevelData.height = _height;
        _instanceLevelData.width = _width;
        currentLevelData = _instanceLevelData;

        if (isNewLevel)
        {
            AssetDatabase.CreateAsset(currentLevelData, LevelPathString + _currentLevelName.Trim() + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            string levelPath = AssetDatabase.GetAssetPath(currentLevelData);
            AssetDatabase.RenameAsset(levelPath, _currentLevelName);
        }

        EditorUtility.SetDirty(currentLevelData);
        LoadLevelData();
    }

    private static LevelData[] GetAllLevelsDataArray()
    {
        LevelData[] levelDataArray;

        var guids = AssetDatabase.FindAssets("", new string[] { LevelPathString });
        levelDataArray = new LevelData[guids.Length];

        if (guids.Length > 0)
        {
            for (int i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                levelDataArray[i] = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            }
        }

        return levelDataArray;
    }

    private LevelData GetLevelData()
    {
        LevelData levelData = null;
        string levelName = _levelDataNamesArray[_selectedLevelDataIndex];
        var guids = AssetDatabase.FindAssets(levelName, new[] { LevelPathString });
        if (guids.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        }

        return levelData;
    }

    private void LoadLevel()
    {
        if (_selectedLevelDataIndex > 0)
        {
            ClearEditor();
            var levelData = GetLevelData();

            if (levelData != null)
            {
                _instanceLevelData = levelData;
                _currentLevelName = _instanceLevelData.name;
                _width = _instanceLevelData.width;
                _height = _instanceLevelData.height;
                _lastLoadedLevelName = _instanceLevelData.name;

                Debug.Log("Level loaded!");
            }
            else
            {
                Debug.LogError("Couldn't load this level!");
                return;
            }
        }
    }

    private static void ClearEditor()
    {
        _currentLevelName = null;
        _lastLoadedLevelName = "";
        _width = 0;
        _height = 0;
        Initialize();

        Debug.Log("Level Cleared!");
    }

    private static void LoadLevelData()
    {
        _allLevelDataArray = GetAllLevelsDataArray();
        _levelDataNamesArray = _allLevelDataArray.Select(x => x.name).Prepend("None").ToArray();
    }
}
