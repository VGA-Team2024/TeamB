using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using TMPro;

public class MarkManager : MonoBehaviour
{
    [SerializeField] MarkDataAsset mark_asset;

    [SerializeField] List<int> randomlist = new List<int>();

    [SerializeField] List<GameObject> markobj = new List<GameObject>();

    [SerializeField] List<TextMeshProUGUI> marktext = new List<TextMeshProUGUI>();

    public bool des_mark = false; //的が消滅したら

    // Start is called before the first frame update
    void Awake()
    {
        NewParameter();
    }

    // Update is called once per frame
    void Update()
    {
        if (des_mark)
        {
            DestoryMark().Forget();
            des_mark = false;
        }

        Text();

        PlayTime();
    }



    async UniTaskVoid DestoryMark()
    {
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            RandomMark();
    }

    void RandomMark()
    {
        int rnd = UnityEngine.Random.Range(1, 101);
        int transX = UnityEngine.Random.Range(-80, 81);
        int transY = UnityEngine.Random.Range(-40, 41);
        Debug.Log(rnd);
        if (rnd <= randomlist[0])
        {
            Instantiate(markobj[0], new Vector3(transX,transY,100), Quaternion.identity);
        }
        else if(rnd <= randomlist[1])
        {
            Instantiate(markobj[1], new Vector3(transX, transY,100), Quaternion.identity);
        } 
        else if(rnd <= randomlist[2])
        {
            Instantiate(markobj[2], new Vector3(transX, transY,100), Quaternion.identity);

        }
 
    }
    //テスト用　初期化
    void NewParameter()
    {
        mark_asset.Score = 0;
        mark_asset.Time = 60;
        mark_asset.Remainder_bullet = 30;
    }
    //時間の減少、ゲームの終了
    void PlayTime()
    {
        mark_asset.Time -= Time.deltaTime;

        if(mark_asset.Time <= 0)
        {
　　　　　#if UNITY_EDITOR
          UnityEditor.EditorApplication.isPlaying = false;
　　　　　#else
   　　　　　 Application.Quit();
　　　　　#endif
        }
    }
    void Text()
    {
        marktext[0].text = "Score: " + mark_asset.Score;
        marktext[1].text = "Time: " + (int)mark_asset.Time;
        marktext[2].text = "" + mark_asset.Remainder_bullet;
        marktext[3].text = "/ " + mark_asset.Maxbullet;
    }
}
