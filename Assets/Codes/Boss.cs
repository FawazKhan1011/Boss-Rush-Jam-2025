using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public PlayerMovement player;
    public GameObject bodycollider;
    public GameObject aura;
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
    public float initialDelay = 15f; // Delay before the attack cycle starts

    private bool isAttacking = false;
    public GameObject blackscreen;
    public string soundname;

    void Start()
    {
        FindAnyObjectByType<AudioManager>().Play("bossmusic");
        FindAnyObjectByType<AudioManager>().Play("impact");
        FindAnyObjectByType<AudioManager>().Play("rock");
        FindAnyObjectByType<AudioManager>().Play("startboss");
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

        // Start the attack cycle after the initial delay
        StartCoroutine(StartAttackCycleWithDelay());
    }

    private IEnumerator StartAttackCycleWithDelay()
    {
        Debug.Log($"Waiting for initial delay: {initialDelay} seconds.");
        yield return new WaitForSeconds(initialDelay);

        Debug.Log("Initial delay complete. Starting attack cycle.");
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
           // FindAnyObjectByType<AudioManager>().Play("hurt");
            int randomSound = Random.Range(1, 4); // Generates a random number between 1 and 3
            switch (randomSound)
            {
                case 1:
                    FindAnyObjectByType<AudioManager>().Play("hit");
                    break;
                case 2:
                    FindAnyObjectByType<AudioManager>().Play("mhurt2");
                    break;
                case 3:
                    FindAnyObjectByType<AudioManager>().Play("mhurt3");
                    break;
            }
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
        FindAnyObjectByType<AudioManager>().Stop("bossmusic");
        bodycollider.SetActive(false);
        aura.SetActive(false);
        DeactivateAllAttacks();

        // Play the "Die" animation
        if (animator != null)
        {
            animator.SetTrigger("dead");
            FindAnyObjectByType<AudioManager>().Play("bossdead");
            StartCoroutine(deadsound());
        }

        Debug.Log("Boss has been defeated!");

        // Wait for a few seconds after the death animation
        StartCoroutine(HandlePostDeathSequence(0.4f)); // Pass the speed and the wait time
        StartCoroutine(BlackScreen());
        StartCoroutine(DelayedSceneChange(15f, "Win"));
    }

    private IEnumerator deadsound()
    {
        yield return new WaitForSeconds(5f);
        FindAnyObjectByType<AudioManager>().Play("fall");
        Debug.Log("Fall sound");

    }
    public IEnumerator DelayedSceneChange(float delay, string scene)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene); // Change to "death" scene
    }

    public IEnumerator BlackScreen()
    {
        yield return new WaitForSeconds(12f);
        Image img = blackscreen.GetComponent<Image>(); // Get the Image component
        if (img == null) yield break;

        blackscreen.SetActive(true); // Ensure it's active before fading
        Color color = img.color;
        color.a = 0f;  // Start fully transparent
        img.color = color;

        float duration = 0.8f; // Fade duration
        float elapsed = 0f; // Start elapsed time at 0

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration); // Gradually increase alpha
            img.color = color;
            yield return null;
        }

        color.a = 1f;  // Ensure it is fully visible at the end
        img.color = color;
    }

    private IEnumerator HandlePostDeathSequence(float speed)
    {
        // Wait for the death animation to play for a given time
        yield return new WaitForSeconds(10f);  // Adjust this based on your animation length

        // Smoothly shift the boss downwards in Y position
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, startPos.y - 10f, startPos.z); // Adjust the downward distance

        float elapsedTime = 0f;

        // Disable the animator to avoid it overriding position
        if (animator != null)
        {
            animator.enabled = false;
        }

        while (elapsedTime < 1f)  // Duration based on speed
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime * speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Deadboss());
    }

    private IEnumerator Deadboss()
    {
        yield return new WaitForSeconds(4f);
        // Disable the boss GameObject
        gameObject.SetActive(false);
    }


    private IEnumerator AttackCycle()
    {
        int lastAttackChoice = -1; // Store the last attack choice

        while (currentHealth > 0) // Continue attacking while the boss is alive
        {
            if (!isAttacking)
            {
                isAttacking = true;

                int attackChoice;
                do
                {
                    attackChoice = Random.Range(1, 4); // Generate a new attack choice
                } while (attackChoice == lastAttackChoice); // Ensure it's not the same as the last one

                lastAttackChoice = attackChoice; // Update the last attack choice
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
        FindAnyObjectByType<AudioManager>().Play("scream");
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
