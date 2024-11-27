using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [Header("音の調整バー"),SerializeField] List<Slider> slider = new List<Slider>();
    [Header("音源"),SerializeField] List<AudioSource> audiosource = new List<AudioSource>();

    [Header("オプション画面"),SerializeField] GameObject option_canvas;
    [Header("ディスプレイ選択画面"), SerializeField] GameObject display_canvas;

    [Header("ディスプレイの選択ボタンをプレハブ化したもの"),SerializeField] GameObject display_prehab;

    [Header("選択ボタンの表示位置"), SerializeField] Transform trans;

    [SerializeField] List<DisplayInfo> displaylist = new List<DisplayInfo>();

    [Header("オプションを開くボタン"),SerializeField] Button option_button;

    enum Audioname
    {
        se,
        voice,
        master
    }

    void Start()
    {
        //マスターボリュームの音量調整
        slider[(int)Audioname.master].onValueChanged.AddListener(value => AudioListener.volume = value);

        //ボイスボリュームの音量調整
        slider[(int)Audioname.voice].onValueChanged.AddListener(value => audiosource[(int)Audioname.voice].volume = value);

        //SEボリュームの音量設定
        slider[(int)Audioname.se].onValueChanged.AddListener(value => audiosource[(int)Audioname.se].volume = value);

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
        //ディスプレイの情報を入手
        Screen.GetDisplayLayout(displaylist);

        var i = 0;

        foreach (var list in displaylist)
        {
            //プレハブの情報を入手
            DisplayPrehab disprehab = Instantiate(display_prehab, trans).GetComponent<DisplayPrehab>();
            disprehab.transform.Translate(0, -30 * i, 0);
            disprehab.displayname.text = "ディスプレイ" + i;
            disprehab.displaybutton.onClick.AddListener(() => Screen.MoveMainWindowTo(list, list.workArea.position));
            i++;
        }
    }

}
