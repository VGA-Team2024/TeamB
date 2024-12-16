using UnityEngine;
using UnityEngine.UI;

namespace TeamB.develop_alpha
{
    public class Enemy : MonoBehaviour
    {
        public int maxHealth = 100; // 敵の最大体力
        public int currentEnemyHealth; // 敵の現在の体力
        public float attackInterval = 2.0f; // 攻撃間隔
        public int attackDamageMin = 5; // 最小ダメージ
        public int attackDamageMax = 15; // 最大ダメージ

        private float lastAttackTime = 0.0f; // 最後に攻撃した時間
        private PlayerController player; // プレイヤーへの参照
        private GameManager gameManager; // ゲームマネージャーへの参照
        private bool isStunned = false; // 敵がスタンしているかどうか
        private float stunEndTime = 0f; // スタンが終了する時間

        void Start()
        {
            currentEnemyHealth = maxHealth; // 敵のHPを最大値で初期化
            player = FindObjectOfType<PlayerController>(); // プレイヤーを取得
            gameManager = FindObjectOfType<GameManager>(); // ゲームマネージャーを取得
        }

        void Update()
        {
            // スタン状態が終了するかどうかを確認
            if (isStunned && Time.time >= stunEndTime)
            {
                isStunned = false;
            }

            // スタン状態でない場合に攻撃
            if (!isStunned && Time.time >= lastAttackTime + attackInterval)
            {
                AttackPlayer(); // プレイヤーを攻撃
                lastAttackTime = Time.time;
            }
        }

        public void TakeDamage(int damage)
        {
            currentEnemyHealth -= damage; // HPを減少
            Debug.Log($"Enemy takes {damage} damage. Health: {currentEnemyHealth}");

            // HPが0以下になった場合
            if (currentEnemyHealth <= 0)
            {
                Debug.Log("Enemy defeated!");
                gameManager.EnemyDefeated(); // ゲームマネージャーに敵が倒されたことを通知
                Destroy(gameObject); // 敵を破棄
            }
            else
            {
                gameManager.UpdateEnemyHealthUI();
            }
        }

        void AttackPlayer()
        {
            if (player != null)
            {
                int damage = Random.Range(attackDamageMin, attackDamageMax + 1); // 攻撃ダメージを計算
                player.TakeDamage(damage); // プレイヤーにダメージを与える
                Debug.Log($"Enemy attacks player for {damage} damage.");
            }
        }

        public void SetEnemyHealth(int health)
        {
            maxHealth = health; // 最大HPを設定
            currentEnemyHealth = maxHealth; // 敵のHPを最大値で初期化
        }

        public void DisableGameplay()
        {
            isStunned = true; // 攻撃を無効化
        }

        public bool IsDefeated() // 敵が倒されたかどうかを確認するメソッド
        {
            return currentEnemyHealth <= 0; // 敵のHPが0以下の場合は倒されたとみなす
        }

        public void Stun(float duration)
        {
            isStunned = true;
            stunEndTime = Time.time + duration; // スタン終了時間を設定
            Debug.Log($"Enemy stunned for {duration} seconds.");
        }
    }
}
