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
        private TMP_Text clearInfoText;
        private void Awake()
        {
            clearInfoText = transform.GetComponentsInChildren<TMP_Text>()
                .FirstOrDefault(text => text.gameObject.name.Contains("StageClear"));
            if (clearInfoText != null) clearInfoText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (GameManager.Instance.State != State.GameEnded) return;
            if (clearInfoText == null) return;
            if (!clearInfoText.gameObject.activeInHierarchy)
            {
                clearInfoText.gameObject.SetActive(true);
                if (GameStateManager.Instance.healthPoint == 0)
                {
                    clearInfoText.text = "Game Over";
                }
            }
        }
    }
}
