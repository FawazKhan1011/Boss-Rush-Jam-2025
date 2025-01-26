using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack3Collision : MonoBehaviour
{

    public Animator animator;
    public PlayerMovement playerref;

    private bool hasDamaged = false;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the boss collides with the player
        if (other.CompareTag("Player") && !hasDamaged)
        {
            Debug.Log("Player hit by the boss! Dealing damage...");
            animator.SetTrigger("attack_short_001");
            StartCoroutine(ResetTriggerAfterDelay("attack_short_001", 1.0f));
            playerref.doDamage(1);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to the player and reset the flag when the player exits
        if (other.CompareTag("Player"))
        {
            hasDamaged = false;  // Reset the flag so the laser can deal damage again when the player re-enter
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
}
