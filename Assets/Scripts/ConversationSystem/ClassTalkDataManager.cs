using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TeamB.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace TeamB.ConversationSystem
{
    public enum LoadType
    {
        Local,
        Server
    }
    
    public class ClassTalkDataManager : MonoBehaviour
    {
        [SerializeField, InspectorVariantName("データ取得タイプ")] private LoadType _loadType = LoadType.Server;
        [SerializeField, InspectorVariantName("会話マスタ")] private LoadedClassData _loadedClassData = null;
        private const string URLHeader = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ780qd4FuPPj59VDNF1fNumrbhI1sxtwOJXan9yVcnNtpZOMsPM_qm9yrpytbpWpPzVeO1fnxoGMzs/pub?ogrid=";
        private const string URLFooter = "&output=csv";
        private static string _seatID = "";
        private const string ClassRoomChoiceDataKey = "1641370933";
        private const string ClassSelectDataKey = "514427729";
        private const string ClassRoomConversationDataKey = "159610865";

        public async void LoadClassData()
        {
            if (_loadedClassData == null)
            {
                Debug.LogError("LoadedClassDataが設定されていません");
                return;
            }
            
            if (_loadType == LoadType.Local)
            {
                //todo: ローカルデータをロードする処理を追加
            }
            else
            {
                //会話データを取得
                
                //授業選択データを取得
                
                //授業内の選択肢データを取得
            }
        }
        
        private enum LoadDataType
        {
            ClassRoomChoice,
            ClassRoomData,
            ClassSelectData,
        }
        
        private static async UniTask GetSpreadsheetDataAsync(LoadDataType loadDataType)
        {
            using (var request = UnityWebRequest.Get(URLHeader + _seatID + URLFooter))
            {
                await request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + request.error);
                }
                else
                {
                    var parsedData = ParseData(request.downloadHandler.text);
                    switch (loadDataType)
                    {
                        case LoadDataType.ClassRoomData:
                            // var conversationDataBuilder = new ConversationDataMaker();
                            // conversationDataBuilder.MakeData(parsedData);
                            break;
                        case LoadDataType.ClassSelectData:
                            // var classSelectDataMaker = new ClassSelectDataMaker();
                            // classSelectDataMaker.MakeData(parsedData);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        
        /// <summary>
        /// スプシのデータを配列に成形する
        /// </summary>
        /// <param name="csvData"></param>
        /// <returns></returns>
        private static List<string[]> ParseData(string csvData)
        {
            var parseData = new List<string[]>();
            var rows =
                csvData.Split(new[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries); //スプレッドシートを1行ずつ配列に格納
            foreach (var row in rows)
            {
                parseData.Add(row.Split(','));
            }

            return parseData;
        }
    }

    [CreateAssetMenu(fileName = "ClassTalkData", menuName = "ClassTalkData", order = 0)]
    public class LoadedClassData : ScriptableObject
    {
        public Dictionary<string, ClassTalkData> LoadedClassTalkData = new ();
        [SerializeField, InspectorVariantName("会話キャラデータ")] public List<ConversationCharaData> ConversationCharaData = new ();
        [SerializeField, InspectorVariantName("背景")] public List<ConversationBackgroundData> ConversationBackgroundData = new ();
    }
}
