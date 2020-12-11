using System.Linq;
using KPU;
using KPU.Manager;
using Scenes.SharedDataEachScenes;
using TMPro;
using UnityEngine;

namespace Scenes.PlayScenes.UI.Scripts
{
    public class StageInfoUIs : MonoBehaviour
    {
        private TMP_Text stageClearInfoText;
        private TMP_Text slowModeInfoText;
        private void Awake()
        {
            stageClearInfoText = transform.GetComponentsInChildren<TMP_Text>()
                .FirstOrDefault(text => text.gameObject.name.Contains("StageClear"));
            if (stageClearInfoText != null) stageClearInfoText.gameObject.SetActive(false);

            slowModeInfoText = transform.GetComponentsInChildren<TMP_Text>()
                .FirstOrDefault(text => text.gameObject.name.Contains("SlowMotionActive"));
            if (slowModeInfoText != null) slowModeInfoText.gameObject.SetActive(false);
        }

        private void Update()
        {
            var currSlowSpeed = SlowMotionManager.Instance.CurrentSlowSpeed;
            if (GameManager.Instance.State != State.Paused)
            {
                if (currSlowSpeed < 0.99f)
                {
                    if (!slowModeInfoText.gameObject.activeInHierarchy) slowModeInfoText.gameObject.SetActive(true);
                    slowModeInfoText.text = $"Slow Mode\nSlow Speed: {currSlowSpeed:0.000}";
                }
                else
                {
                    if (slowModeInfoText != null) slowModeInfoText.gameObject.SetActive(false);
                }
            }

            if (GameManager.Instance.State != State.GameEnded) return;
            if (stageClearInfoText == null) return;
            if (!stageClearInfoText.gameObject.activeInHierarchy)
            {
                stageClearInfoText.gameObject.SetActive(true);
                if (GameStateManager.Instance.healthPoint == 0)
                {
                    stageClearInfoText.text = "Game Over";
                }
            }
        }
    }
}
