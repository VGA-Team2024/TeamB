using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="backGroundSprite">取得するテクスチャ</param>
        /// <returns>テクスチャ</returns>
        public bool TryGetSpriteByName(string textureName, out Sprite backGroundSprite)
        {
            if (string.IsNullOrEmpty(textureName))
            {
                backGroundSprite = null;
                return false;
            }
            textureName = textureName.Trim();
            var spriteValue = _loadedSprites.FirstOrDefault((x => string.Equals(x.Key, textureName, StringComparison.OrdinalIgnoreCase)));
            backGroundSprite = spriteValue?.Sprite;
            return backGroundSprite != null;
        }
    }
    
    [Serializable]
    public class SpriteValue
    {
        public string Key;
        public Sprite Sprite;
    }
}