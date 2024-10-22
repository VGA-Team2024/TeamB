using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MockUp
{
	/// <summary>
	/// もぐらの機能
	/// </summary>
	public class Mole : MonoBehaviour
	{
		[SerializeField, Header("最初のインターバル")] private RandomMinMax _firstInterval;

		[SerializeField, Header("潜っている時間の最小最大")]
		private RandomMinMax _random;

		[SerializeField] Collider2D _moleCollider;
		[SerializeField] Transform _moleTransform;
		[SerializeField] private float _spawnDuration;
		
		public event Action OnClicked;

		private Sequence _sequence;

		private void OnDestroy()
		{
			CancellMole();
		}

		/// <summary>
		/// モグラの上下機能
		/// </summary>
		public async void SpawnMole()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(RandomInterval(_firstInterval)));
			if (_sequence != null)
				CancellMole();
			Debug.Log("SpawnMole");
			_moleCollider.enabled = true;
			_sequence = DOTween.Sequence();
			_sequence.Append(_moleTransform.DOLocalMove(new Vector3(0, 2.54f, 0), _spawnDuration))
				.AppendInterval(1f)
				.Append(_moleTransform.DOLocalMove(Vector3.zero, _spawnDuration))
				.AppendInterval(RandomInterval(_random))
				.SetLoops(-1, LoopType.Restart);
		}

		/// <summary>
		/// モグラクリック時機能
		/// </summary>
		public void ClickedMole()
		{
			OnClicked?.Invoke();
			Debug.Log("ClickedMole");
			_moleCollider.enabled = false;
			CancellMole();
			_sequence = DOTween.Sequence();
			_sequence.Append(_moleTransform.DOLocalMove(Vector3.zero, _spawnDuration))
				.OnComplete(() => SpawnMole());
		}

		/// <summary>
		/// モグラの上下移動キャンセル
		/// </summary>
		public void CancellMole()
		{
			_sequence.Kill();
		}

		/// <summary>
		/// モグラをひっこめて、出てこなくする機能（ゲーム終了時に呼び出す）
		/// </summary>
		public void EndMole()
		{
			_moleTransform.transform.localPosition = Vector3.zero;
			_sequence.Kill();
		}

		/// <summary>
		/// ランダムな値を返す機能
		/// </summary>
		/// <param name="minMax"></param>
		/// <returns></returns>
		private float RandomInterval(RandomMinMax minMax)
		{
			float rand = Random.Range(minMax.Min, minMax.Max);
			Random.InitState((int)rand);
			return rand;
		}
	}

	[Serializable]
	public class RandomMinMax
	{
		[SerializeField, Range(0f, 100f)] public float Min;
		[SerializeField, Range(0f, 100f)] public float Max;

		public RandomMinMax(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}
}
