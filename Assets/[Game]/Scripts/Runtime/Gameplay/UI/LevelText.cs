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
            LevelManager.Instance.OnLevelCompleted += onLevelCompleted;
        }

        private void OnDisable()
        {
            LevelManager.Instance.OnLevelCompleted -= onLevelCompleted;
        }

        private void onLevelCompleted()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            txt.text = "Lvl " + LevelManager.CurrentLevel;
        }
    }
}