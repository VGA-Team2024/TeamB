using UnityEngine;
using UnityEngine.UI;

namespace TeamB.develop_alpha
{

    public class GameManager : MonoBehaviour
    {
        public PlayerController playerController; // プレイヤーコントローラーの参照
        public GameObject enemyPrefab; // 新しい敵のプレハブ
        public Button startButton; // スタートボタン
        public Text timerText; // タイマー表示用のテキスト
        public Slider playerHealthSlider; // プレイヤーHP表示用スライダー
        public Slider enemyHealthSlider; // エネミーHP表示用スライダー
        public Text waveText; // Wave表示用のテキスト
        public Text scoreText; // ポイント表示用のテキスト

        private float timer = 45.0f; // タイマーの初期値
        private bool gameStarted = false; // ゲーム開始状態のフラグ
        private int currentWave = 0; // 現在のWave
        private Enemy currentEnemy; // 現在の敵の参照
        private int playerScore = 0; // プレイヤーのポイント

        void Start()
        {
            // スタートボタンのリスナーを設定
            startButton.onClick.AddListener(StartGame);
            DisableGameplay();

            playerHealthSlider.maxValue = playerController.maxHealth; // プレイヤーのHPに基づく
            UpdateScoreUI(); // 初期のスコアUIを更新
        }

        void Update()
        {
            if (gameStarted)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime; // タイマーを減少
                    UpdateTimerUI(); // UIの更新
                    UpdatePlayerHealthUI(); // プレイヤーHPのUIを更新
                    UpdateEnemyHealthUI(); // エネミーHPのUIを更新
                }
                else
                {
                    EndGame(); // タイマーが0になったらゲーム終了処理
                }
            }
        }

        void StartGame()
        {
            gameStarted = true; // ゲーム開始
            timer = 45.0f; // タイマーをリセット
            currentWave = 1; // Waveを1に初期化
            playerController.StartGame(); // プレイヤーの操作を有効にする
            startButton.gameObject.SetActive(false); // スタートボタンを非表示
            SpawnNewEnemy(); // 新しい敵を生成
            UpdateWaveUI(); // Wave表示を更新
        }

        void SpawnNewEnemy()
        {
            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject); // 既存の敵を破棄
            }

            currentEnemy = Instantiate(enemyPrefab).GetComponent<Enemy>(); // 新しい敵を生成
            currentEnemy.SetEnemyHealth(100 + (currentWave - 1) * 50); // Waveごとに敵のHPを設定
            enemyHealthSlider.maxValue = currentEnemy.maxHealth; // HPバーの最大値を更新
            enemyHealthSlider.value = currentEnemy.currentEnemyHealth; // HPバーの現在値を設定

            // プレイヤーに新しい敵の参照を渡す
            playerController.SetEnemyTarget(currentEnemy);
        }

        void UpdateTimerUI()
        {
            timerText.text = "Time: " + timer.ToString("F2"); // 小数点以下2桁で表示
        }

        void UpdatePlayerHealthUI()
        {
            playerHealthSlider.value = playerController.currentHealth; // スライダーの値をプレイヤーのHPに設定
        }

        public void UpdateEnemyHealthUI()
        {
            if (currentEnemy != null)
            {
                enemyHealthSlider.value = currentEnemy.currentEnemyHealth; // スライダーの値をエネミーのHPに設定
            }
        }

        void UpdateWaveUI()
        {
            waveText.text = "Wave: " + currentWave; // WaveのUIを更新
        }

        public void EnemyDefeated()
        {
            currentWave++; // Waveを上げる
            SpawnNewEnemy(); // 新しい敵を生成
            UpdateWaveUI(); // Wave表示を更新
        }

        public void AddPoints(int points)
        {
            playerScore += points; // ポイントを追加
            UpdateScoreUI(); // UIを更新
        }

        void UpdateScoreUI()
        {
            scoreText.text = "Score: " + playerScore; // スコアの表示を更新
        }

        public void EndGame()
        {
            gameStarted = false; // ゲーム終了
            playerController.DisableGameplay(); // プレイヤーの操作を無効にする
            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject); // 現在の敵を破棄
            }
            Debug.Log("Game Over!");
            // ゲームオーバーの処理を追加
        }

        void DisableGameplay()
        {
            playerController.DisableGameplay();
            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject); // 既存の敵を破棄
            }
        }
    }

}