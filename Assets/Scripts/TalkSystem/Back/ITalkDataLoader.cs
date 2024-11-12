using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace TeamB.TalkSystem
{
    public interface ITalkDataLoader
    {
        public UniTask LoadClassSelectData();
        public UniTask LoadTalkData();
        public UniTask LoadChoiceData();
        public bool TryGetClassSelectData(string id, out ClassSelectData classSelectData);
        public bool TryGetTalkData(string id, out ClassTalkData talkData);
        public bool TryGetChoiceData(string id, out ChoiceData choiceData);
                
        protected static RewardType GetRewardType(string reward)
        {
            return reward switch
            {
                "直観力" => RewardType.Intuition,
                "読解力、学力" => RewardType.ReadingComprehension,
                "集中力" => RewardType.Concentration,
                _ => RewardType.None
            };
        }

    }

    public class LocalTalkDataLoader : ITalkDataLoader
    {
        private HashSet<ClassSelectData> _classSelectDataCache = new();
        private HashSet<ClassTalkData> _talkDataCache = new();
        private HashSet<ChoiceData> _choiceDataCache = new();

        public UniTask LoadClassSelectData()
        {
            var path = Path.Combine(Application.streamingAssetsPath, nameof(ClassSelectData));
            var files = Directory.GetFiles(path, "*.json");

            _classSelectDataCache = new HashSet<ClassSelectData>();

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                //todo: 現状Enumのデータが読み込めないので、Enumのデータを読み込む処理を追加する
                var data = JsonUtility.FromJson<ClassSelectData>(json);
                _classSelectDataCache.Add(data);
            }
            return UniTask.CompletedTask;
        }

        public UniTask LoadTalkData()
        {
            var path = Path.Combine(Application.streamingAssetsPath, nameof(ClassTalkData));
            var files = Directory.GetFiles(path, "*.json");

            _talkDataCache = new HashSet<ClassTalkData>();

            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var data = JsonUtility.FromJson<ClassTalkData>(json);
                _talkDataCache.Add(data);
            }
            return UniTask.CompletedTask;
        }

        public UniTask LoadChoiceData()
        {
            var path = Path.Combine(Application.streamingAssetsPath, nameof(ChoiceData));
            var files = Directory.GetFiles(path, "*.json");
            _choiceDataCache = new HashSet<ChoiceData>();
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var data = JsonUtility.FromJson<ChoiceData>(json);
                _choiceDataCache.Add(data);
            }
            return UniTask.CompletedTask;
        }

        public bool TryGetClassSelectData(string id, out ClassSelectData classSelectData)
        {
            classSelectData = _classSelectDataCache.FirstOrDefault(data => data.ClassSelectId == id);
            return classSelectData != null;
        }

        public bool TryGetTalkData(string id, out ClassTalkData talkData)
        {
            talkData = _talkDataCache.FirstOrDefault(data => data.TalkDataId == id);
            return talkData != null;
        }

        public bool TryGetChoiceData(string id, out ChoiceData choiceData)
        {
            choiceData = _choiceDataCache.FirstOrDefault(data => data.ChoiceId == id);
            return choiceData != null;
        }
    }

    public class RemoteTalkDataLoader : ITalkDataLoader
    {
        private const string ClassSelectDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=514427729&single=true&output=csv";
        private const int ClassSelectDataLength = 4;
        private const string ClassRoomTalkDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=159610865&single=true&output=csv";
        private const int ClassRoomTalkDataLength = 3;
        private const string ClassRoomChoiceDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=1641370933&single=true&output=csv";
        private const int ClassRoomChoiceLength = 3;
        private HashSet<ClassSelectData> _classSelectData = new();
        private HashSet<ClassTalkData> _talkData = new();
        private HashSet<ChoiceData> _choiceData = new();

        public async UniTask LoadClassSelectData()
        {
            var rawData = await CSVLoader.GetSpreadsheetDataAsync(ClassSelectDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }

            for (var i = 1; i < rawData.Count; i++)
            {
                var data = rawData[i];
                var classSelectData = new ClassSelectData
                {
                    ClassSelectId = data[0]
                };
                var dataLength = (data.Length);
                var classChoices = new List<ClassChoiceData>();
                for (var j = 1; j < dataLength; j += ClassSelectDataLength)
                {
                    var classChoiceData = new ClassChoiceData
                    {
                        ChoiceName = data[j],
                        JapaneseChoiceName = data[j + 1],
                        TalkDataId = data[j + 2],
                        TalkReward = ITalkDataLoader.GetRewardType(data[j + 3])
                    };

                    Debug.Log(data[j + 3]);
                    Debug.Log(ITalkDataLoader.GetRewardType(data[j + 3]));
                    classChoices.Add(classChoiceData);
                }

                classSelectData.ClassChoices = classChoices.ToArray();
                _classSelectData.Add(classSelectData);
            }
        }

        public async UniTask LoadTalkData()
        {
            var rawData = await CSVLoader.GetSpreadsheetDataAsync(ClassRoomTalkDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }
            var talkData =  new ClassTalkData { TalkDataId = rawData[1][0] };
            var talkEntryData = new List<ClassTalkEntryData>();
            for (var i = 1; i < rawData.Count; i++)
            {
                if (i != 1 && rawData[i][0] != "")
                {   // 会話データのIDが変わったら保存して新しい会話データを作成
                    talkData.TalkData = talkEntryData.ToArray();
                    _talkData.Add(talkData);
                    talkData = new ClassTalkData { TalkDataId = rawData[i][0] };
                    talkEntryData = new List<ClassTalkEntryData>();
                }

                var classTalkEntryData = new ClassTalkEntryData();
                var classTalkCharaData = new List<ClassTalkCharaData>();
                for (var j = 1; j < 6; j += ClassRoomTalkDataLength)
                {
                    var eachClassTalkCharaData = new ClassTalkCharaData
                    {
                        CharaName = rawData[i][j],
                        CharaStateName = rawData[i][j + 2]
                    };
                    if (!string.IsNullOrEmpty(rawData[i][j + 1]))
                        eachClassTalkCharaData.StandingPosition = Enum.Parse<StandingPosition>(rawData[i][j + 1]);
                    classTalkCharaData.Add(eachClassTalkCharaData);
                }
                classTalkEntryData.TalkCharaData = classTalkCharaData.ToArray();
                classTalkEntryData.TalkSpeaker = rawData[i][7];
                classTalkEntryData.TalkBackground = rawData[i][8];
                classTalkEntryData.JapaneseTalkDialogue = rawData[i][9];
                classTalkEntryData.EnglishTalkDialogue = rawData[i][10];
                talkEntryData.Add(classTalkEntryData);

                if (i != rawData.Count - 1) continue;   // 最後のデータの場合は保存する
                talkData.TalkData = talkEntryData.ToArray();
                _talkData.Add(talkData);
            }
        }

        public async UniTask LoadChoiceData()
        {
            var rawData = await CSVLoader.GetSpreadsheetDataAsync(ClassRoomChoiceDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }
            
            for (var i = 1; i < rawData.Count; i++)
            {
                var data = rawData[i];
                var choiceData = new ChoiceData
                {
                    ChoiceId = data[0],
                    ChoiceTime = float.Parse(data[1]),
                    AnswerIndex = data.Length - 1,
                    ChoiceEntries = new ChoiceEntry[ClassRoomChoiceLength]
                };
                for (var j = 0; j < ClassRoomChoiceLength ; j++)
                {
                    var choiceEntry = new ChoiceEntry
                    {
                        ChoiceEntryId = data[j * 2 + 2],
                        EnglishChoiceEntryName = data[j * 2 + 2],
                        JapaneseChoiceEntryName = data[j * 2 + 3]
                    };
                    choiceData.ChoiceEntries[j] = choiceEntry;
                }
                _choiceData.Add(choiceData);
            }
        }

        public bool TryGetClassSelectData(string id, out ClassSelectData classSelectData)
        {
            classSelectData = _classSelectData.FirstOrDefault(x => x.ClassSelectId == id);
            return classSelectData != null;
        }

        public bool TryGetTalkData(string id, out ClassTalkData talkData)
        {
            talkData = _talkData.FirstOrDefault(x => x.TalkDataId == id);
            return talkData != null;
        }

        public bool TryGetChoiceData(string id, out ChoiceData choiceData)
        {
            choiceData = _choiceData.FirstOrDefault(x => x.ChoiceId == id);
            return choiceData != null;
        }

       
    }
}