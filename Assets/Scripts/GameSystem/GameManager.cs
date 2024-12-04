using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeamB.GameSystem
{
    /// <summary>
    /// ゲームシステムに相当する。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] ExamStateDatas examStateDatas;

        private string ExamDataURL =
            "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=1160587194&single=true&output=csv";

        private void Start()
        {
            InitialExamData();
        }

        private async void InitialExamData()
        {
            List<string[]> rawData = await CsvLoader.GetSpreadsheetDataAsync(ExamDataURL);

            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }

            for (var i = 1; i < rawData.Count; i++)
            {
                var data = rawData[i];

                var classChoiceData = new ExamStateData
                {
                    ExamDataID = data[0],
                    CurrentState = data[1],
                    VictoryState = data[2],
                    DefeatState = data[3],
                };
                examStateDatas.Data.Add(classChoiceData);
            }
        }
    }


}