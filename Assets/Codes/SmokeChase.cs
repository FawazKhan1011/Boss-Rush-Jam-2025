using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SmokeChase : MonoBehaviour
{
    public GameObject chaseCollider;
    public GameObject bodyCollider;
    public Animator animator;
    public GameObject eye;
    public GameObject fog; // The particle object
    public GameObject aura;
    public Transform player;
    public NavMeshAgent agent;
    public Transform teleportPosition; // Assigned teleport position
    public float transitionTime = 2f;  // Time for the particle to fade out
    public float chaseDuration = 15f;  // Duration before teleporting and disabling particles
    public GameObject Boss;

    private bool isChasing = true;

    void Start()
    {
        // Initialize states
        bodyCollider.SetActive(false);
        chaseCollider.SetActive(true);
        animator.SetTrigger("move_forward_fast");
        eye.SetActive(true);
        fog.SetActive(true);
        aura.SetActive(false);

        // Start the 15-second timer
        StartCoroutine(ChaseTimer());
    }

    void Update()
    {
        // If chasing, set the boss's destination to the player's position
        if (isChasing)
        {
            agent.destination = player.position;
        }
    }

    // Timer to stop the chase after a specific duration
    private IEnumerator ChaseTimer()
    {
        yield return new WaitForSeconds(chaseDuration);

        // Stop chasing and teleport the boss to the assigned position
        isChasing = false;
        agent.ResetPath(); // Stop NavMeshAgent's movement
        animator.ResetTrigger("move_forward_fast");
        eye.SetActive(false);
        bodyCollider.SetActive(true);
        chaseCollider.SetActive(false);
        aura.SetActive(true);

        if (teleportPosition != null)
        {
            Debug.Log("Teleporting Boss to: " + teleportPosition.position); // Debug log

            // Temporarily disable NavMeshAgent to avoid interference
            agent.enabled = false;

            // Set position
            Boss.transform.position = teleportPosition.position;
            Boss.transform.rotation = teleportPosition.rotation;

            // Set rotation explicitly (adjusting only the y-axis)
            Vector3 newRotation = teleportPosition.eulerAngles;
            newRotation.y = Mathf.Repeat(newRotation.y, 360); // Normalize to [0, 360]
            Boss.transform.rotation = Quaternion.Euler(newRotation);

            // Re-enable NavMeshAgent
            agent.enabled = true;
        }
        else
        {
            Debug.LogWarning("Teleport Position is not assigned!");
        }

        // Smoothly disable the fog particle
        StartCoroutine(DisableFogSmoothly());
    }

    // Coroutine to smoothly disable the fog particle system
    private IEnumerator DisableFogSmoothly()
    {
        if (fog.TryGetComponent<ParticleSystem>(out var particleSystem))
        {
            // Get the main module of the particle system
            var main = particleSystem.main;

            // Gradually reduce the particle size over time
            float startTime = Time.time;
            while (Time.time < startTime + transitionTime)
            {
                float t = (Time.time - startTime) / transitionTime;
                main.startSize = Mathf.Lerp(main.startSize.constant, 0, t);
                yield return null;
            }

            // Stop the particle system and disable the fog object
            particleSystem.Stop();
        }
        else
        {
            // If not a particle system, disable directly
            fog.SetActive(false);
        }
    }
}
