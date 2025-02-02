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
        List<StickmanData> currentStickmenDataList = new();

        foreach (var passenger in PassengerManager.Instance.Passengers)
        {
            currentStickmenDataList.Add(passenger.Data);
        }

        List<StickmanData> currentWaitingAreaStickmenDataList = WaitingAreaManager.Instance.WaitingsData;
        Queue<VehicleData> currentBusQueue = VehicleManager.Instance.busQueue;
        int currentBusPassengerCount = VehicleManager.Instance.CurrentVehicle.GetCurrentPassengerCount();
        float currentGameTime = LevelTimer.RemainingSeconds;

        CurrentSaveData = new GameplaySaveData(currentStickmenDataList, currentBusQueue, currentBusPassengerCount, currentWaitingAreaStickmenDataList, currentGameTime);

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
        public Queue<VehicleData> BusQueue;
        public int LastBusPassengerCount;
        public List<StickmanData> LastWaitingAreaStickmenDataList;
        public float LastGameTime;

        public GameplaySaveData(List<StickmanData> lastStickmenDataList, Queue<VehicleData> busQueue, int lastBusPassengerCount, List<StickmanData> lastWaitingAreaStickmenDataList, float lastGameTime)
        {
            LastStickmenDataList = lastStickmenDataList;
            BusQueue = busQueue;
            LastBusPassengerCount = lastBusPassengerCount;
            LastWaitingAreaStickmenDataList = lastWaitingAreaStickmenDataList;
            LastGameTime = lastGameTime;
        }
    }
}
