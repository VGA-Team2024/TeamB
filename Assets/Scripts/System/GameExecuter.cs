using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームシステムに相当する。
/// </summary>
public class GameExecuter : GameExecuterBase
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// 次のシーン読み込みを試行する
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public void LoadNextScene(string sceneName)
    {
        var active = SceneManager.GetActiveScene();

        var sceneDepends = AssetDatabase.LoadAssetAtPath<SceneDependencies>(SceneDependencies.AssetPath);

        var ind = sceneDepends.GetAll().IndexOf(sceneDepends.GetAll().Where(scene => scene.Name == active.name).First());
        
        Debug.Log("Loading:" + sceneDepends.GetAll()[ind + 1].Name);
        
        SceneLoader.LoadSceneSimple(sceneDepends.GetAll()[ind + 1].Name);
    }

    public override void InitializeScene()
    {
    }

    public override void FinalizeScene()
    {
    }
}