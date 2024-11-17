#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace TeamB.TalkSystem.Editor
{
    public class TalkAssetSettingEditor : EditorWindow
    {
        private TalkDataLoadSetting _talkDataLoadSetting;
        private const string TalkDataLoadSettingKey = "TalkDataLoadSettingGUID";
        private const string TalkDataLoadSettingPath = "Assets/DataAsset/MasterData/ConstantTalkData";
        private string _classSelectDataKey;
        private string _classRoomTalkDataKey;
        private string _classRoomChoiceDataKey;
        private string _testRemoteClassSelectDataKey;
        private string _testRemoteClassRoomTalkDataKey;
        private string _testRemoteClassRoomChoiceDataKey;
        private bool _isFoldoutOpen;
       
        // メニューからウィンドウを開くための関数
        [MenuItem("Tools/TalkSystem/Talk AssetSetting Editor")]
        public static void ShowWindow()
        {
            // ウィンドウを作成して表示
            GetWindow<TalkAssetSettingEditor>("Talk AssetSetting Editor");
        }

        // ウィンドウが描画されるたびに呼ばれる
        private void OnGUI()
        {
            // IMGUIコードでUIを描画
            GUILayout.Label("リモートからロードする際の設定一覧");

            SetRemoteLoadSetting();
        }
        
        private void OnEnable()
        {
            // ウィンドウが有効化されたときに設定を復元
            var guid = EditorPrefs.GetString(TalkDataLoadSettingKey, null);
            if (string.IsNullOrEmpty(guid)) return;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            _talkDataLoadSetting = AssetDatabase.LoadAssetAtPath<TalkDataLoadSetting>(assetPath);
        }

        private void OnDisable()
        {
            // ウィンドウが無効化されたときに設定を保存
            if (_talkDataLoadSetting != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(_talkDataLoadSetting);
                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                EditorPrefs.SetString(TalkDataLoadSettingKey, guid);
            }
            else
            {
                EditorPrefs.DeleteKey(TalkDataLoadSettingKey);
            }
        }

        #region RemoteLoadSetting
        private void SetRemoteLoadSetting()
        {
            // 折りたたみ可能なセクション
            _isFoldoutOpen = EditorGUILayout.Foldout(_isFoldoutOpen, "設定データの保存先");
            if (!_isFoldoutOpen) return;
            _talkDataLoadSetting = (TalkDataLoadSetting)EditorGUILayout.ObjectField(
                "設定データの保存先",
                _talkDataLoadSetting,
                typeof(TalkDataLoadSetting),
                false
            );

            if (_talkDataLoadSetting == null)
            {
                EditorGUILayout.HelpBox("設定データを設定、または作成してください", MessageType.Warning);
                if (!GUILayout.Button("Create")) return;

                _talkDataLoadSetting = CreateInstance<TalkDataLoadSetting>();
                GUILayout.Label("LoadSettingの保存先", EditorStyles.boldLabel);
                AssetDatabase.CreateAsset(_talkDataLoadSetting,
                    $"{TalkDataLoadSettingPath}/{nameof(TalkDataLoadSetting)}.asset");
                AssetDatabase.SaveAssets();
                return;
            }

            GUILayout.Label("リモートからロードする際の設定一覧", EditorStyles.boldLabel);
            _classSelectDataKey = EditorGUILayout.TextField(_talkDataLoadSetting.RemoteClassSelectDataKey);
            GUILayout.Label("授業内会話データのロード先", EditorStyles.boldLabel);
            _classRoomTalkDataKey = EditorGUILayout.TextField(_talkDataLoadSetting.RemoteClassRoomTalkDataKey);
            GUILayout.Label("授業内の選択肢データのロード先", EditorStyles.boldLabel);
            _classRoomChoiceDataKey = EditorGUILayout.TextField(_talkDataLoadSetting.RemoteClassRoomChoiceDataKey);
            GUILayout.Label("テスト用の選択する授業のデータのロード先", EditorStyles.boldLabel);
            _testRemoteClassSelectDataKey =
                EditorGUILayout.TextField(_talkDataLoadSetting.TestRemoteClassSelectDataKey);
            GUILayout.Label("テスト用の授業内会話データのロード先", EditorStyles.boldLabel);
            _testRemoteClassRoomTalkDataKey =
                EditorGUILayout.TextField(_talkDataLoadSetting.TestRemoteClassRoomTalkDataKey);
            GUILayout.Label("テスト用の授業内の選択肢データのロード先", EditorStyles.boldLabel);
            _testRemoteClassRoomChoiceDataKey =
                EditorGUILayout.TextField(_talkDataLoadSetting.TestRemoteClassRoomChoiceDataKey);

            if (GUILayout.Button("Save"))
            {
                SaveData();
            }
        }

        private void SaveData()
        {
            if (_talkDataLoadSetting == null)
            {
                Debug.LogError("No ScriptableObject selected!");
                return;
            }

            // スクリプタブルオブジェクトに値を代入
            _talkDataLoadSetting.RemoteClassSelectDataKey = _classSelectDataKey;
            _talkDataLoadSetting.RemoteClassRoomTalkDataKey = _classRoomTalkDataKey;
            _talkDataLoadSetting.RemoteClassRoomChoiceDataKey = _classRoomChoiceDataKey;
            _talkDataLoadSetting.TestRemoteClassSelectDataKey = _testRemoteClassSelectDataKey;
            _talkDataLoadSetting.TestRemoteClassRoomTalkDataKey = _testRemoteClassRoomTalkDataKey;
            _talkDataLoadSetting.TestRemoteClassRoomChoiceDataKey = _testRemoteClassRoomChoiceDataKey;

            // 変更を記録し、保存
            EditorUtility.SetDirty(_talkDataLoadSetting);
            AssetDatabase.SaveAssets();

            Debug.Log($"Saved: {_talkDataLoadSetting.name}を保存しました。");
        }
        #endregion

        public static void AddImagesFromFolder()
        {
            var targetFolder = "Assets/Images"; // 指定フォルダのパス
            var labelName = "MyLabel"; // 指定するラベル名

            // Addressable Settingsの取得
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(false);

            // フォルダ内のすべての画像ファイルを取得
            var imagePaths = AssetDatabase.FindAssets("t:Texture2D", new[] { targetFolder });

            foreach (var guid in imagePaths)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);

                // ラベルを設定
                if (!entry.labels.Contains(labelName))
                {
                    entry.SetLabel(labelName, true);
                    Debug.Log($"Addressable化: {assetPath} にラベル {labelName} を設定しました。");
                }
            }

            // 保存
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif