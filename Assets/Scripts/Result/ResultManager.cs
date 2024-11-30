using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using TeamB.GameSystem.Statics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [Header("ポイント内訳のテキストボックス"),SerializeField] List<RectTransform> result_obj;
    [Header("ポイント内訳の項目"), SerializeField] List<TextMeshProUGUI> result_text;
    [Header("合否のテキスト"), SerializeField] TextMeshProUGUI pass_Text;

    [Header("テキストボックスのアニメーション時間"), SerializeField] float textAnimTime;
    [Header("スタンプのアニメーション時間"), SerializeField] float stampAnimTime;
    //[SerializeField] float startTime;

    ResultData resultData;

    [Header("スタンプ"), SerializeField] GameObject result_stamp;

    void Start()
    {
        resultData = GameStatics.resultData;

        //[仮]ランダムに試験が出るようにしています。
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
    //1次試験のポイント
    void FirstResult()
    {
        resultData.firsttestP = resultData.defense + resultData.leftoverHp - resultData.hit;
        result_text[0].text = "防御した回数 " + resultData.defense + " 回";
        result_text[1].text = "プレイヤー残り体力 " + resultData.leftoverHp;
        result_text[2].text = "被弾回数 " + resultData.hit + " 回";
    }

    //2次試験のポイント
    void SecondResult()
    {
        resultData.secondtestP = resultData.leftoverTime + resultData.leftoverHp - resultData.defense;
        result_text[0].text = "残り時間 " + resultData.leftoverTime + " 秒";
        result_text[1].text = "プレイヤー残り体力 " + resultData.leftoverHp;
        result_text[2].text = "防御回数 " + resultData.defense + " 回";
    }
    //合否の判定
    void Result(int _point, bool _pass)
    {
        if(_point > 20)
        {
            _pass = true;
        }

        if (_pass)
        {
            pass_Text.text = "合格";
        }
        else
        {
            pass_Text.text = "不合格";
        }
    }

    IEnumerator Move(RectTransform _rect)
    {
        var startTime = Time.time;
        
        var startpos = _rect.localPosition;
        var endpos = new Vector3(300, _rect.localPosition.y, _rect.localPosition.z);

        // アニメーションが終了するまでループ
        while (Time.time - startTime < textAnimTime)
        {
            float time = (Time.time - startTime) / textAnimTime;
            _rect.localPosition = Vector3.Lerp(startpos, endpos, time);
            yield return null;
        }

        //位置を設定
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

    //順番にアニメーションの実行
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

