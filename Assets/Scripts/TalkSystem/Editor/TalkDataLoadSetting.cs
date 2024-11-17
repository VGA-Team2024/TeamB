using System.Collections;
using System.Collections.Generic;
using TeamB.Editor;
using UnityEngine;

namespace TeamB.TalkSystem.Editor
{
    [CreateAssetMenu(fileName = "TalkDataLoadSetting", menuName = "TalkSystem/RemoteTalkData")]
    public class TalkDataLoadSetting : ScriptableObject
    {
        [Header("ドライブ上のスプシの各シートのロード先")]
        [InspectorVariantName("選択する授業のデータのロード先"), ReadOnly] public string RemoteClassSelectDataKey;
        [InspectorVariantName("授業内会話データのロード先"), ReadOnly] public string RemoteClassRoomTalkDataKey;
        [InspectorVariantName("授業内の選択肢データのロード先"), ReadOnly] public string RemoteClassRoomChoiceDataKey;
        [InspectorVariantName("テスト用の選択する授業のデータのロード先"), ReadOnly] public string TestRemoteClassSelectDataKey;
        [InspectorVariantName("テスト用の授業内会話データのロード先"), ReadOnly] public string TestRemoteClassRoomTalkDataKey;
        [InspectorVariantName("テスト用の授業内の選択肢データのロード先"), ReadOnly] public string TestRemoteClassRoomChoiceDataKey;
        [Header("ローカルの設定")]
        [InspectorVariantName("画像フォルダが格納されているローカルのパス"), ReadOnly] public string LocalImagePath;
    }
}
