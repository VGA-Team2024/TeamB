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
            }
        }

        [Serializable]
        public class CharacterMaster : SpreadSheetDataObject
        {
            public CharacterData[] Data;
        }
    }
}