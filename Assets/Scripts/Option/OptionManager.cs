using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [Header("音量調節バー"), SerializeField] List<Slider> slider = new List<Slider>();

    [Header("オプション画面"), SerializeField] GameObject option_canvas;
    [Header("ディスプレイ選択画面"), SerializeField] GameObject display_canvas;

    [Header("ディスプレイの選択ボタンをプレハブ化したもの"),SerializeField] GameObject display_prefab;

    [Header("ディスプレイ選択ボタンの座標"), SerializeField] Transform trans;

    [Header("オプション画面を開くボタン"), SerializeField] Button option_button;

    [SerializeField] List<DisplayInfo> displaylist = new List<DisplayInfo>();

    enum Audioname
    {
        se,
        voice,
        master
    }
    void Awake()
    {
        CRIAudioManager.Initialize();
    }
    void Start()
    {
        //マスターボリュームの音量調整
        slider[(int)Audioname.master].onValueChanged.AddListener(value => AudioListener.volume = value);

        //ボイスボリュームの音量調整
        slider[(int)Audioname.voice].onValueChanged.AddListener(value => CRIAudioManager.BGM.SetVolume(value));

        //SEボリュームの音量設定
        slider[(int)Audioname.se].onValueChanged.AddListener(value => CRIAudioManager.BGM.SetVolume(value));

        //オプション画面を開くボタン
        option_button.onClick.AddListener(() => option_canvas.SetActive(true));

        DisplayInt();
    }

    //戻るボタンを押しオプション画面を閉じる
    public void BackButton()
    {
        option_canvas.SetActive(false);
    }

    //ディスプレイ選択画面からオプション画面に戻る
    public void DisplayBackButton()
    {
        display_canvas.SetActive(false);
        option_canvas.SetActive(true);
    }

    //表示ディスプレイの変更
    public void DisplayChange()
    {
        display_canvas.SetActive(true);
        option_canvas.SetActive(false);
    }

    void DisplayInt()
    {
        //ディスプレイのデータ取得
        Screen.GetDisplayLayout(displaylist);
        var i = 0;

        foreach (var list in displaylist)
        {
            //プレハブの情報を入手
            DisplayPrefab disprefab = Instantiate(display_prefab, trans).GetComponent<DisplayPrefab>();
            disprefab.transform.Translate(0, -30 * i, 0);
            disprefab.displayname.text = "ディスプレイ" + i;
            disprefab.displaybutton.onClick.AddListener(() => Screen.MoveMainWindowTo(list, list.workArea.position));
            i++;
        }
    }

}
