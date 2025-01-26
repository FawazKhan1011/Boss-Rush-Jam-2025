using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int maxHealth = 10; // Maximum health of the boss
    public int currentHealth; // Current health of the boss
    public BossHealth health; // Reference to the health UI system
    public Animator animator; // Reference to the Animator component
    
    public RotatingLasers rotatingLasers;

    void Start()
    {
        currentHealth = maxHealth;
        health.SetMaxHealth(maxHealth);

        // Ensure the Animator component is assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component is missing! Assign it to the Boss.");
            }
        }
        if (animator != null)
        {
            animator.SetTrigger("idle_combat");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        health.SetHealth(currentHealth);

        // Play the "TakeDamage" animation
        if (animator != null)
        {
            animator.SetTrigger("damage_001");
            Debug.Log("Triggering: damage_001");
            StartCoroutine(ResetTriggerAfterDelay("damage_001", 1.0f));
        }

        // Check if the boss is defeated
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private IEnumerator ResetTriggerAfterDelay(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null)
        {
            animator.ResetTrigger(triggerName);
        }
    }
    void Die()
    {
        // Play the "Die" animation
        if (animator != null)
        {
            animator.SetTrigger("dead");
        }
        Debug.Log("Boss has been defeated!");
        
    }

    public void StartLaserAttack()
    {
        rotatingLasers.enabled = true; // Enable laser behavior
    }

    public void StopLaserAttack()
    {
        rotatingLasers.enabled = false; // Disable laser behavior
    }
}
