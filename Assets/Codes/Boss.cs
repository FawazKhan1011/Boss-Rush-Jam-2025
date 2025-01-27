using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int maxHealth = 10; // Maximum health of the boss
    public int currentHealth; // Current health of the boss
    public BossHealth health; // Reference to the health UI system
    public Animator animator; // Reference to the Animator component

    [Header("Boss Attacks")]
    public GameObject attack1; // Reference to Attack 1 GameObject
    public GameObject attack2; // Reference to Attack 2 GameObject
    public GameObject attack3; // Reference to Attack 3 GameObject

    [Header("Attack Durations (Seconds)")]
    public float attack1Duration = 3f; // Duration for Attack 1
    public float attack2Duration = 4f; // Duration for Attack 2
    public float attack3Duration = 5f; // Duration for Attack 3

    [Header("Attack Timers")]
    public float afterAttackDelay = 2f; // Delay before generating another attack

    private bool isAttacking = false;

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

        // Start the attack cycle
        StartCoroutine(AttackCycle());
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
        DeactivateAllAttacks();
        // Play the "Die" animation
        if (animator != null)
        {
            animator.SetTrigger("dead");
        }
        Debug.Log("Boss has been defeated!");
    }

    private IEnumerator AttackCycle()
    {
        while (currentHealth > 0) // Continue attacking while the boss is alive
        {
            if (!isAttacking)
            {
                isAttacking = true;

                // Generate a random attack
                int attackChoice = Random.Range(1, 4);
                Debug.Log($"Generated attack: {attackChoice}");

                // Activate the chosen attack
                float attackDuration = GetAttackDuration(attackChoice);
                yield return StartCoroutine(ActivateAttack(attackChoice, attackDuration));
                // Wait for the after-attack delay before generating the next attack
                yield return new WaitForSeconds(afterAttackDelay);

                isAttacking = false;
            }

            yield return null;
        }
    }

    private IEnumerator ActivateAttack(int attackChoice, float duration)
    {
        GameObject attackObject = GetAttackObject(attackChoice);

        if (attackObject != null)
        {
            attackObject.SetActive(true);
            Debug.Log($"{attackObject.name} activated for {duration} seconds.");
            yield return new WaitForSeconds(duration);
            attackObject.SetActive(false);
            Debug.Log($"{attackObject.name} deactivated.");
        }
    }

    private void DeactivateAllAttacks()
    {
        if (attack1 != null) attack1.SetActive(false);
        if (attack2 != null) attack2.SetActive(false);
        if (attack3 != null) attack3.SetActive(false);
        Debug.Log("All attacks deactivated.");
    }

    private GameObject GetAttackObject(int attackChoice)
    {
        return attackChoice switch
        {
            1 => attack1,
            2 => attack2,
            3 => attack3,
            _ => null,
        };
    }

    private float GetAttackDuration(int attackChoice)
    {
        return attackChoice switch
        {
            1 => attack1Duration,
            2 => attack2Duration,
            3 => attack3Duration,
            _ => 0f,
        };
    }
}
