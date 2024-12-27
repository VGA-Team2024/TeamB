using UnityEngine;
using UnityEngine.UI;

namespace TeamB.develop_alpha
{
    public class Enemy : MonoBehaviour
    {
        public int maxHealth = 100; 
        public int currentEnemyHealth; 
        public float attackInterval = 2.0f;
        public int attackDamageMin = 5; 
        public int attackDamageMax = 15; 

        private float lastAttackTime = 0.0f;
        private PlayerController player; 
        private GameManager gameManager;
        private bool isStunned = false;
        private float stunEndTime = 0f;

        void Start()
        {
            currentEnemyHealth = maxHealth; 
            player = FindObjectOfType<PlayerController>(); 
            gameManager = FindObjectOfType<GameManager>(); 
        }

        void Update()
        {
            if (isStunned && Time.time >= stunEndTime)
            {
                isStunned = false;
            }

            if (!isStunned && Time.time >= lastAttackTime + attackInterval)
            {
                AttackPlayer(); 
                lastAttackTime = Time.time;
            }
        }

        public void TakeDamage(int damage)
        {
            currentEnemyHealth -= damage; 
            Debug.Log($"Enemy takes {damage} damage. Health: {currentEnemyHealth}");

            if (currentEnemyHealth <= 0)
            {
                Debug.Log("Enemy defeated!");
                gameManager.EnemyDefeated(); 
                Destroy(gameObject); 
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
                int damage = Random.Range(attackDamageMin, attackDamageMax + 1); 
                player.TakeDamage(damage); 
                Debug.Log($"Enemy attacks player for {damage} damage.");
            }
        }

        public void SetEnemyHealth(int health)
        {
            maxHealth = health;
            currentEnemyHealth = maxHealth;
        }

        public void DisableGameplay()
        {
            isStunned = true;
        }

        public bool IsDefeated() 
        {
            return currentEnemyHealth <= 0; 
        }

        public void Stun(float duration)
        {
            isStunned = true;
            stunEndTime = Time.time + duration;
            Debug.Log($"Enemy stunned for {duration} seconds.");
        }
    }
}
