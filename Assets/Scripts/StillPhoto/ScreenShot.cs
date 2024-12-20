using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace TeamB
{
    public class ScreenShot : MonoBehaviour
    {
        Camera cam;
        GameObject canvas;
        GameObject cameraFrame;
        string screenShotPath;
        string timeStamp;
        GameObject targetImage;

        void Awake()
        {
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            canvas = GameObject.Find("UICanvas");
            cameraFrame = GameObject.Find("Frame");
            targetImage = GameObject.Find("RawImage");
        }

        private string GetScreenShotPath()
        {
            string path = "";
            path = timeStamp + ".png";
            //path = Application.persistentDataPath+timeStamp + ".png";
            return path;
        }

        private void UIStateChange()
        {
            canvas.SetActive(!canvas.activeSelf);
        }

        private IEnumerator CreateScreenShot()
        {
            UIStateChange();
            DateTime date = DateTime.Now;
            timeStamp = date.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            yield return new WaitForEndOfFrame();

            RenderTexture renderTexture = new RenderTexture(Screen.width/2, Screen.height/2, 24);
            cam.targetTexture = renderTexture;

            Texture2D texture = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);

            texture.ReadPixels(new Rect(cameraFrame.transform.position.x- 960/2, cameraFrame.transform.position.y-540/2, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            texture.Apply();

            byte[] pngData = texture.EncodeToPNG();
            screenShotPath = GetScreenShotPath();

            
            File.WriteAllBytes(screenShotPath, pngData);

            cam.targetTexture = null;

            Debug.Log("Done!");
            UIStateChange();
        }

        

        public void ClickShootButton()
        {
            StartCoroutine(CreateScreenShot());
        }

        public void ShowSSImage()
        {
            if (!String.IsNullOrEmpty(screenShotPath))
            {
                byte[] image = File.ReadAllBytes(screenShotPath);

                Texture2D tex = new Texture2D(0, 0);
                tex.LoadImage(image);

                RawImage target = targetImage.GetComponent<RawImage>();
                target.texture = tex;
            }
        }


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
