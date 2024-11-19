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
        private Dictionary<string, Sprite> _loadedSprites = new Dictionary<string, Sprite>();
        [SerializeField] private string _label = "SkitTexture";
        
        public async UniTask InitializeSkitResourceLoader()
        {
            var handle = Addressables.LoadAssetsAsync<Sprite>(_label, null);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var sprite in handle.Result)
                {
                    _loadedSprites[sprite.name] = sprite; // 名前をキーに辞書に格納
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
            if (_loadedSprites.TryGetValue(textureName, out var texture))
            {
                return texture;
            }

            Debug.LogWarning($"Texture with name '{textureName}' not found.");
            return null;
        }
    }
}