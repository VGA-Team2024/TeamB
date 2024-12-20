using TeamB.GameSystem.Statics;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Develop
{
	public class StudentCouncilPresidentUI : MonoBehaviour
	{
		[SerializeField] Image _slider;
		private EnemyManager _enemyManager;

		private void Awake()
		{
			_enemyManager = FindAnyObjectByType<EnemyManager>();
			_enemyManager.GetCurrentEnemyData.OnTakeDamage += OnChanged;
		}

		private void OnChanged()
		{
			float ratio = _enemyManager.GetCurrentEnemyData.GetCurrentData.Hp /
			              GameStatics.Characters[(int)_enemyManager.GetCurrentEnemyData.GetFirstCharacterType].Hp;
			_slider.fillAmount = ratio;
		}
	}
}
