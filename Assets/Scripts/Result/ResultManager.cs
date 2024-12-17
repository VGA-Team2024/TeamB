using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [Header("�|�C���g����̃e�L�X�g�{�b�N�X"),SerializeField] List<RectTransform> result_obj;
    [Header("�|�C���g����̍���"), SerializeField] List<TextMeshProUGUI> result_text;
    [Header("���ۂ̃e�L�X�g"), SerializeField] Image pass_Image;
    [SerializeField] Sprite pass_Sprite;
    [SerializeField] Sprite nopass_Sprite;

    [Header("�e�L�X�g�{�b�N�X�̃A�j���[�V��������"), SerializeField] float textAnimTime;
    [Header("�X�^���v�̃A�j���[�V��������"), SerializeField] float stampAnimTime;
    //[SerializeField] float startTime;

    ResultData resultData;

    [Header("�X�^���v"), SerializeField] GameObject result_stamp;

    void Start()
    {
        resultData = GameStatics.resultData;

        //[��]�����_���Ɏ������o��悤�ɂ��Ă��܂��B
        int round = Random.Range(1, 3);

        if(round == 1)
        {
            FirstResult();
            Result(resultData.firsttestP, resultData.firstpass);
        }
        else
        {
            SecondResult();
            Result(resultData.secondtestP, resultData.secondpass);
        }

        StartCoroutine(MoveText());

    }
    //1�������̃|�C���g
    void FirstResult()
    {
        resultData.firsttestP = resultData.defense + resultData.leftoverHp - resultData.hit;
        result_text[0].text = "防御回数 " + resultData.defense + " 回";
        result_text[1].text = "のこりHP " + resultData.leftoverHp;
        result_text[2].text = "被弾回数 " + resultData.hit + " 回";
    }

    //2�������̃|�C���g
    void SecondResult()
    {
        resultData.secondtestP = resultData.leftoverTime + resultData.leftoverHp - resultData.defense;
        result_text[0].text = "残り時間 " + resultData.leftoverTime + " 秒";
        result_text[1].text = "残り体力 " + resultData.leftoverHp;
        result_text[2].text = "防御回数 " + resultData.defense + " 回";
    }
    //���ۂ̔���
    void Result(int _point, bool _pass)
    {
        if(_point > 20)
        {
            _pass = true;
        }

        if (_pass)
        {
            pass_Image.sprite = pass_Sprite;
        }
        else
        {
            pass_Image.sprite = nopass_Sprite;
        }
    }

    IEnumerator Move(RectTransform _rect)
    {
        var startTime = Time.time;
        
        var startpos = _rect.localPosition;
        var endpos = new Vector3(300, _rect.localPosition.y, _rect.localPosition.z);

        // �A�j���[�V�������I������܂Ń��[�v
        while (Time.time - startTime < textAnimTime)
        {
            float time = (Time.time - startTime) / textAnimTime;
            _rect.localPosition = Vector3.Lerp(startpos, endpos, time);
            yield return null;
        }

        //�ʒu��ݒ�
        _rect.localPosition = endpos;
    }

    IEnumerator ResultStamp()
    {
        var startTime = 0f;
        result_stamp.SetActive(true);

        while (startTime <= stampAnimTime)
        {
            startTime += Time.deltaTime;
            result_stamp.transform.localEulerAngles += new Vector3(0, 0, 0.04f);
            result_stamp.transform.localScale += new Vector3(0.0005f, 0.0005f, 0);
            yield return null;
        }

    }

    //���ԂɃA�j���[�V�����̎��s
    IEnumerator MoveText()
    {
        yield return StartCoroutine(Move(result_obj[0]));
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(Move(result_obj[1]));
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(Move(result_obj[2]));
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(ResultStamp());
        yield return new WaitForSeconds(2);
    }
}

