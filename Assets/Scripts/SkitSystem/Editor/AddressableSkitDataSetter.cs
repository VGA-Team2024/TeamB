#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace TeamB.SkitSystem.Editor
{
    public class SkitAssetPostprocessor : AssetPostprocessor
    {
        private const string SkitDataLabel = "SkitTexture";
        private const string FolderPath = "Assets/Graphics/Textures/SkitTexture";
        private const string AssetFilterTypeSprite = "t:Sprite";
        private const string AssetFilterTypeTexture2D = "t:Texture2D";

        /// <summary>
        /// アセットがインポートされたときに呼び出される
        /// </summary>
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            bool hasRelevantChanges = false;

            // フォルダ内に変更があったか確認
            foreach (var path in importedAssets)
            {
                if (!path.StartsWith(FolderPath)) continue;
                hasRelevantChanges = true;
                break;
            }

            foreach (var movedAsset in movedAssets)
            {
                if (!movedAsset.StartsWith(FolderPath)) continue;
                hasRelevantChanges = true;
                break;
            }

            // フォルダ内に変更があれば処理を実行
            if (hasRelevantChanges)
            {
                SetAddressableSkitData();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// アドレッサブルのデータを設定する
        /// </summary>
        private static void SetAddressableSkitData()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressableの設定がありません");
                return;
            }

            var guids = AssetDatabase.FindAssets(AssetFilterTypeSprite, new[] { FolderPath });
            foreach (var guid in guids)
            {
                var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                entry.SetLabel(SkitDataLabel, true);
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                entry.address = fileName;
            }
            guids = AssetDatabase.FindAssets(AssetFilterTypeTexture2D, new[] { FolderPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                // TextureTypeをSpriteに変更
                if (importer != null && importer.textureType != TextureImporterType.Sprite)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.SaveAndReimport();
                    Debug.Log($"TextureTypeをSpriteに変更しました: {path}");
                }

                // Addressableの設定
                var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                entry.SetLabel(SkitDataLabel, true);

                // ファイル名をアドレスとして設定
                var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                entry.address = fileName;
            }
        }
    }
}

#endif