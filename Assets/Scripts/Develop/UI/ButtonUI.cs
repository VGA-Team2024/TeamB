using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ButtonUI : MonoBehaviour
{

	public void AnimWhenPointerEnter()
	{
		transform.DOScale(1.1f, 0.5f).SetLink(gameObject).SetEase(Ease.OutBounce).OnComplete(() =>
		{
			transform.DOScale(1, 0.5f);
		});
	}
}
