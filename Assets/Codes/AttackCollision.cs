using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    private bool hasDamaged = false;  // To track if the damage has already been dealt
    public PlayerMovement player;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player and if damage hasn't been dealt yet
        if (other.CompareTag("Player") && !hasDamaged)
        {
            player.doDamage(1);
            hasDamaged = true;  // Set flag to indicate damage has been dealt
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
}
