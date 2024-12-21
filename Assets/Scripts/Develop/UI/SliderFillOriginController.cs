using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderFillOriginController : MonoBehaviour
{
    [SerializeField] Slider cutInEffect; // 操作するスライダー
    [SerializeField] Slider cutInCut; // 操作するスライダー

    [SerializeField] Image cutInEffectFillImage; // Fill 部分の Image コンポーネント
    [SerializeField] Image cutInCutFillImage; // Fill 部分の Image コンポーネント

    void Start()
    {
        if (cutInEffect == null)
        {
            Debug.LogError("ターゲットスライダーが設定されていません！");
            return;
        }
        if (cutInCut == null)
        {
            Debug.LogError("ターゲットスライダーが設定されていません！");
            return;
        }

        // Fill Area -> Fill の Image コンポーネントを取得
        Transform cutInEffectFillArea = cutInEffect.transform.Find("Fill Area");
        if (cutInEffectFillArea != null)
        {
            Transform fill = cutInEffectFillArea.Find("Fill");
            if (fill != null)
            {
                cutInEffectFillImage = fill.GetComponent<Image>();
                if (cutInEffectFillImage == null)
                {
                    Debug.LogError("Fill に Image コンポーネントが見つかりません！");
                }
            }
            else
            {
                Debug.LogError("Fill Area に Fill オブジェクトが見つかりません！");
            }
        }
        else
        {
            Debug.LogError("スライダーに Fill Area が見つかりません！");
        }

        // Fill Area -> Fill の Image コンポーネントを取得
        Transform cutInCutFillArea = cutInCut.transform.Find("Fill Area");
        if (cutInCutFillArea != null)
        {
            Transform fill = cutInCutFillArea.Find("Fill");
            if (fill != null)
            {
                cutInCutFillImage = fill.GetComponent<Image>();
                if (cutInCutFillImage == null)
                {
                    Debug.LogError("Fill に Image コンポーネントが見つかりません！");
                }
            }
            else
            {
                Debug.LogError("Fill Area に Fill オブジェクトが見つかりません！");
            }
        }
        else
        {
            Debug.LogError("スライダーに Fill Area が見つかりません！");
        }
    }

    /// <summary>
    /// CutInEffect の Fill Origin を左からに設定
    /// </summary>
    public void SetCutInEffectFillOriginLeft()
    {
        if (cutInEffectFillImage != null)
        {
            cutInEffectFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            Debug.Log("Fill Origin を左からに設定しました！");
        }
    }

    /// <summary>
    /// CutInCut の　Fill Origin を左からに設定
    /// </summary>
    public void SetCutInCutFillOriginLeft()
    {
        if (cutInCutFillImage != null)
        {
            cutInCutFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            Debug.Log("Fill Origin を左からに設定しました！");
        }
    }

    /// <summary>
    /// CutInEffect の　Fill Origin を右からに設定
    /// </summary>
    public void SetCutInEffectFillOriginRight()
    {
        if (cutInEffectFillImage != null)
        {
            cutInEffectFillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
            Debug.Log("Fill Origin を右からに設定しました！");
        }
    }

    /// <summary>
    /// CutInCut の　Fill Origin を右からに設定
    /// </summary>
    public void SetCutInCutFillOriginRight()
    {
        if (cutInCutFillImage != null)
        {
            cutInCutFillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
            Debug.Log("Fill Origin を右からに設定しました！");
        }
    }
}

