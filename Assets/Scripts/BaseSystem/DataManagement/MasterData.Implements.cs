
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


namespace DataManagement
{
    /// <summary>
    /// マスターデータ管理クラス
    /// NOTE: このクラスは破壊的変更を行う可能性があるので注意
    /// </summary>
    public partial class MasterData
    {
        //設定
        const string DataPrefix = "DataAsset/MasterData";


        //マスターデータ読み込みリスト
        static public TextMaster TextMaster { get; private set; }
        static public EnemyMaster EnemyMaster { get; private set; }
        static public CharacterMaster CharacterMaster { get; private set; }


        //読み込み処理
        async UniTask MasterDataLoad()
        {
            //マスタ読み込み
            TextMaster = new TextMaster();
            EnemyMaster = new EnemyMaster();
            CharacterMaster = new CharacterMaster();
            

            await UniTask.WhenAll(new List<UniTask>()
            {
                TextMaster.Marshal(),
                EnemyMaster.Marshal(),
                CharacterMaster.Marshal(),
            });
        }
    }
}