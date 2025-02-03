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
    private static int _levelDuration;
    private static string _currentLevelName;
    private static string _lastLoadedLevelName;
    private static int _selectedLevelDataIndex;
    private static LevelData[] _allLevelDataArray;
    private static string[] _levelDataNamesArray = new string[0];

    private readonly static string LevelPathString = "Assets/Resources/Data/LevelData/";
    private readonly static string LevelDataInstancePathString = "Assets/[Game]/Scripts/Level_Data_Instance.asset";
    private readonly static string LevelEditorScenePathString = "Assets/[Game]/Scenes/LevelEditorScene.unity";
    private readonly static string MainScenePathString = "Assets/[Game]/Scenes/Game.unity";


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

        if (GUILayout.Button("Go to game main scene to play", GUILayout.Height(50)))
        {
            EditorSceneManager.OpenScene(MainScenePathString);
        }

        if (GUILayout.Button("Reload Editor Tool", GUILayout.Height(50)))
        {
            ClearEditor();
        }

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

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
                return stickman.stickmanColor.ToString();

        if (_instanceLevelData.obstacles.Contains(new Vector2Int(x, y)))
            return "X";

        return ".";
    }

    private void PlaceObjectAt(int x, int y)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddSeparator("Add Stickman/");
        menu.AddItem(new GUIContent("Add Stickman/Green"), false, () => AddStickman(x, y, ColorIDs.Green));
        menu.AddItem(new GUIContent("Add Stickman/Blue"), false, () => AddStickman(x, y, ColorIDs.Blue));
        menu.AddItem(new GUIContent("Add Stickman/Yellow"), false, () => AddStickman(x, y, ColorIDs.Yellow));
        menu.AddItem(new GUIContent("Add Stickman/Orange"), false, () => AddStickman(x, y, ColorIDs.Orange));
        menu.AddItem(new GUIContent("Add Stickman/White"), false, () => AddStickman(x, y, ColorIDs.White));
        menu.AddItem(new GUIContent("None"), false, () => Remove(x, y));
        menu.ShowAsContext();
    }

    private void AddStickman(int x, int y, ColorIDs colorType)
    {
        for (int i = 0; i < _instanceLevelData.stickmen.Count; i++) {

            if (_instanceLevelData.stickmen[i].position == new Vector2Int(x, y))
            {
                _instanceLevelData.stickmen[i].stickmanColor = colorType;
                return;
            }
        } 
        _instanceLevelData.stickmen.Add(new StickmanData { position = new Vector2Int(x, y), stickmanColor = colorType });
    }

    private void Remove(int x, int y)
    {
        foreach (var stickman in _instanceLevelData.stickmen)
        {
            if (stickman.position == new Vector2Int(x, y))
            {
                _instanceLevelData.stickmen.Remove(stickman);
                break;
            }
        }
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

        _instanceLevelData = AssetDatabase.LoadAssetAtPath<LevelData>(LevelDataInstancePathString);
        if( _instanceLevelData == null)
        {
            _instanceLevelData = CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(_instanceLevelData, LevelDataInstancePathString);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(_instanceLevelData);
        }
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

        EditorGUILayout.LabelField("Level Duration");
        _levelDuration = EditorGUILayout.IntField(_levelDuration);

        EditorGUILayout.Space(10);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Load Level"))
        {
            LoadLevel();
        }

        if(GUILayout.Button("Clear Grid"))
        {
            _instanceLevelData.stickmen.Clear();
            _instanceLevelData.obstacles.Clear();
        }

        if (GUILayout.Button("Clear Editor"))
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
        _instanceLevelData.duration = _levelDuration;

        if (isNewLevel)
        {
            currentLevelData = CreateInstance<LevelData>();
            currentLevelData = LevelData.CopyLevelData(_instanceLevelData);
            AssetDatabase.CreateAsset(currentLevelData, LevelPathString + _currentLevelName.Trim() + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            currentLevelData = LevelData.CopyLevelData(_instanceLevelData);
            string levelPath = AssetDatabase.GetAssetPath(currentLevelData);
            AssetDatabase.RenameAsset(levelPath, _currentLevelName);
        }

        EditorUtility.SetDirty(currentLevelData);
        
        _instanceLevelData.ResetLevelData();
        EditorUtility.SetDirty(_instanceLevelData);
        LoadLevelData();

        EditorUtility.DisplayDialog("Level Successfully Created", currentLevelData.name, "Okay");
        ClearEditor();
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
                _instanceLevelData = LevelData.CopyLevelData(levelData);
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
        _levelDuration = 0;
        Initialize();

        Debug.Log("Level Cleared!");
    }

    private static void LoadLevelData()
    {
        _allLevelDataArray = GetAllLevelsDataArray();
        _levelDataNamesArray = _allLevelDataArray.Select(x => x.name).Prepend("None").ToArray();
    }
}
