using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MockUp;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using TeamB.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace TeamB.GameSystem
{
    /// <summary>
    /// ゲームシステムに相当する。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // いったんもぐらたたきがミニゲームとして進行するとする
            var man = GameObject.FindAnyObjectByType<MiniGameTimeManager>();
            if (man is not null)
            {
                man.OnLimit
                    += () => { GameObject.FindAnyObjectByType<InExamUIView>().ExitExam(); };
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// 次のシーン読み込みをする
        /// </summary>
        /// <param name="currentSceneName"></param>
        /// <returns></returns>
        public void LoadNextScene(string currentSceneName)
        {
            var active = SceneManager.GetActiveScene();

            var sceneDepends = AssetDatabase.LoadAssetAtPath<SceneDependencies>(SceneDependencies.AssetPath);

            var ind = sceneDepends.GetAll()
                .IndexOf(sceneDepends.GetAll().Where(scene => scene.Name == active.name).First());

            Debug.Log("Loading:" + sceneDepends.GetAll()[ind + 1].Name);

            SceneLoader.LoadSceneSimple(sceneDepends.GetAll()[ind + 1].Name);
        }
    }
}