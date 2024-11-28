using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField] List<Slider> slider = new List<Slider>();
    [SerializeField] List<AudioSource> audiosource = new List<AudioSource>();

    [SerializeField] GameObject option_canvas;
    [SerializeField] GameObject display_canvas;

    [SerializeField] GameObject display_prehab;

    [SerializeField] Transform trans;

    [SerializeField] Button option_button;

    int i = 0;

    [SerializeField] List<DisplayInfo> displaylist = new List<DisplayInfo>();

    enum Audioname
    {
        master,
        voice,
        se
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
        Screen.GetDisplayLayout(displaylist);
        Debug.Log(displaylist.Count);

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
