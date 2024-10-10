#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using TeamB.Data;

namespace TeamB.Editor
{
    /// <summary>
    /// 会話データローダー
    /// </summary>
    public class ConversationDataLoader : MonoBehaviour
    {
        private const string URL =
            "https://docs.google.com/spreadsheets/d/11yRwE0U9cVTU1eUJcNEQZoRKq-WYLgbUtr8ruEVuCNQ/edit?usp=sharing";

        [MenuItem("TeamB/Load Conversation Data")]
        private static async void LoadData()
        {
            using (var req = UnityWebRequest.Get(URL))
            {
                var operation = req.SendWebRequest();
                while (!operation.isDone)
                {
                    await System.Threading.Tasks.Task.Yield();
                }

                if (IsWebRequestSuccessful(req)) //成功した場合
                {
                    var conversationData = ParseData(req.downloadHandler.text); //受け取ったデータを整形する関数に情報を渡す
                    SaveDataAsJson(conversationData); //データをJSONとして保存する
                }
                else //失敗した場合
                {
                    Debug.Log("error");
                }
            }
        }
        
        private static bool IsWebRequestSuccessful(UnityWebRequest req)
        {
            /*プロトコルエラーとコネクトエラーではない場合はtrueを返す*/
            return req.result != UnityWebRequest.Result.ProtocolError &&
                   req.result != UnityWebRequest.Result.ConnectionError;
        }

        private static ConversationData ParseData(string rawData)
        {
            // ここで rawData を ConversationData に変換する処理を実装します
            // 仮の実装例として、空の ConversationData を返します
            var conversationData = ScriptableObject.CreateInstance<ConversationData>();
            conversationData.ConversationName = "Sample Conversation";
            conversationData.ConversationEntries = new List<ConversationEntry>
            {
                new ConversationEntry
                {
                    Speaker = Speaker.Speaker1,
                    Dialogue = "Hello, World!",
                    Character1 = new CharacterData
                    {
                        CharacterName = Speaker.Speaker1, Position = Position.Left, Animation = Data.AnimationState.Idle
                    },
                    Character2 = new CharacterData
                    {
                        CharacterName = Speaker.Speaker2, Position = Position.Right,
                        Animation = Data.AnimationState.Talking
                    }
                }
            };
            return conversationData;
        }

        private static void SaveDataAsJson(ConversationData conversationData)
        {
            string json = JsonUtility.ToJson(conversationData, true);
            string path = Path.Combine(Application.dataPath, "ConversationData.json");
            File.WriteAllText(path, json);
            Debug.Log($"Conversation data saved to {path}");
        }
    }
}
#endif