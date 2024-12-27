using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderFillOriginController : MonoBehaviour
{
    [SerializeField] Slider cutInEffect;
    [SerializeField] Slider cutInCut;

    [SerializeField] Image cutInEffectFillImage;
    [SerializeField] Image cutInCutFillImage;

    void Start()
    {
        if (cutInEffect == null)
        {
            return;
        }
        if (cutInCut == null)
        {
            return;
        }

        Transform cutInEffectFillArea = cutInEffect.transform.Find("Fill Area");
        if (cutInEffectFillArea != null)
        {
            Transform fill = cutInEffectFillArea.Find("Fill");
            if (fill != null)
            {
                cutInEffectFillImage = fill.GetComponent<Image>();
                if (cutInEffectFillImage == null)
                {
                }
            }
            else
            {
            }
        }
        else
        {
        }

        Transform cutInCutFillArea = cutInCut.transform.Find("Fill Area");
        if (cutInCutFillArea != null)
        {
            Transform fill = cutInCutFillArea.Find("Fill");
            if (fill != null)
            {
                cutInCutFillImage = fill.GetComponent<Image>();
                if (cutInCutFillImage == null)
                {
                }
            }
            else
            {
            }
        }
        else
        {
        }
    }

    /// <summary>
    /// CutInEffect
    /// </summary>
    public void SetCutInEffectFillOriginLeft()
    {
        if (cutInEffectFillImage != null)
        {
            cutInEffectFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }

    public void SetCutInCutFillOriginLeft()
    {
        if (cutInCutFillImage != null)
        {
            cutInCutFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }

    public void SetCutInEffectFillOriginRight()
    {
        if (cutInEffectFillImage != null)
        {
            cutInEffectFillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }

    public void SetCutInCutFillOriginRight()
    {
        if (cutInCutFillImage != null)
        {
            cutInCutFillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }
}

