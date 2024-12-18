using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace TeamB.SkitSystem
{
    public static class CsvLoader
    {
        public static async UniTask<List<string[]>> GetRemoteSpreadsheetDataAsync(string url)
        {
            using var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                return null;
            }

            var parsedData = ParseData(request.downloadHandler.text);
            return parsedData;
        }

        public static async UniTask<List<string[]>> GetLocalSpreadsheetDataAsync(string addressablePath)
        {
            var csvFile = await Addressables.LoadAssetAsync<TextAsset>(addressablePath).Task; 
            if (csvFile == null)
            {
                Debug.LogError("Error: CSV file not found");
                return null;
            }
            var parsedData = ParseData(csvFile.text);
            return parsedData;
        }

        /// <summary>
        /// スプシのデータを配列に成形する
        /// </summary>
        /// <param name="csvData"></param>
        /// <returns></returns>
        private static List<string[]> ParseData(string csvData)
        {
            var rows =
                csvData.Split(new[] { "\n" },
                    System.StringSplitOptions.RemoveEmptyEntries); //スプレッドシートを1行ずつ配列に格納

            return rows.Select(row => row.Split(',').Select(cell => cell.Trim()).ToArray()).ToList();
        }
    }
}