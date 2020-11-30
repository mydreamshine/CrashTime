using TMPro;
using UnityEngine;

namespace Scenes.GameOverScene.Scripts.UI
{
    public class InputFieldUserNameUI : MonoBehaviour
    {
        [SerializeField] private RectTransform inputFieldSpawnPoints;
        [SerializeField] private GameObject rankListUIGameObject;

        private int inputFieldSpawnIndex;
        private int inputFieldSpawnCount;
        private TMP_InputField inputField;
        private int selectedAnchorPos;
        private int selectedFocusPos;

        public int GetInputFieldSpawnIndex() => inputFieldSpawnIndex;
        public int GetInputFieldSpawnCount() => inputFieldSpawnCount;
        public string GetCompletedPlayerName() => inputField.text.Length == 3 ? inputField.text : null;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
            
            if (inputFieldSpawnPoints == null) inputFieldSpawnPoints = GetComponent<RectTransform>();
            
            inputFieldSpawnCount = inputFieldSpawnPoints != null ? inputFieldSpawnPoints.childCount : 0;
        }

        private void Start()
        {
            inputField.onValidateInput += (input, charIndex, addedChar) => NameValidation(addedChar);
            inputField.onSubmit.AddListener(NameFieldEnter);
            inputField.characterLimit = 3;
            inputField.text = 'A'.ToString();
            inputField.Select();

            InitCaretFocus();
        }

        private void Update()
        {
            if (inputField.selectionAnchorPosition != selectedAnchorPos)
            {
                if (inputField.text.Length < 3) inputField.text += 'A';
                
                selectedAnchorPos = inputField.selectionAnchorPosition;
                selectedFocusPos = selectedAnchorPos + 1;
                
                inputField.selectionFocusPosition = selectedFocusPos;
                inputField.ForceLabelUpdate();
            }
            else if (inputField.selectionFocusPosition != selectedFocusPos)
            {
                if (inputField.selectionFocusPosition == 0) inputField.selectionFocusPosition = 1;
                
                selectedFocusPos = inputField.selectionFocusPosition;
                selectedAnchorPos = selectedFocusPos > 0 ? selectedFocusPos - 1 : 0;

                inputField.selectionAnchorPosition = selectedAnchorPos;
                inputField.ForceLabelUpdate();
            }

            if (Input.GetKeyDown(KeyCode.Return) && inputField.text.Length < 3)
            {
                while (inputField.text.Length < 3) inputField.text += 'A';
                
                inputField.ActivateInputField();
                inputField.Select();
                inputField.selectionAnchorPosition = selectedAnchorPos = 2;
                inputField.selectionFocusPosition = selectedFocusPos = 3;
                inputField.ForceLabelUpdate();
            }
        }

        private char NameValidation(char c)
        {
            if ('a' <= c && c <= 'z')
                return (char) (c - ('a' - 'A'));
            if (('A' <= c && c <= 'Z') || ('0' <= c && c <= '9'))
                return c;
            
            return '\0';
        }
        
        private void NameFieldEnter(string text)
        {
            if (inputField.text.Length == 3)
            {
                var rankListUI = rankListUIGameObject.GetComponent<RankListUI>();
                rankListUI.SaveRankData();
                gameObject.SetActive(false);
            }
        }

        private void InitCaretFocus()
        {
            selectedAnchorPos = 0;
            selectedFocusPos = selectedAnchorPos + 1;
            inputField.selectionAnchorPosition = selectedAnchorPos;
            inputField.selectionFocusPosition = selectedFocusPos;
            inputField.ForceLabelUpdate();
        }

        public void CalculateInputFieldSpawnIndex(int rankListCount, int selectedRank)
        {
            var selectedRankIndex = selectedRank - 1;
            
            inputFieldSpawnIndex = selectedRankIndex;
            if (rankListCount > inputFieldSpawnCount)
            {
                var inputFieldSpawnCountPivot = inputFieldSpawnCount / 2;
                var inputFieldIndexSub = (rankListCount - 1) - selectedRankIndex;
                if (inputFieldIndexSub > inputFieldSpawnCountPivot)
                    inputFieldSpawnIndex = (inputFieldSpawnCount / 2) - 1;
                else
                    inputFieldSpawnIndex = (inputFieldSpawnCount - 1) - inputFieldIndexSub;
            }

            SetInputFieldSpawnIndex(inputFieldSpawnIndex);
        }
        
        private void SetInputFieldSpawnIndex(int index)
        {
            if (inputFieldSpawnPoints == null) return;
            if (inputFieldSpawnPoints.childCount < index) return;
            
            var selectedTransform = inputFieldSpawnPoints.GetChild(index).GetComponent<RectTransform>();
            var rectTransform = GetComponent<RectTransform>();
            
            var spawnPoint = selectedTransform.anchoredPosition;
            rectTransform.anchoredPosition = spawnPoint;
        }
    }
}
