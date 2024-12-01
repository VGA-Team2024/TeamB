using UnityEngine;

namespace TeamB.Develop.Develop
{
    public class SceneManager : MonoBehaviour
    {
        public void NextScene(string sceneName)
        {
            SceneLoader.LoadScene(sceneName);
        }
    }
}