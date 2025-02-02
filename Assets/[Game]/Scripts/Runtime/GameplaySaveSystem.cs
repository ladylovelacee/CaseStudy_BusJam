using Runtime.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySaveSystem : MonoBehaviour
{
    public static GameplaySaveData CurrentSaveData;

    private readonly static string GameplayDataPrefKey = "gameplay_save_data";

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        LoadGameplayData();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveGameplayData();
        }
    }

    public static void SaveGameplayData()
    {
        List<StickmanData> currentStickmenDataList = new List<StickmanData>();
        List<StickmanData> currentWaitingAreaStickmenDataList = new List<StickmanData>();
        int currentBusOrderIndex = 0;
        int currentBusPassengerCount = 0;
        float currentGameTime = 0;

        CurrentSaveData = new GameplaySaveData(currentStickmenDataList, currentBusOrderIndex, currentBusPassengerCount, currentWaitingAreaStickmenDataList, currentGameTime);

        string jsonString = JsonUtility.ToJson(CurrentSaveData);
        PlayerPrefs.SetString(GameplayDataPrefKey, jsonString);
    }

    public static void RemoveGameplayData()
    {
        // Remove data after pass level.
        CurrentSaveData = null;
        PlayerPrefs.SetString(GameplayDataPrefKey, "");
    }

    private static void LoadGameplayData()
    {
        string jsonString = PlayerPrefs.GetString(GameplayDataPrefKey, "");
        if (jsonString != "")
        {
            CurrentSaveData = JsonUtility.FromJson<GameplaySaveData>(jsonString);
        }
        else
        {
            CurrentSaveData = null;
        }
    }

    [Serializable]
    public class GameplaySaveData
    {
        public List<StickmanData> LastStickmenDataList;
        public int LastBusOrderIndex;
        public int LastBusPassengerCount;
        public List<StickmanData> LastWaitingAreaStickmenDataList;
        public float LastGameTime;

        public GameplaySaveData(List<StickmanData> lastStickmenDataList, int lastBusOrderIndex, int lastBusPassengerCount, List<StickmanData> lastWaitingAreaStickmenDataList, float lastGameTime)
        {
            LastStickmenDataList = lastStickmenDataList;
            LastBusOrderIndex = lastBusOrderIndex;
            LastBusPassengerCount = lastBusPassengerCount;
            LastWaitingAreaStickmenDataList = lastWaitingAreaStickmenDataList;
            LastGameTime = lastGameTime;
        }
    }
}
