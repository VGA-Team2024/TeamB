using System.Collections;
using System.Collections.Generic;
using TGS2023.BGM;
using UnityEngine;

public class CreditView : MonoBehaviour
{
    void Start()
    {
        CRIAudioManager.BGM.Play("BGM", nameof(BGM.BGM_001_title));
    }

}
