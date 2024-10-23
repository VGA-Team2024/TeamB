using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCount : MonoBehaviour
{



    [SerializeField] private Text _startCount;

    private const string cSTART_COUNT_TEXT = " {0} ";
    private const string cSTART_TEXT_ = "START";
    private const string cSTART_ENABLED_TEXT = "";

    // Start is called before the first frame update
    void Start()
    {
        Setting(3);
        SetTextEnable(true);
    }

    public void SetTextEnable(bool flg)
    {
        _startCount.gameObject.SetActive(flg);
    }

    public void Setting(int count)
    {
        _startCount.text = string.Format(cSTART_COUNT_TEXT, count);
    }

    public void SettingEnd()
    {
        _startCount.text = cSTART_TEXT_;
    }

    public void SettingEnabled()
    {
        _startCount.text = cSTART_ENABLED_TEXT;
    }


    // Update is called once per frame
    void Update()
    {

    }
}