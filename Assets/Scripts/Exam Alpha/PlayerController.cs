using UnityEngine;
using UnityEngine.UI;

namespace TeamB.develop_alpha
{

    public class PlayerController : MonoBehaviour
    {
        public int maxHealth = 100;
        public int currentHealth; 

        public GameObject keyBubble; 
        public Text keyText; 
        public Image skill1IconFill; 
        public Image skill2IconFill;
        public Image skill3IconFill; 
        public Slider healthSlider;

        public float skill1Cooldown = 7.0f;
        public float skill2Cooldown = 12.0f; 
        public float skill3Cooldown = 20.0f; 

        private char currentKey; 
        private bool isBubbleActive = false; 
        private float skill1LastUsedTime; 
        private float skill2LastUsedTime; 
        private float skill3LastUsedTime; 

        private int successfulKeyPresses = 0;
        private const float minCooldown = 1.0f;
        private Enemy enemyTarget; 
        private bool gameStarted = false; 
        private GameManager gameManager; 

        void Start()
        {
            currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth; 
            UpdateHealthSlider(); 
            GenerateRandomKey(); 
            enemyTarget = FindObjectOfType<Enemy>();
            gameManager = FindObjectOfType<GameManager>();
        }

        void Update()
        {
            if (gameStarted)
            {
                UpdateSkillCooldownUI(); 

                if (isBubbleActive && Input.GetKeyDown(currentKey.ToString().ToLower()))
                {
                    Debug.Log("Correct key pressed: " + currentKey);
                    successfulKeyPresses++;
                    gameManager.AddPoints(50); 
                    GenerateRandomKey(); 

                    if (successfulKeyPresses >= 3)
                    {
                        ReduceCooldowns();
                        successfulKeyPresses = 0;
                    }
                }

                if (Input.GetKeyDown(KeyCode.I) && CanUseSkill1())
                {
                    UseSkill1();
                    gameManager.AddPoints(100);
                    successfulKeyPresses = 0;
                }

                if (Input.GetKeyDown(KeyCode.O) && CanUseSkill2())
                {
                    UseSkill2();
                    gameManager.AddPoints(150); 
                    successfulKeyPresses = 0;
                }

                if (Input.GetKeyDown(KeyCode.P) && CanUseSkill3())
                {
                    UseSkill3();
                    gameManager.AddPoints(200); 
                    successfulKeyPresses = 0;
                }
            }
        }

        public void StartGame()
        {
            gameStarted = true; 
            currentHealth = maxHealth; 
            skill1LastUsedTime = Time.time; 
            skill2LastUsedTime = Time.time;
            skill3LastUsedTime = Time.time;
            GenerateRandomKey(); 
            UpdateHealthSlider(); 
        }

        void GenerateRandomKey()
        {
            char[] keys = { 'W', 'A', 'S', 'D' };
            currentKey = keys[Random.Range(0, keys.Length)];
            keyText.text = currentKey.ToString();
            keyBubble.SetActive(true);
            isBubbleActive = true;
        }

        public void SetEnemyTarget(Enemy newEnemy)
        {
            enemyTarget = newEnemy;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            UpdateHealthSlider(); 
            Debug.Log($"Player takes {damage} damage. Health: {currentHealth}");

            if (currentHealth <= 0)
            {
                Debug.Log("Player is defeated!");
                gameManager.EndGame();
            }
        }

        void UseSkill1()
        {
            Debug.Log("Skill 1 activated! Dealing 20 damage to the enemy.");
            skill1LastUsedTime = Time.time; 
            skill1Cooldown = 7.0f; 
            
            if (enemyTarget != null)
            {
                enemyTarget.TakeDamage(20); 
                if (enemyTarget.IsDefeated())
                {
                    gameManager.EnemyDefeated();
                    gameManager.AddPoints(300); 
                }
            }
        }

        void UseSkill2()
        {
            Debug.Log("Skill 2 activated! Healing player for 15 health.");
            currentHealth += 15;
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            skill2LastUsedTime = Time.time; 
            skill2Cooldown = 12.0f; 
            UpdateHealthSlider();
        }

        void UseSkill3()
        {
            Debug.Log("Skill 3 activated! Stopping enemy's action for 5 seconds.");
            skill3LastUsedTime = Time.time; 
            skill3Cooldown = 15.0f; 
            if (enemyTarget != null)
            {
                enemyTarget.Stun(5.0f);
            }
        }

        void UpdateHealthSlider()
        {
            healthSlider.value = currentHealth;
        }

        void UpdateSkillCooldownUI()
        {
            skill1IconFill.fillAmount = (Time.time - skill1LastUsedTime) / skill1Cooldown;
            skill2IconFill.fillAmount = (Time.time - skill2LastUsedTime) / skill2Cooldown; 
            skill3IconFill.fillAmount = (Time.time - skill3LastUsedTime) / skill3Cooldown;
        }

        bool CanUseSkill1()
        {
            return (Time.time - skill1LastUsedTime) >= skill1Cooldown;
        }

        bool CanUseSkill2()
        {
            return (Time.time - skill2LastUsedTime) >= skill2Cooldown;
        }

        bool CanUseSkill3()
        {
            return (Time.time - skill3LastUsedTime) >= skill3Cooldown;
        }

        public void DisableGameplay()
        {
            gameStarted = false;
            keyBubble.SetActive(false);
            isBubbleActive = false;
        }

        void ReduceCooldowns()
        {
            skill1Cooldown = Mathf.Max(minCooldown, skill1Cooldown * 0.75f);
            skill2Cooldown = Mathf.Max(minCooldown, skill2Cooldown * 0.75f);
            skill3Cooldown = Mathf.Max(minCooldown, skill3Cooldown * 0.75f);
        }
    }
}

