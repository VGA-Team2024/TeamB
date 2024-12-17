using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CatchFallingScore : MonoBehaviour
{
    [SerializeField] private Text _score;

    private const string cSCORE_TEXT = "GET{0}";


    // Start is called before the first frame update
    void Start()
    {
        Setting(0);
        SetTextEnable(false);
    }

    public void SetTextEnable(bool flg)
    {
        _score.gameObject.SetActive(flg);
    }

    public void Setting(int count)
    {
        _score.text = string.Format(cSCORE_TEXT, count);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
