using System.Collections;
using UnityEngine;

public class BossAttack3Collision : MonoBehaviour
{
    public Animator animator;
    public PlayerMovement playerref;

    public float damageCooldown = 5f; // Cooldown time between damage instances
    private bool canDamage = true;    // Controls whether damage can be dealt

    private void OnTriggerEnter(Collider other)
    {
        // Check if the boss collides with the player and is allowed to deal damage
        if (other.CompareTag("Player") && canDamage)
        {
            Debug.Log("Player hit by the boss! Dealing damage...");

            // Trigger the attack animation
            if (animator != null)
            {
                animator.SetTrigger("attack_short_001");
                StartCoroutine(ResetTriggerAfterDelay("attack_short_001", 1.0f));
            }

            // Deal damage to the player
            if (playerref != null)
            {
                StartCoroutine(delaydamage(1.2f));
            }

            // Start the cooldown timer
            StartCoroutine(DamageCooldown());
        }
    }

    private IEnumerator delaydamage(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerref.doDamage(1);

    }
    private IEnumerator ResetTriggerAfterDelay(string triggerName, float delay)
    {
        // Wait for the specified delay before resetting the trigger
        yield return new WaitForSeconds(delay);
        if (animator != null)
        {
            animator.ResetTrigger(triggerName);
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false; // Disable damage
        yield return new WaitForSeconds(damageCooldown); // Wait for the cooldown duration
        canDamage = true;  // Re-enable damage
    }
}
