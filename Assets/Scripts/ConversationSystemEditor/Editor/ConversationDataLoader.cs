#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codice.Utils;
using Cysharp.Threading.Tasks;
using TeamB.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace TeamB.Editor
{
    /// <summary>
    /// 会話データローダー
    /// スプシからデータを取ってきて整形する
    /// </summary>
    public class ConversationDataLoader : MonoBehaviour
    {
        private const string URL =
            "https://docs.google.com/spreadsheets/d/e/2PACX-1vThKPuoZP1mWXWO35ZRrwerfLE_Qg6eL-BMoL4f5pSuyEacMgtqLYc_N2whIOlK9MRGhgwSdPAAb-oC/pub?output=csv";

        public enum LoadDataType
        {
            CharaData,
            ConversationData,
        }

        [MenuItem("ConversationEdit/Load Conversation Data")]
        private static void LoadConversationData()
        {
            GetSpreadsheetDataAsync(LoadDataType.ConversationData).Forget();
        }

        [MenuItem("ConversationEdit/Load Chara Data")]
        private static void LoadCharaData()
        {
            GetSpreadsheetDataAsync(LoadDataType.CharaData).Forget();
        }

        
        //todo: スプシからデータを取る処理とデータを整形する処理を分ける
        private static async UniTask GetSpreadsheetDataAsync(LoadDataType loadDataType)
        {
            using (var request = UnityWebRequest.Get(URL))
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
                        case LoadDataType.ConversationData:
                            var conversationDataBuilder = new ConversationDataMaker();
                            conversationDataBuilder.MakeData(parsedData);
                            break;
                        case LoadDataType.CharaData:
                            var charaDataBuilder = new CharaDataMaker();
                            charaDataBuilder.MakeData(parsedData);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

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

    /// <summary>
    /// 取得したデータを変換するメソッドの共通インターフェース
    /// </summary>
    public interface IConversationDataBuilder
    {
        void MakeData(List<string[]> data);
    }
    
    /// <summary>
    /// 会話データの生成処理クラス
    /// </summary>
    public class ConversationDataMaker : IConversationDataBuilder
    {
        private const int CharaCount = 2;
        private const int CharaDataCount = 3;
        private const string SavePath = "Assets/DataAsset/MasterData/ConversationData";
        public void MakeData(List<string[]> rawData)
        {
            if (rawData == null)
            {
                Debug.LogError("Error: データがありません");
                return;
            }
            
            ConversationData conversationData = null;
            Debug.Log(rawData.Count());
            for (int i = 1; i < rawData.Count(); i++)  //1行目はヘッダーなので読み飛ばす
            {
                if (!string.IsNullOrEmpty(rawData[i][0]))　//1列目がある場合はそれまでの会話データのスクリプタブルオブジェクトを保存して新たな会話データを作成
                {   
                    if (conversationData != null)
                    {
                        GenerateConversationDataAsset(conversationData);
                    }
                    conversationData = ScriptableObject.CreateInstance<ConversationData>(); 
                    conversationData.ConversationName = rawData[i][0];
                }
                
                if (conversationData == null)
                {
                    Debug.LogError("Error: 会話データがありません");
                    return;
                }
                conversationData.ConversationEntries.Add(new ConversationEntry());
                conversationData.ConversationEntries.Last().Characters = new List<CharacterData>();

                for (var j = 0; j < CharaCount; j++)
                {
                    var chara = new CharacterData
                    {
                        CharacterName = rawData[i][1 + j * CharaDataCount],
                        Position = Enum.TryParse(rawData[i][2 + j * CharaDataCount], out Position position) ? position : Position.Left,
                        Animation = rawData[i][3 + j * CharaDataCount]
                    };
                    conversationData.ConversationEntries.Last().Characters.Add(chara);
                }
                
                conversationData.ConversationEntries.Last().Speaker = rawData[i][7];
                conversationData.ConversationEntries.Last().Dialogue = rawData[i][8];
                
                if (i == rawData.Count() - 1)
                {
                    GenerateConversationDataAsset(conversationData);
                }
            }
            Debug.Log("会話データの作成が完了しました");
        }

        private static void GenerateConversationDataAsset(ConversationData conversationData)
        {
            if (File.Exists(SavePath + "/" + "ConversationData_" + conversationData.ConversationName + ".asset"))
            {
                AssetDatabase.DeleteAsset(SavePath + "/" + "ConversationData_" + conversationData.ConversationName + ".asset");
            }
            AssetDatabase.CreateAsset(conversationData, SavePath + "/" + "ConversationData_" + conversationData.ConversationName + ".asset");
            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// 話者のデータの生成処理クラス
    /// </summary>
    public class CharaDataMaker : IConversationDataBuilder
    {
        private const string LoadDataKey = "キャラ一覧";
        private const string ExportPath = "Assets/Scripts/ConversationSystem";
        private const string FileName = "CharaAnimationEnum";
        private const string nameSpace = "TeamB.Data";

        public void MakeData(List<string[]> rawData)
        {
            if (rawData == null)
            {
                Debug.LogError("Error: データがありません");
                return;
            }

            if (rawData[0][0] != LoadDataKey)
            {
                Debug.LogError("Error: データがキャラデータのものではありません");
                return;
            }

            var enumData = new Dictionary<string, (List<string> itemNameList, string summary)>();
            for (int i = 1; i < rawData.Count; i++) //1行目はヘッダーなので読み飛ばす
            {
                var data = rawData[i];
                var enumName = data[0];
                var enumValues = data.Skip(2).ToList(); //配列名と日本語名は飛ばす
                enumData.Add(enumName, (enumValues, $"{enumName}のアニメーションEnum"));
            }

            EnumMaker.Create(FileName, enumData, ExportPath, nameSpace);
        }
    }
}
#endif