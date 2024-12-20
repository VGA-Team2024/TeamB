using System.Collections;
using System.Collections.Generic;
using TeamB.UI;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    [SerializeField] List<Slider> slider = new List<Slider>();

    [SerializeField] GameObject option_canvas;
    [SerializeField] GameObject display_canvas;

    [SerializeField] GameObject display_prefab;

    [SerializeField] Transform trans;

    [SerializeField] Button option_button;

    [SerializeField] List<DisplayInfo> displaylist = new List<DisplayInfo>();
    
    TitleUIView title_uiview;

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
        title_uiview = FindObjectOfType<TitleUIView>();
        slider[(int)Audioname.master].onValueChanged.AddListener(value => AudioListener.volume = value);

        slider[(int)Audioname.voice].onValueChanged.AddListener(value => CRIAudioManager.BGM.SetVolume(value));

        slider[(int)Audioname.se].onValueChanged.AddListener(value => CRIAudioManager.BGM.SetVolume(value));

        option_button.onClick.AddListener(
            () =>
            {
                option_canvas.SetActive(true);
                title_uiview.ClickSound();
            });

        DisplayInt();
    }

    public void BackButton()
    {
        option_canvas.SetActive(false);
    }

    public void DisplayBackButton()
    {
        display_canvas.SetActive(false);
        option_canvas.SetActive(true);
    }

    public void DisplayChange()
    {
        display_canvas.SetActive(true);
        option_canvas.SetActive(false);
    }

    void DisplayInt()
    {
        Screen.GetDisplayLayout(displaylist);
        var i = 0;

        foreach (var list in displaylist)
        {
            DisplayPrefab disprefab = Instantiate(display_prefab, trans).GetComponent<DisplayPrefab>();
            disprefab.transform.Translate(0, -100 * i, 0);
            disprefab.displayname.text = "ディスプレイ" + i;
            disprefab.displaybutton.onClick.AddListener(() => Screen.MoveMainWindowTo(list, list.workArea.position));
            i++;
        }
    }

}
