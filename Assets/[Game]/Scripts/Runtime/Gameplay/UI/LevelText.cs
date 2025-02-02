using Runtime.Core;
using TMPro;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class LevelText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txt;

        private void OnEnable()
        {
            UpdateText();
            LevelManager.Instance.OnLevelEnd += onLevelCompleted;
        }

        private void OnDisable()
        {
            LevelManager.Instance.OnLevelEnd -= onLevelCompleted;
        }

        private void onLevelCompleted(bool success)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            txt.text = "Lvl " + LevelManager.CurrentLevel;
        }
    }
}