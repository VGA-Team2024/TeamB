using System;

/// <summary>
/// スプレッドシートからダウンロードしてくるデータたち
/// </summary>
namespace DataManagement
{
    public interface IDataManagement
    {
        
    }
    
    /// <summary>
    /// システム用
    /// </summary>
    [Serializable]
    public class SpreadSheetMasterVersion
    {
        public string SheetName;
        public int Version;
    }

    [Serializable]
    public class SpreadSheetDataObject
    {
        public int Version;
        //xxx[] Data;
    }

    [Serializable]
    public class MasterDataVersion
    {
        public long TimeStamp;
        public SpreadSheetMasterVersion[] Data;
    }

    namespace SpreadSheet
    {
        /// <summary>
        /// テキストデータ
        /// </summary>
        [Serializable]
        public class TextData
        {
            public string Key;
            public string Text;
        }

        [Serializable]
        public class TextMaster : SpreadSheetDataObject
        {
            public TextData[] Data;
        }

        /// <summary>
        /// サンプルの敵データ
        /// </summary>
        [Serializable]
        public class EnemyData : IDataManagement
        {
            public int Id;
            public string Name;
            public string Card;
            public string ResourceName;
            public float Hp;
            public float HitRate;
            public float AttackSpeed;
            public float ATK;
            //public int SkillId;
        }

        [Serializable]
        public class EnemyMaster : SpreadSheetDataObject
        {
            public EnemyData[] Data;
        }

        /// <summary>
        /// スキルのデータ
        /// </summary>
        [Serializable]
        public class SkillData : IDataManagement
        {
            public int Id;
            public string Text;
        }

        [Serializable]
        public class SkillMaster : SpreadSheetDataObject
        {
            public SkillData[] Data;
        }
        //


        /// <summary>
        /// キャラクターデータ
        /// </summary>
        [Serializable]
        public class CharacterData : IDataManagement
        {
            public int Id;
            public string Name;
            public string Card;
            public string ResourceName;
            public float Hp;
            public float HitRate;
            public float ChantingSpeed;
            public float MagicATK;
            public float AvoidanceRate;

            public CharacterData(CharacterData data)
            {
                Id = data.Id;
                Name = data.Name;
                Card = data.Card;
                ResourceName = data.ResourceName;
                Hp = data.Hp;
                HitRate = data.HitRate;
                ChantingSpeed = data.ChantingSpeed;
                MagicATK = data.MagicATK;
                AvoidanceRate = data.AvoidanceRate;
            }

            public CharacterData(string[] data)
            {
                Id = int.Parse(data[0]);
                Name = data[1];
                Card = data[2];
                ResourceName = data[3];
                Hp = float.Parse(data[4]);
                MagicATK = float.Parse(data[5]);
                ChantingSpeed = float.Parse(data[6]);
                HitRate = float.Parse(data[7]);
                AvoidanceRate = float.Parse(data[8]);
            }
        }

        [Serializable]
        public class CharacterMaster : SpreadSheetDataObject
        {
            public CharacterData[] Data;
        }
    }
}