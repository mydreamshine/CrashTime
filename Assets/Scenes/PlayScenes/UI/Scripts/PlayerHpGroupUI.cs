using System.Collections.Generic;
using System.Linq;
using KPU.Manager;
using Scenes.SharedDataEachScenes;
using UnityEngine;

namespace Scenes.PlayScenes.UI.Scripts
{
    public class PlayerHpGroupUI : MonoBehaviour
    {
        private List<RectTransform> heartImageTransforms;
        private int maxHeartCount;
        public int MaxHeartCount => maxHeartCount;

        private void Awake()
        {
            heartImageTransforms = GetComponentsInChildren<RectTransform>().ToList();
            heartImageTransforms.Remove(GetComponent<RectTransform>());
            maxHeartCount = heartImageTransforms.Count;

            EventManager.On("game_started", OnStarted);

            var inst = GameStateManager.Instance;
        }

        private void OnStarted(object obj)
        {
            for (var i = 0; i < maxHeartCount; i++)
                heartImageTransforms[i].Rotate(0.0f, i * 45.0f, 0.0f);

            RefreshHeart();
        }

        private void Update()
        {
            foreach (var trans in heartImageTransforms)
                trans.Rotate(0.0f, 100.0f * Time.deltaTime, 0.0f);

            RefreshHeart();
        }

        private void RefreshHeart()
        {
            var unValidHeartCount = maxHeartCount - GameStateManager.Instance.healthPoint;
            for (var i = 0; i < unValidHeartCount; i++)
            {
                var removeTarget = heartImageTransforms[heartImageTransforms.Count - 1];
                heartImageTransforms.Remove(removeTarget);
                removeTarget.gameObject.SetActive(false);

                maxHeartCount--;
            }
        }
        
    }
}
