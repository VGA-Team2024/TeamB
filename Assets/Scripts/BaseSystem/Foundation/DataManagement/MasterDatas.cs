using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SerializableCollections;
using UnityEditor;
using UnityEngine;

namespace DataManagement
{
    /// <summary>
    /// キャラクターマスタデータ
    /// </summary>
    [Serializable]
    public class CharacterMaster : MasterDataBase<int, CharacterMaster.CharacterData>
    {
        public override string MasterName => "CharacterMaster";
        /// <summary>
        /// スキルのデータ
        /// </summary>
        [Serializable]
        public class CharacterData
        {
            public int Id;
            public string Name;
            public string Card;
            public string ResourceName;
            /// <summary>
            /// 等級
            /// </summary>
            public int Rank;

            /// <summary>
            /// HP
            /// </summary>
            public float Hp;

            /// <summary>
            /// 命中率
            /// </summary>
            public float HitRate;

            /// <summary>
            /// 魔法の詠唱速度
            /// </summary>
            public float ChantingSpeed;

            /// <summary>
            /// 魔法攻撃力
            /// </summary>
            public float MagicATK;
            public SkillData Skill;

            public CharacterData(SpreadSheet.CharacterData data)
            {
                Id = data.Id;
                Name = data.Name;
                Card = data.Card;
                ResourceName = data.ResourceName;
                Rank = data.Rank;
                Hp = data.Hp;
                HitRate = data.HitRate;
                ChantingSpeed = data.ChantingSpeed;
                MagicATK = data.MagicATK;
            }
        }

        /// <summary>
        /// スキルのデータ
        /// </summary>
        [Serializable]
        public class SkillData
        {
            public int Id;
            public string Text;
        }

        public async UniTask<DataManagement.SpreadSheet.CharacterMaster> LoadFromFile(string masterName = "default")
        {
            if(masterName == "default")
            {
                masterName = MasterName;
            }
            return await LocalData.LoadAsync<DataManagement.SpreadSheet.CharacterMaster>(MasterData.GetFileName(masterName));
        }
        
        public override async UniTask Marshal()
        {
            SpreadSheet.CharacterMaster character = default;
            SpreadSheet.SkillMaster skill = default;
            List<UniTask> masterDataDownloads = new List<UniTask>()
            {
                MasterData.LoadMasterData("Character", (SpreadSheet.CharacterMaster data) => { character = data; }),
                MasterData.LoadMasterData("Skill", (SpreadSheet.SkillMaster data) => { skill = data; })
            };
            await masterDataDownloads;

            // 整形処理
            pretty(character.Data, (SpreadSheet.CharacterData data) => { return (data.Id, new CharacterData(data)); });
        }
    }
}