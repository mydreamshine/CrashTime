using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Scenes.SharedDataEachScenes;
using TMPro;
using UnityEngine;

namespace Scenes.GameOverScene.Scripts.UI
{
    public class RankListUI : MonoBehaviour
    {
        [SerializeField] private GameObject inputFieldGameObject;

        private TMP_Text textField;
        private List<RankData> rankList;
        private InputFieldUserNameUI inputFieldUserNameUI;

        public RankData playerRankData;

        private void Awake()
        {
            textField = GetComponent<TMP_Text>();
            
            if (inputFieldGameObject == null)
                inputFieldUserNameUI = transform.parent.GetComponentInChildren<InputFieldUserNameUI>();
            else
                inputFieldUserNameUI = inputFieldGameObject.GetComponent<InputFieldUserNameUI>();
            
            rankList = new List<RankData>();
            
            // test
            playerRankData = new RankData()
            {
                userName = "",
                rank = 0,
                huntingCount = 12,
                playMilliSecondTime = 22001
            };
        }

        private void Start()
        {
            LoadRankData(Path.Combine(Application.dataPath, "RankTable.json"));

            playerRankData.rank = CalculatePlayerRank();
            playerRankData.userName = "";
            rankList[playerRankData.rank - 1] = playerRankData;

            inputFieldUserNameUI.CalculateInputFieldSpawnIndex(rankList.Count, playerRankData.rank);
            
            RankListToTextByPlayerRank(playerRankData.rank);
        }

        private int CalculatePlayerRank()
        {
            playerRankData.userName = "unknown"; // 식별용
            rankList.Add(playerRankData);
            rankList.Sort(delegate(RankData lhsData, RankData rhsData)
            {
                if (lhsData.playMilliSecondTime > rhsData.playMilliSecondTime) return 1;
                if (lhsData.playMilliSecondTime < rhsData.playMilliSecondTime) return -1;
                return 0;
            });

            for (int i = 0; i < rankList.Count; i++)
            {
                var data = rankList[i];
                data.rank = i + 1;
                rankList[i] = data;
            }

            var playerRankIndex = rankList.FindIndex(data => data.userName == "unknown");

            return (playerRankIndex + 1);
        }

        private void RankListToText(int rankViewStartIndex, int maxRankView)
        {
            string rankTextFull = "";
            for (int i = rankViewStartIndex; i < rankViewStartIndex + maxRankView; i++)
            {
                string rankTextLine = "    ";
                
                string tRank = (i + 1).ToString();
                if (i == 0) tRank += "st.";
                else if (i == 1) tRank += "nd.";
                else if (i == 2) tRank += "rd.";
                else tRank += "th.";
                rankTextLine += tRank;
                
                if (tRank == "2nd.") rankTextLine += "           ";
                else if (i >= 9) rankTextLine += "          ";
                else rankTextLine += "            ";

                string tHuntingCount = $"{rankList[i].huntingCount:D2}";
                rankTextLine += tHuntingCount + "            ";
                
                var timeSpan = new TimeSpan(0,0,0,0,rankList[i].playMilliSecondTime);
                string tTime = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{timeSpan.Milliseconds:D3}";
                rankTextLine += tTime + "             ";

                string tName = rankList[i].userName;
                rankTextLine += tName;

                if (i + 1 < rankViewStartIndex + maxRankView) rankTextLine += Environment.NewLine;
                rankTextFull += rankTextLine;
            }

            textField.text = rankTextFull;
        }

        private void RankListToTextByPlayerRank(int playerRank)
        {
            var inputFieldSpawnCount = inputFieldUserNameUI.GetInputFieldSpawnCount();
            var inputFieldSpawnIndex = inputFieldUserNameUI.GetInputFieldSpawnIndex();
            
            var rankViewStartIndex = (playerRank - 1) - inputFieldSpawnIndex;
            var maxRankView = Math.Min(rankList.Count, inputFieldSpawnCount);
            RankListToText(rankViewStartIndex, maxRankView);
        }

        private void LoadRankData(string path)
        {
            if (!File.Exists(path)) return;
            
            var jsonText = File.ReadAllText(path);
            rankList = JsonUtility.FromJson<Serialization<RankData>>(jsonText).ToList();
        }

        public void SaveRankData()
        {
            playerRankData.userName = inputFieldUserNameUI.GetCompletedPlayerName();
            rankList[playerRankData.rank - 1] = playerRankData;

            var path = Path.Combine(Application.dataPath, "RankTable.json");
            var jsonText = JsonUtility.ToJson(new Serialization<RankData>(rankList));
            File.WriteAllText(path, jsonText, Encoding.UTF8);

            RankListToTextByPlayerRank(playerRankData.rank);
        }
    }
}
