using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TeamB.Data
{
    /// <summary>
    /// プレイヤーのデータを管理するクラス
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// データをJsonで保存する機能
        /// </summary>
        /// <param name="characterData"></param>
        public async UniTask SaveDataAsync<T>(string path, T data)
        {
            using (StreamWriter wr = new StreamWriter(path, false))
            {
                string json = JsonUtility.ToJson(data); 
                await wr.WriteLineAsync(json);  
            }
        }

        /// <summary>
        /// プレイヤーデータJsonを読み取る
        /// </summary>
        /// <returns></returns>
        public async UniTask<T> ReadDataAsync<T>(string path, Action<T> onSuccess)
        {
            try
            {
                StreamReader rd = new StreamReader(path);               // ファイル読み込み指定
                string json = rd.ReadToEnd();                           // ファイル内容全て読み込む
                rd.Close();                                             // ファイル閉じる
                                                                
                return JsonUtility.FromJson<T>(json); 
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }
}