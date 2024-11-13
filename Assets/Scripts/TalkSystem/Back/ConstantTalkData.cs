using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TeamB.TalkSystem
{
    [CreateAssetMenu(fileName = "ConstantTalkData", menuName = "TalkSystem/ConstantTalkData")]
    public class ConstantTalkData : ScriptableObject
    {
        [Header("キャラクターデータ")] 
        public List<CharacterData> CharacterDataList = new List<CharacterData>();
        [Header("背景データ")] 
        public List<BackgroundData> BackgroundDataList = new List<BackgroundData>();
    }
    
    [System.Serializable]
    public class CharacterData
    {
        public string CharacterID;
        public string JapaneseName;
        public List<CharaStateData> TalkDataList;
    }

    [System.Serializable]
    public class CharaStateData
    {
        public string StateName;
        public Sprite CharaSprite;
    }
    
    [System.Serializable]
    public class BackgroundData
    {
        public string BackgroundID;
        public Sprite BackgroundSprite;
    }
    #if UNITY_EDITOR
    public static class ConstantTalkDataEditor
    {
        private const string FolderPath = "Assets/DataAsset/MasterData" + "/" + nameof(ConstantTalkData);
        private const string FileName = "ConstantTalkData.asset";
        private const string CharacterDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=0&single=true&output=csv";
        private const string BackgroundDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=1399808365&single=true&output=csv";
        
        [UnityEditor.MenuItem("Tools/TalkSystem/Create ConstantTalkData")]
        public static async void LoadConstantTalkData()
        {
            var savePath = FolderPath + "/" + FileName;
            
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Deleted existing ConstantTalkData");
            }
            
            // CharacterData
            var targetData = ScriptableObject.CreateInstance<ConstantTalkData>();
            var rawData = await CSVLoader.GetSpreadsheetDataAsync(CharacterDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load CharacterData");
                return;
            }
            
            var characterDataList = new List<CharacterData>();
            for (var i = 1; i < rawData.Count; i++)
            {
                var characterData = new CharacterData
                {
                    CharacterID = rawData[i][0],
                    JapaneseName = rawData[i][1],
                    TalkDataList = new List<CharaStateData>()
                };
                for (var j = 2; j < rawData[i].Length ; j++)
                {
                    if (string.IsNullOrEmpty(rawData[i][j])) continue;
                    characterData.TalkDataList.Add(new CharaStateData
                    {
                        StateName = rawData[i][j],
                    });
                }

                characterDataList.Add(characterData);
            }
            targetData.CharacterDataList = characterDataList;
            
            // BackgroundData
            rawData = await CSVLoader.GetSpreadsheetDataAsync(BackgroundDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load BackgroundData");
                return;
            }
            
            var backgroundDataList = new List<BackgroundData>();
            for (var i = 1; i < rawData.Count; i++)
            {
                backgroundDataList.Add(new BackgroundData
                {
                    BackgroundID = rawData[i][0],
                });
            }
            targetData.BackgroundDataList = backgroundDataList;
            
            UnityEditor.AssetDatabase.CreateAsset(targetData, savePath);
            UnityEditor.AssetDatabase.Refresh();
        }
    }
    #endif
}
