using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OnClickEffect : MonoBehaviour
{
    [SerializeField] GameObject clickEffect;

    [SerializeField] GameObject drugEffect;
    [SerializeField] GameObject test;

    Vector2 tapPosition;

    Vector2 dragPosition;

    bool drageffect = false;

    Coroutine drugEffectCoroutine;
    // Update is called once per frame
    void Update()
    {
        Vector3 screen_trans = Input.mousePosition;
        screen_trans.z = 10f;
        tapPosition = Camera.main.ScreenToWorldPoint(screen_trans); // 現在のタップ位置を取得
        test.transform.position = tapPosition;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(ClickEffect());
        }

        if (Input.GetKey(KeyCode.Mouse0) && !drageffect)
        {
            drageffect = true;
            drugEffectCoroutine = StartCoroutine(DrugEffect());
            Debug.Log("okokok");
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (drugEffectCoroutine != null)
            {
                StopCoroutine(drugEffectCoroutine);
                drugEffectCoroutine = null;  
            }
            drugEffect.SetActive(false);
            StartCoroutine(ClickEffect());
            drageffect = false;
        }
    }

    IEnumerator ClickEffect()
    {
        clickEffect.SetActive(true);
        clickEffect.transform.position = tapPosition;
        yield return new WaitForSeconds(0.5f);
        clickEffect.SetActive(false);
    }

    IEnumerator DrugEffect()
    {
        drugEffect.SetActive(true);
        Vector2 beforeTapPosition = tapPosition;
        Debug.Log(beforeTapPosition);

        while (true)
        {
            // エフェクトの位置を更新
            drugEffect.transform.position = tapPosition;

            // 現在の位置と前回の位置の差を計算
            Vector2 direction = tapPosition - beforeTapPosition;

            // ベクトルの方向から角度を計算（ラジアンを度に変換）
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            drugEffect.transform.eulerAngles = new Vector3(0, 0, angle);
            Debug.Log(angle);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
