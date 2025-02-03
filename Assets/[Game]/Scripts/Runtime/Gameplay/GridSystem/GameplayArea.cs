using Runtime.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class GameplayArea : Singleton<GameplayArea>
    {
        #region Signals
        public Action<Tile> OnTileObjectSelected;
        public Action OnVehicleBoarded;
        #endregion

        #region Gameplay Controllers
        public StickmenController StickmanController { get; private set; }
        public BusController BusController { get; private set; }
        public WaitingAreaController WaitingArea { get; private set; }
        public SelectionController SelectionController { get; private set; }
        #endregion

        #region Pools
        public ObjectPoolBase<Tile> CellPool { get; private set; }
        public ObjectPoolBase<StickmanGridObj> StickmanPool { get; private set; }
        public ObjectPoolBase<Bus> BusPool { get; private set; }
        #endregion

        #region Instances
        [SerializeField] private Tile tileInstance;
        [SerializeField] private StickmanGridObj stickmanInstance;
        [SerializeField] private Bus vehicleInstance;
        #endregion

        #region Board Settings
        private int _width, _height;
        public const float TileSize = 1f;
        #endregion

        #region Markers
        [field: SerializeField] public Transform WaitingAreaFirstTile { get; private set; }
        [field: SerializeField] public Transform BusQueueParent { get; private set; }
        #endregion

        private Dictionary<Vector2Int, Tile> _grid = new();
        public List<Vector2Int> exitPoints = new();

        public LevelData levelData;

        #region Methods From MonoBehaviour
        private void Awake()
        {
            // Pools Initialize

            CellPool = new(tileInstance);
            StickmanPool = new(stickmanInstance);
            BusPool = new(vehicleInstance);

            // Create Controllers
            StickmanController = new(this);
            BusController = new(this);
            WaitingArea = new(this);
            SelectionController = new(this);
        }
        #endregion

        private void Start()
        {
            _width = levelData.width;
            _height = levelData.height;

            GenerateGrid();
            LoadLevelData();
        }

        private void GenerateGrid()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    int index = GetIndex(pos);

                    Tile tile = CellPool.Get();
                    tile.transform.position = new Vector3(x * TileSize, 0, y * TileSize);

                    tile.Init(pos, index, true);
                    _grid[pos] = tile;
                }
            }

            AssignExitPoints();
        }

        private void AssignExitPoints()
        {
            exitPoints.Clear();
            for (int i = 0; i < _width; i++)
            {
                Vector2Int pos = new Vector2Int(i, _height - 1);
                if (_grid.ContainsKey(pos))
                {
                    exitPoints.Add(pos);
                }
            }
        }
        
        private void LoadLevelData()
        {
            if (levelData == null) return;
            _width = levelData.width;
            _height = levelData.height;

            foreach (StickmanData stickman in levelData.stickmen)
            {
                if (_grid.ContainsKey(stickman.position))
                {
                    StickmanGridObj instance = StickmanPool.Get();
                    instance.transform.position = new Vector3(stickman.position.x, 0, stickman.position.y);
                    instance.Initialize(stickman);

                    _grid[stickman.position].SetGridObject(instance);
                    StickmanController.AddStickman(instance);
                }
            }

            InitializeControllers();
        }

        private void InitializeControllers()
        {
            StickmanController.Initialize();
            WaitingArea.Initialize();
            BusController.Initialize();
            SelectionController.Initialize();
        }

        public int GetIndex(Vector2Int position) => _grid.ContainsKey(position) ? position.x + (position.y * _width) : -1;
        public Tile GetTile(Vector2Int position)
        {
            return _grid.ContainsKey(position) ? _grid[position] : null;
        }
        public bool CanStickmanMove(Vector2Int position)
        {
            return _grid.ContainsKey(position) && _grid[position].IsWalkable && !_grid[position].HasObject();
        }
    }
}