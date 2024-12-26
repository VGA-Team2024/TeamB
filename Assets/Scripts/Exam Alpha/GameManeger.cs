using UnityEngine;
using UnityEngine.UI;

namespace TeamB.develop_alpha
{

    public class GameManager : MonoBehaviour
    {
        public PlayerController playerController;
        public GameObject enemyPrefab; 
        public Button startButton; 
        public Text timerText; 
        public Slider playerHealthSlider; 
        public Slider enemyHealthSlider; 
        public Text waveText;
        public Text scoreText; 

        private float timer = 45.0f; 
        private bool gameStarted = false; 
        private int currentWave = 0; 
        private Enemy currentEnemy; 
        private int playerScore = 0; 

        void Start()
        {
            startButton.onClick.AddListener(StartGame);
            DisableGameplay();

            playerHealthSlider.maxValue = playerController.maxHealth; 
            UpdateScoreUI();
        }

        void Update()
        {
            if (gameStarted)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    UpdateTimerUI();
                    UpdatePlayerHealthUI(); 
                    UpdateEnemyHealthUI(); 
                }
                else
                {
                    EndGame();
                }
            }
        }

        void StartGame()
        {
            gameStarted = true;
            timer = 45.0f; 
            currentWave = 1; 
            playerController.StartGame(); 
            startButton.gameObject.SetActive(false); 
            SpawnNewEnemy(); 
            UpdateWaveUI();
        }

        void SpawnNewEnemy()
        {
            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject); 
            }

            currentEnemy = Instantiate(enemyPrefab).GetComponent<Enemy>(); 
            currentEnemy.SetEnemyHealth(100 + (currentWave - 1) * 50); 
            enemyHealthSlider.maxValue = currentEnemy.maxHealth;
            enemyHealthSlider.value = currentEnemy.currentEnemyHealth;

            playerController.SetEnemyTarget(currentEnemy);
        }

        void UpdateTimerUI()
        {
            timerText.text = "Time: " + timer.ToString("F2"); 
        }

        void UpdatePlayerHealthUI()
        {
            playerHealthSlider.value = playerController.currentHealth; 
        }

        public void UpdateEnemyHealthUI()
        {
            if (currentEnemy != null)
            {
                enemyHealthSlider.value = currentEnemy.currentEnemyHealth;
            }
        }

        void UpdateWaveUI()
        {
            waveText.text = "Wave: " + currentWave;
        }

        public void EnemyDefeated()
        {
            currentWave++; 
            SpawnNewEnemy(); 
            UpdateWaveUI(); 
        }

        public void AddPoints(int points)
        {
            playerScore += points; 
            UpdateScoreUI(); 
        }

        void UpdateScoreUI()
        {
            scoreText.text = "Score: " + playerScore; 
        }

        public void EndGame()
        {
            gameStarted = false;
            playerController.DisableGameplay();
            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject); 
            }
            Debug.Log("Game Over!");
        }

        void DisableGameplay()
        {
            playerController.DisableGameplay();
            if (currentEnemy != null)
            {
                Destroy(currentEnemy.gameObject);
            }
        }
    }

}
