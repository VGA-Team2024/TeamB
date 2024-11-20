using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TeamB.SkitSystem
{
    public class SkitResourceLoader : MonoBehaviour
    {
        [SerializeField] private List<SpriteValue> _loadedSprites = new();
        [SerializeField] private string _label = "SkitTexture";
        
        public async UniTask InitializeSkitResourceLoader()
        {
            var handle = Addressables.LoadAssetsAsync<Sprite>(_label, null);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var sprite in handle.Result)
                {
                    _loadedSprites.Add(new SpriteValue()
                    {
                        Key = sprite.name,
                        Sprite = sprite
                    }); // 名前をキーに辞書に格納
                }
            }
            else
            {
                Debug.LogError("Failed to load assets with label: " + _label);
            }
        }
        
        /// <summary>
        /// 特定のテクスチャを取得
        /// </summary>
        /// <param name="textureName">取得したいテクスチャの名前</param>
        /// <returns>テクスチャ</returns>
        public Sprite GetSpriteByName(string textureName)
        {
            var spriteValue = _loadedSprites.Find(x => x.Key == textureName);
            if (spriteValue != null)
            {
                return spriteValue.Sprite;
            }

            Debug.LogWarning($"Texture with name '{textureName}' not found.");
            return null;
        }
    }
    
    [Serializable]
    public class SpriteValue
    {
        public string Key;
        public Sprite Sprite;
    }
}