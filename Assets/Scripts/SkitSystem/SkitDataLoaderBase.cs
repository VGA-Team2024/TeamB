using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TeamB.SkitSystem
{
    /// <summary>
    /// 会話シーンで用いられるデータを読み込むクラスの共通処理
    /// </summary>
    public abstract class SkitDataLoaderBase
    {
        protected HashSet<ClassSelectData> _classSelectData = new();
        protected HashSet<SkitData> _skitData = new();
        protected HashSet<SkitChoiceData> _skitChoiceData = new();
        protected HashSet<TutorialData> _tutorialData = new();
        public string PlayerName { get; set; }
        public abstract UniTask InitTalkData();

        private const string JapaneseIntuition = "直観力";
        private const string JapaneseReadingComprehension = "読解力、学力";
        private const string JapaneseConcentration = "集中力";
        protected static RewardType GetRewardType(string reward)
        {
            return reward switch
            {
                JapaneseIntuition => RewardType.Intuition,
                JapaneseReadingComprehension => RewardType.ReadingComprehension,
                JapaneseConcentration => RewardType.Concentration,
                _ => RewardType.None
            };
        }
        
        public bool TryGetClassSelectDataById(string id, out ClassSelectData classSelectData)
        {
            classSelectData = _classSelectData.FirstOrDefault(x => x.Id == id);
            return classSelectData != null;
        }

        public bool TryGetSkitSceneDataByFlag(SkitFlagData flag, out ISkitSceneData classSelectData)
        {
            classSelectData = _classSelectData.FirstOrDefault(x => x.Flag == flag.CurrentFlag);
            // foreach (var data in _skitData)
            // {
            //     Debug.Log($"今見てるデータ{data.SkitEntryData[0].ToString()}");
            // }
            if (classSelectData == null) classSelectData = _skitData.FirstOrDefault(x => x.Flag.Trim() == flag.CurrentFlag.Trim());
            return classSelectData != null;
        }

        public bool TryGetSkitDataById(string id, out SkitData skitData)
        {
            skitData = _skitData.FirstOrDefault(x => x.Id == id);
            return skitData != null;
        }

        public bool TryGetSkitChoiceDataByID(string id, out SkitChoiceData choiceData)
        {
            choiceData = _skitChoiceData.FirstOrDefault(x => x.Id == id);
            return choiceData != null;
        }
        
        public bool TryGetTutorialDataById(out TutorialData tutorialData, string id = "")
        {
            tutorialData = _tutorialData.FirstOrDefault(x => x.Id == id);
            return tutorialData != null;
        }

        public SkitChoiceData[] GetAllSkitChoiceData()
        {
            return _skitChoiceData.ToArray();
        }

        public SkitData[] GetAllSkitData()
        {
            return _skitData.ToArray();
        }

        public ClassSelectData[] GetAllClassSelectData()
        {
            return _classSelectData.ToArray();
        }

        public bool TryGetAllClassSelectData(out ClassSelectData[] classSelectData)
        {
            classSelectData = _classSelectData.ToArray();
            return classSelectData == null;
        }
    }
    
    /// <summary>
    /// ドライブのスプシからデータを読み込むクラス
    /// </summary>
    public class RemoteSkitDataLoaderBase : SkitDataLoaderBase
    { 
        private const string ClassSelectDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=514427729&single=true&output=csv";
        private const int ClassSelectDataLength = 4;
        private const string SkitDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=159610865&single=true&output=csv";
        private const int SkitDataLength = 3;
        private const string SkitChoiceDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=1641370933&single=true&output=csv";
        private const int DefaultLimitTime = 10;
        private const string TutorialDataKey = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?gid=878141&single=true&output=csv";
        
        private const int SkitChoiceLength = 2;
        private string _playerName;
        
        
        public override UniTask InitTalkData()
        {
            return UniTask.WhenAll(LoadClassSelectData(), LoadSkitData(), LoadSkitChoiceData(), LoadTutorialData());
        }

        private async UniTask LoadClassSelectData()
        {
            var rawData = await CsvLoader.GetSpreadsheetDataAsync(ClassSelectDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }

            for (var i = 1; i < rawData.Count; i++)
            {
                var data = rawData[i];
                var dataLength = data.Length;
                var classChoices = new List<ClassChoiceData>();
                for (var j = 5; j < dataLength; j += ClassSelectDataLength)
                {
                    var classChoiceData = new ClassChoiceData
                    {
                        ChoiceName = data[j],
                        JapaneseChoiceName = data[j + 1],
                        TalkDataId = data[j + 2],
                        TalkReward = SkitDataLoaderBase.GetRewardType(data[j + 3])
                    };
                    classChoices.Add(classChoiceData);
                }
                var classSelectData = new ClassSelectData(data[0], data[1],data[2], data[3], data[4], classChoices.ToArray());
                _classSelectData.Add(classSelectData);
            }
        }

        private async UniTask LoadSkitData()
        {
            var rawData = await CsvLoader.GetSpreadsheetDataAsync(SkitDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }
            var currentSkitDataId = rawData[1][0];
            var currentSkitFlag = rawData[1][1];
            var skitEntryDataList = new List<SkitEntryData>();
            for (var i = 1; i < rawData.Count; i++)
            {
                if (i != 1 && rawData[i][0] != "")
                {   // 会話データのIDが変わったら保存して新しい会話データを作成
                    var skitData = new SkitData(currentSkitDataId, currentSkitFlag, skitEntryDataList.ToArray());
                    currentSkitDataId = rawData[i][0];
                    currentSkitFlag = rawData[i][1];
                    _skitData.Add(skitData);
                    skitEntryDataList = new List<SkitEntryData>();
                }

                var classTalkCharaData = new List<SkitTalkCharaData>();
                
                for (var j = 6; j < rawData[i].Length; j += SkitDataLength)
                {   // 会話キャラクターデータを作成
                    var standingPosition = !string.IsNullOrEmpty(rawData[i][j + 1]) ? Enum.Parse<StandingPosition>(rawData[i][j + 1]) : StandingPosition.None;
                    var eachClassTalkCharaData = new SkitTalkCharaData(rawData[i][j], standingPosition, rawData[i][j + 2]);
                    classTalkCharaData.Add(eachClassTalkCharaData);
                }
                var skitEntryData = new SkitEntryData(classTalkCharaData.ToArray(), rawData[i][2], rawData[i][3], rawData[i][4], rawData[i][5]);
                skitEntryDataList.Add(skitEntryData);

                if (i != rawData.Count - 1) continue;   // 最後のデータの場合は保存する
                var lastSkitData = new SkitData(currentSkitDataId, currentSkitFlag, skitEntryDataList.ToArray());
                _skitData.Add(lastSkitData);
            }
        }
        
        private async UniTask LoadTutorialData()
        {
            var rawData = await CsvLoader.GetSpreadsheetDataAsync(TutorialDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }

            var id = "";
            var flag = rawData[1][0];
            var background = rawData[1][1];
            var dialogueList = new List<string>();
            for (var i = 1; i < rawData.Count; i++)
            {
                var data = rawData[i][2];
                dialogueList.Add(data);
            }
            _tutorialData.Add(new TutorialData(id, flag, dialogueList.ToArray(), background));
        }

        private async UniTask LoadSkitChoiceData()
        {
            var rawData = await CsvLoader.GetSpreadsheetDataAsync(SkitChoiceDataKey);
            if (rawData == null)
            {
                Debug.LogError("Failed to load data");
                return;
            }
            
            for (var i = 1; i < rawData.Count; i++)
            {
                var data = rawData[i];
                var id = data[0];
                var limitTime = string.IsNullOrEmpty(data[1].Trim()) ? DefaultLimitTime : float.Parse(data[1]);
                var problemDialogue = data[2];
                var answer = data[3].Trim();
                var choiceEntries = new List<ChoiceEntry>();
                for (var j = 4; j < data.Length; j += SkitChoiceLength)
                {
                    var choiceEntry = new ChoiceEntry(data[j].Trim(), data[j].Trim(), data[j + 1].Trim());
                    choiceEntries.Add(choiceEntry);
                }

                var choiceData = new SkitChoiceData(id, limitTime, answer, choiceEntries.ToArray(), problemDialogue);
                _skitChoiceData.Add(choiceData);
            }
        }
    }
    
    /// <summary>
    /// ローカルでデータを読み込むクラス
    /// </summary>
    public class LocalSkitDataLoaderBase : SkitDataLoaderBase
    {
        public override UniTask InitTalkData()
        {
            return UniTask.WhenAll(LoadClassSelectData(), LoadSkitData(), LoadSkitChoiceData(), LoadTutorialData());
        }
        
        // 以下にデータロード用のメソッド群を定義
        private async UniTask LoadClassSelectData()
        {
            // ここでローカルのclassSelectData.jsonなどを読み込む処理を記述
            // とりあえず例なので処理は空にしておく
        }

        private async UniTask LoadSkitChoiceData()
        {
            // ここでskitChoiceData.jsonなどから読み込む処理を記述
        }

        private async UniTask LoadTutorialData()
        {
            // ここでtutorialData.jsonなどから読み込む処理を記述
        }
        
        private async UniTask LoadSkitData()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // StreamingAssets内の"SkitData"というフォルダにあるJSONをすべて読み込みます。
            var folderPath = Path.Combine(Application.streamingAssetsPath, "SkitData");
            // PC/Editorなどの場合はディレクトリからすべてのjsonファイルを取得可能
            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning($"SkitData folder not found at: {folderPath}");
                return;
            }

            var jsonFiles = Directory.GetFiles(folderPath, "*.json");
            foreach (var file in jsonFiles)
            {
                var json = await File.ReadAllTextAsync(file);
                var loadedSkitData = JsonConvert.DeserializeObject<SkitData>(json);

                if (loadedSkitData != null)
                {
                    _skitData.Add(loadedSkitData);
                }
                else
                {
                    Debug.LogError($"Failed to load SkitData from: {file}");
                }
            }
            stopwatch.Stop();
            Debug.Log($"Total loading time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
