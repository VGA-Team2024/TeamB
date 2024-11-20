#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace TeamB.SkitSystem.Editor
{
    public class AddressableSkitDataSetter
    {
        private const string SkitDataLabel = "SkitTexture";
        private const string FolderPath = "Assets/Graphics/Textures/SkitTexture";
        
        [UnityEditor.MenuItem("Tools/SetAddressableSkitData")]
        public static void SetAddressableSkitData()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogError("Addressable Asset Settings not found.");
                return;
            }

            var guids = AssetDatabase.FindAssets("t:Sprite", new[] { FolderPath });
            foreach (var guid in guids)
            {
                var entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                entry.SetLabel(SkitDataLabel, true);
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                entry.address = fileName;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Addressable SkitData set successfully.");
        }
    }
}

#endif