using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField] List<RectTransform> result_obj;
    [SerializeField] List<TextMeshProUGUI> result_text;
    [SerializeField] Image pass_Image;
    [SerializeField] Sprite pass_Sprite;
    [SerializeField] Sprite nopass_Sprite;

    [SerializeField] float textAnimTime;
    [SerializeField] float stampAnimTime;
    //[SerializeField] float startTime;

    ResultData resultData;

    [SerializeField] GameObject result_stamp;
    Sequence seq;


    void Start()
    {
        resultData = GameStatics.resultData;

        switch (GameStatics.ExamState)
        {
            case ExamState.Tutorial:
                FirstResult();
                break;
            case ExamState.FirstExam:
                FirstResult();
                break;
            case ExamState.SecondExam:
                FirstResult();
                break;
            case ExamState.ExamClear:
                SecondResult();
                break;
        }

        StartCoroutine(MoveText());
    }

    void FirstResult()
    {
        resultData.firsttestP = resultData.defense + resultData.leftoverHp - resultData.hit;
        result_text[0].text = "防御回数 " + resultData.defense + " 回";
        result_text[1].text = "のこりHP " + resultData.leftoverHp;
        result_text[2].text = "被弾回数 " + resultData.hit + " 回";
    }

    void SecondResult()
    {
        resultData.secondtestP = resultData.leftoverTime + resultData.leftoverHp - resultData.defense;
        result_text[0].text = "残り時間 " + resultData.leftoverTime + " 秒";
        result_text[1].text = "残り体力 " + resultData.leftoverHp;
        result_text[2].text = "防御回数 " + resultData.defense + " 回";
    }

    IEnumerator Move(RectTransform _rect)
    {
        var startTime = Time.time;

        var startpos = _rect.localPosition;
        var endpos = new Vector3(300, _rect.localPosition.y, _rect.localPosition.z);

        while (Time.time - startTime < textAnimTime)
        {
            float time = (Time.time - startTime) / textAnimTime;
            _rect.localPosition = Vector3.Lerp(startpos, endpos, time);
            yield return null;
        }

        _rect.localPosition = endpos;
    }

    IEnumerator ResultStamp()
    {
        result_stamp.transform.rotation = new Quaternion(0, 0, 270, 0);
        if (GameStatics.ExamResult == ExamResult.Clear)
        {
            pass_Image.sprite = pass_Sprite;
        }
        else
        {
            pass_Image.sprite = nopass_Sprite;
        }

        result_stamp.SetActive(true);
        seq = DOTween.Sequence();
        seq.Join(result_stamp.transform.DOScale(Vector3.one * 1.5f, stampAnimTime));
        seq.Join(result_stamp.transform.DORotate(new Vector3(0, 0, 360f), stampAnimTime, RotateMode.FastBeyond360));
        seq.OnComplete(() => { CRIAudioManager.SE.Play("SE", nameof(TGS2023.SE.SE.SE_010_Stamp)); });
        yield return seq.Play();
    }

    IEnumerator MoveText()
    {
        yield return StartCoroutine(Move(result_obj[0]));
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Move(result_obj[1]));
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(Move(result_obj[2]));
        yield return new WaitForSeconds(0.5f);

        yield return ResultStamp();
        yield return new WaitForSeconds(1);
    }
}