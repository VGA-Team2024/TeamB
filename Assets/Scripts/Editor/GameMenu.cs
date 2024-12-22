#if UNITY_EDITOR

using System;
using System.IO;
using System.Text;
using DataManagement;
using TeamB.Data;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace TeamB.Editor
{
    public static class GameMenu
    {
        private const string characterFilePath = @"Assets\Scripts\Data\CharacterType.cs";
        private const string enemyFilePath = @"Assets\Scripts\Data\EnemyType.cs";

        /// <summary>
        /// マスターデータをもとにキャラクターEnum作る
        /// </summary>
        /// <param name="masterData"></param>
        [MenuItem("Assets/Create/CharacterType")]
        public static async void CreateCharacterType()
        {
            try
            {
                string datastr = await File.ReadAllTextAsync(GameConsts.CharacterFile);
                DataManagement.SpreadSheet.CharacterMaster characterData =
                    JsonConvert.DeserializeObject<DataManagement.SpreadSheet.CharacterMaster>(datastr);
                using (FileStream fs = new FileStream(characterFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    string script = "/// <summary> キャラの種類 </summary>\npublic enum CharacterType{";
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

        /// <summary>
        /// マスターデータをもとに敵Enum作る
        /// </summary>
        /// <param name="masterData"></param>
        [MenuItem("Assets/Create/EnemyType")]
        public static async void CreateEnemyType()
        {
            try
            {
                string datastr = await File.ReadAllTextAsync(GameConsts.EnemyFile);
                DataManagement.SpreadSheet.EnemyMaster characterData =
                    JsonConvert.DeserializeObject<DataManagement.SpreadSheet.EnemyMaster>(datastr);
                using (FileStream fs = new FileStream(enemyFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    string script = "/// <summary> 敵の種類 </summary>public enum EnemyType{";
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

        [MenuItem("Assets/SetUp/MasterData")]
        private static void CharacterImport()
        {
            MasterData.Instance.Setup();
        }
    }
}
#endif