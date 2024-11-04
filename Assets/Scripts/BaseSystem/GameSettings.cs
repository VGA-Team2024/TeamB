using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameSettings
{
    public class SceneSetting
    {
        public string BaseSceneName = "";
        public Action StartDriver = null;
        public List<string> AdditiveSceneName = new List<string>();
    }

    static public string MasterDataAPIURI =>
        "https://script.google.com/macros/s/AKfycbxEAbCfuD3MfziEM5kKY3UB8ulrMoPg_9vItGk9fzyQWLUvEP25NsfFVaVu8Jt79hogAQ/exec";


    /// <summary>
    /// チームで個別に設定してください
    /// NOTE: @PlaySceneは現在設定中のシーンが
    /// </summary>
    static Dictionary<string, SceneSetting> _sceneTypeDic = new Dictionary<string, SceneSetting>()
    {
        {
            "Ingame",
            new SceneSetting()
            {
                BaseSceneName = "@PlayScene",
                AdditiveSceneName = new List<string>()
                {
                    "IngameSystem"
                }
            }
        },
        {
            "Ingame_Debug",
            new SceneSetting()
            {
                BaseSceneName = "@PlayScene",
                AdditiveSceneName = new List<string>()
                {
                    "IngameSystem",
                    "IngameDebug"
                }
            }
        },
        {
            "ProgramTest",
            new SceneSetting()
            {
                BaseSceneName = "Night",
                AdditiveSceneName = new List<string>()
                {
                    "IngameSystem"
                }
            }
        }
    };

    public static SceneSetting GetSetting(string key) => _sceneTypeDic.ContainsKey(key)
        ? _sceneTypeDic[key]
        : throw new KeyNotFoundException("SceneSettingのキーがありません:" + key);

#if UNITY_EDITOR
    public static Dictionary<string, SceneSetting> SceneTypeDic => _sceneTypeDic;
#endif
}