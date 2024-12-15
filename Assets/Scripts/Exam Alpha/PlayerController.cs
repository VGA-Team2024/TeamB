using UnityEngine;
using UnityEngine.UI;

namespace TeamB.develop_alpha
{

    public class PlayerController : MonoBehaviour
    {
        // プレイヤーの設定
        public int maxHealth = 100; // プレイヤーの最大体力
        public int currentHealth; // プレイヤーの現在の体力

        // UI関連
        public GameObject keyBubble; // 吹き出しUIのオブジェクト
        public Text keyText; // 吹き出しのテキスト
        public Image skill1IconFill; // スキル1アイコンのフィル
        public Image skill2IconFill; // スキル2アイコンのフィル
        public Image skill3IconFill; // スキル3アイコンのフィル
        public Slider healthSlider; // プレイヤーのHPを表示するスライダー

        // スキルのクールダウン設定
        public float skill1Cooldown = 7.0f; // スキル1のクールダウン
        public float skill2Cooldown = 12.0f; // スキル2のクールダウン
        public float skill3Cooldown = 20.0f; // スキル3のクールダウン

        // 内部変数
        private char currentKey; // 現在のキー
        private bool isBubbleActive = false; // 吹き出しがアクティブかどうか
        private float skill1LastUsedTime; // スキル1の最終使用時刻
        private float skill2LastUsedTime; // スキル2の最終使用時刻
        private float skill3LastUsedTime; // スキル3の最終使用時刻

        private int successfulKeyPresses = 0; // 成功したキー入力のカウント
        private const float minCooldown = 1.0f; // スキルの最小クールダウン時間
        private Enemy enemyTarget; // スキルの対象となる敵
        private bool gameStarted = false; // ゲームの開始状態
        private GameManager gameManager; // GameManagerへの参照

        void Start()
        {
            currentHealth = maxHealth; // 現在の体力を最大に設定
            healthSlider.maxValue = maxHealth; // HPスライダーの最大値を設定
            UpdateHealthSlider(); // HPスライダーを更新
            GenerateRandomKey(); // ランダムなキーを生成
            enemyTarget = FindObjectOfType<Enemy>(); // シーン内のEnemyを取得
            gameManager = FindObjectOfType<GameManager>(); // GameManagerを取得
        }

        void Update()
        {
            if (gameStarted)
            {
                UpdateSkillCooldownUI(); // スキルのクールダウンUIを更新

                // 吹き出しのキー入力処理
                if (isBubbleActive && Input.GetKeyDown(currentKey.ToString().ToLower()))
                {
                    Debug.Log("Correct key pressed: " + currentKey);
                    successfulKeyPresses++;
                    gameManager.AddPoints(50); // 成功したキー入力時にポイントを加算
                    GenerateRandomKey(); // 次のキーを生成

                    // 3回成功するとクールダウンを短縮
                    if (successfulKeyPresses >= 3)
                    {
                        ReduceCooldowns();
                        successfulKeyPresses = 0;
                    }
                }

                // スキル1の使用
                if (Input.GetKeyDown(KeyCode.I) && CanUseSkill1())
                {
                    UseSkill1();
                    gameManager.AddPoints(100); // スキル1使用時にポイントを加算
                    successfulKeyPresses = 0;
                }

                // スキル2の使用
                if (Input.GetKeyDown(KeyCode.O) && CanUseSkill2())
                {
                    UseSkill2();
                    gameManager.AddPoints(150); // スキル2使用時にポイントを加算
                    successfulKeyPresses = 0;
                }

                // スキル3の使用
                if (Input.GetKeyDown(KeyCode.P) && CanUseSkill3())
                {
                    UseSkill3();
                    gameManager.AddPoints(200); // スキル3使用時にポイントを加算
                    successfulKeyPresses = 0;
                }
            }
        }

        // ゲーム開始時の初期化処理
        public void StartGame()
        {
            gameStarted = true; // ゲームを開始状態に設定
            currentHealth = maxHealth; // 体力を最大にリセット
            skill1LastUsedTime = Time.time; // スキルの使用時刻をリセット
            skill2LastUsedTime = Time.time;
            skill3LastUsedTime = Time.time;
            GenerateRandomKey(); // ランダムなキーを生成
            UpdateHealthSlider(); // HPスライダーを更新
        }

        // ランダムなキーを生成する
        void GenerateRandomKey()
        {
            char[] keys = { 'W', 'A', 'S', 'D' };
            currentKey = keys[Random.Range(0, keys.Length)];
            keyText.text = currentKey.ToString();
            keyBubble.SetActive(true);
            isBubbleActive = true;
        }

        // 新しい敵を設定する
        public void SetEnemyTarget(Enemy newEnemy)
        {
            enemyTarget = newEnemy;
        }

        // プレイヤーがダメージを受ける処理
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            UpdateHealthSlider(); // HPスライダーを更新
            Debug.Log($"Player takes {damage} damage. Health: {currentHealth}");

            if (currentHealth <= 0)
            {
                Debug.Log("Player is defeated!");
                gameManager.EndGame();
            }
        }

        // スキル1の使用処理
        void UseSkill1()
        {
            Debug.Log("Skill 1 activated! Dealing 20 damage to the enemy.");
            skill1LastUsedTime = Time.time; // 最終使用時刻を更新
            skill1Cooldown = 7.0f; // スキル1のクールダウンを初期化

            if (enemyTarget != null)
            {
                enemyTarget.TakeDamage(20); // 敵にダメージを与える
                if (enemyTarget.IsDefeated())
                {
                    gameManager.EnemyDefeated(); // 敵が倒れたときにGameManagerに通知
                    gameManager.AddPoints(300); // 敵を倒した時にポイントを加算
                }
            }
        }

        // スキル2の使用処理
        void UseSkill2()
        {
            Debug.Log("Skill 2 activated! Healing player for 15 health.");
            currentHealth += 15;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            skill2LastUsedTime = Time.time; // 最終使用時刻を更新
            skill2Cooldown = 12.0f; // スキル2のクールダウンを初期化
            UpdateHealthSlider(); // HPスライダーを更新
        }

        // スキル3の使用処理
        void UseSkill3()
        {
            Debug.Log("Skill 3 activated! Stopping enemy's action for 5 seconds.");
            skill3LastUsedTime = Time.time; // 最終使用時刻を更新
            skill3Cooldown = 15.0f; // スキル3のクールダウンを初期化
            if (enemyTarget != null)
            {
                enemyTarget.Stun(5.0f); // 5秒間敵の行動を停止
            }
        }

        // HPスライダーの更新
        void UpdateHealthSlider()
        {
            healthSlider.value = currentHealth;
        }

        // スキルのクールダウンUIを更新する
        void UpdateSkillCooldownUI()
        {
            skill1IconFill.fillAmount = (Time.time - skill1LastUsedTime) / skill1Cooldown; // スキル1のクールダウン更新
            skill2IconFill.fillAmount = (Time.time - skill2LastUsedTime) / skill2Cooldown; // スキル2のクールダウン更新
            skill3IconFill.fillAmount = (Time.time - skill3LastUsedTime) / skill3Cooldown; // スキル3のクールダウン更新
        }

        // スキル1が使用可能か確認
        bool CanUseSkill1()
        {
            return (Time.time - skill1LastUsedTime) >= skill1Cooldown;
        }

        // スキル2が使用可能か確認
        bool CanUseSkill2()
        {
            return (Time.time - skill2LastUsedTime) >= skill2Cooldown;
        }

        // スキル3が使用可能か確認
        bool CanUseSkill3()
        {
            return (Time.time - skill3LastUsedTime) >= skill3Cooldown;
        }

        // ゲームプレイを無効化
        public void DisableGameplay()
        {
            gameStarted = false;
            keyBubble.SetActive(false);
            isBubbleActive = false;
        }

        // クールダウンを短縮する処理
        void ReduceCooldowns()
        {
            skill1Cooldown = Mathf.Max(minCooldown, skill1Cooldown * 0.75f);
            skill2Cooldown = Mathf.Max(minCooldown, skill2Cooldown * 0.75f);
            skill3Cooldown = Mathf.Max(minCooldown, skill3Cooldown * 0.75f);
        }
    }
}

