﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            
            inputFieldUserNameUI = inputFieldGameObject == null
                ? transform.parent.GetComponentInChildren<InputFieldUserNameUI>()
                : inputFieldGameObject.GetComponent<InputFieldUserNameUI>();
            
            rankList = new List<RankData>();
        }

        private void Start()
        {
            LoadRankData(Path.Combine(Application.dataPath, "RankTable.json"));
            
            var gameStateManager = FindObjectOfType<GameStateManager>();
            if (gameStateManager != null)
            {
                playerRankData = gameStateManager.gameData;
            }
            else
            {
                // test
                playerRankData = new RankData()
                {
                    userName = "",
                    rank = 0,
                    huntingCount = 0,
                    playMilliSecondTime = 0
                };
            }

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
            rankList = rankList.OrderBy(data => data.playMilliSecondTime).ThenByDescending(data => data.huntingCount).ToList();

            for (var i = 0; i < rankList.Count; i++)
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
            var rankTextFull = "";
            for (var i = rankViewStartIndex; i < rankViewStartIndex + maxRankView; i++)
            {
                var rankTextLine = "    ";
                
                var tRank = (i + 1).ToString();
                if (i == 0) tRank += "st.";
                else if (i == 1) tRank += "nd.";
                else if (i == 2) tRank += "rd.";
                else tRank += "th.";
                rankTextLine += tRank;
                
                if (tRank == "2nd.") rankTextLine += "           ";
                else if (i >= 9) rankTextLine += "          ";
                else rankTextLine += "            ";

                var tHuntingCount = $"{rankList[i].huntingCount:D2}";
                rankTextLine += tHuntingCount + "            ";
                
                var timeSpan = new TimeSpan(0,0,0,0,rankList[i].playMilliSecondTime);
                var tTime = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{timeSpan.Milliseconds:D3}";
                rankTextLine += tTime + "             ";

                var tName = rankList[i].userName;
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
