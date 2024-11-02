using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using TeamB.Data;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace TeamB.Editor
{
    public static class GameMenu
    {
        private const string filePath = @"Assets\Scripts\Data\CharacterType.cs";

        /// <summary>
        /// マスターデータをもとにEnum作る
        /// </summary>
        /// <param name="masterData"></param>
        [MenuItem("Assets/Create/CharacterType")]
        public static async void CreateCharacterType()
        {
            try
            {
                string datastr = await File.ReadAllTextAsync(GameConsts.CharacterFile);
                DataManagement.SpreadSheet.CharacterMaster characterData = JsonConvert.DeserializeObject<DataManagement.SpreadSheet.CharacterMaster>(datastr);
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    string script = "public enum CharacterType{";
                    for (int i = 0; i < characterData.Data.Length; i++)
                    {
                        script += $"{characterData.Data[i].ResourceName}={characterData.Data[i].Id},";
                    }

                    script += "}";
                    byte[] bytes = Encoding.UTF8.GetBytes(script);
                    await fs.WriteAsync(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
            
        }
    }
}