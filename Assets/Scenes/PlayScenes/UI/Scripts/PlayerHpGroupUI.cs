using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.PlayScenes.UI.Scripts
{
    public class PlayerHpGroupUI : MonoBehaviour
    {
        private Image[] heartImageGameObjects;

        private void Awake()
        {
            heartImageGameObjects = GetComponentsInChildren<Image>();
        }

        private void Start()
        {
            for (int i = 0; i < heartImageGameObjects.Length; i++)
                heartImageGameObjects[i].GetComponent<RectTransform>().Rotate(0.0f, i * 45.0f, 0.0f);
        }

        private void Update()
        {
            for (int i = 0; i < heartImageGameObjects.Length; i++)
                heartImageGameObjects[i].GetComponent<RectTransform>().Rotate(0.0f, 100.0f * Time.deltaTime, 0.0f);
        }
    }
}
