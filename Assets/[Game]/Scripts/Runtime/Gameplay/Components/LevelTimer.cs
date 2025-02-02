using Runtime.Core;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class LevelTimer : MonoBehaviour
    {
        public int TimerDuration = 50; //TODO: get duration from level editor

        [SerializeField] private GameObject timerVisual;
        [SerializeField] private TextMeshProUGUI txtRemained;

        private readonly WaitForSecondsRealtime _waitForSecondsRealTime = new(1);

        public static int RemainingSeconds;
        private Coroutine _timerCoroutine;

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            LevelManager.Instance.OnLevelStarted += onLevelStarted;
        }

        private void OnDisable()
        {
            LevelManager.Instance.OnLevelStarted -= onLevelStarted;
        }

        private void Initialize()
        {
            RemainingSeconds = TimerDuration;
            UpdateTimerUI();
        }

        private void onLevelStarted()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerCoroutine = StartCoroutine(TimerCoroutine());
        }

        private IEnumerator TimerCoroutine()
        {

            while (true)
            {
                yield return _waitForSecondsRealTime;

                RemainingSeconds -= 1;
                RemainingSeconds = Mathf.Max(0, RemainingSeconds);

                UpdateTimerUI();

                if (RemainingSeconds <= 0)
                    break;
            }

            LevelManager.Instance.CompleteLevel(false);
        }

        private void UpdateTimerUI()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(RemainingSeconds);
            string formattedTime = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

            /*
            
            #Optimization Alternative - 1

                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                timerText.text = $"{minutes:D2}:{seconds:D2}";

            #Optimization Alternative - 2

                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);

                // StringBuilder

                timeStringBuilder.Clear();
                if (minutes < 10) timeStringBuilder.Append('0');
                timeStringBuilder.Append(minutes);
                timeStringBuilder.Append(':');
                if (seconds < 10) timeStringBuilder.Append('0');
                timeStringBuilder.Append(seconds);

             */

            txtRemained.text = formattedTime;   
        }
    }
}