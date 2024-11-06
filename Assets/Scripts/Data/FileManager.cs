using System;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
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
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                await fs.WriteAsync(bytes, 0, bytes.Length);
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
                string datastr = await File.ReadAllTextAsync(path);
                T characterData = JsonConvert.DeserializeObject<T>(datastr);
                onSuccess?.Invoke(characterData);
                return characterData;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }
}