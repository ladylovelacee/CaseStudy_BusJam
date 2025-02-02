using Runtime.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Gameplay
{
    public class LevelEndPanel : PanelBase
    {
        [SerializeField] TextMeshProUGUI levelEndText;
        [SerializeField] TextMeshProUGUI continueButtonText;
        [SerializeField] Button continueButton;

        private void OnEnable()
        {
            LevelManager.Instance.OnLevelEnd += onLevelEnd;
            LevelManager.Instance.LevelLoader.OnLevelLoaded += onLevelLoaded;

            continueButton.onClick.AddListener(ContinueButton);
        }

        private void onLevelEnd(bool success)
        {
            OpenPanel();
            levelEndText.text = success ? "Level Cleared" : "Level Failed";
            continueButtonText.text = success ? "Next Level" : "Try Again";
        }

        private void onLevelLoaded()
        {
            ClosePanel();
        }

        public override void Dispose()
        {
            base.Dispose();
            LevelManager.Instance.OnLevelEnd -= onLevelEnd;
            LevelManager.Instance.LevelLoader.OnLevelLoaded -= onLevelLoaded;

            continueButton.onClick.RemoveListener(ContinueButton);
        }

        private void ContinueButton()
        {
            ClosePanel();
            LevelManager.Instance.LevelLoader.LoadLevel();
        }
    }
}
