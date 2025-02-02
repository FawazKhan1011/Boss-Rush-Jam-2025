using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SmokeChase : MonoBehaviour
{
    public GameObject chaseCollider;
    public GameObject bodyCollider;
    public Animator animator;
    public GameObject eye;
    public GameObject fog; // Reference to the fog GameObject holding the particle system
    public GameObject aura;
    public Transform player;
    public NavMeshAgent agent;
    public Transform teleportPosition; // Teleport location for the boss
    public float transitionTime = 2f;  // Time to fade out the fog
    public float chaseDuration = 15f;  // Duration of the chase state
    public GameObject Boss;

    public GameObject warningObject; // Assign the GameObject to shake in the Inspector
    public float shakeIntensity = 1f; // Intensity of the shake
    public float shakeDuration = 3f; // Duration of the shake effect

    private Vector3 originalPosition; // Store the original position of the GameObject

    private bool isChasing = true;

    void OnEnable()
    {
        FindAnyObjectByType<AudioManager>().Play("smoke");
        FindAnyObjectByType<AudioManager>().Play("heart");
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned to SmokeChase.");
            return;
        }

        isChasing = true;

        // Temporarily disable NavMeshAgent
        agent.enabled = false;

        bodyCollider.SetActive(false);
        chaseCollider.SetActive(true);
        animator.SetTrigger("move_forward_fast");
        eye.SetActive(true);
        aura.SetActive(false);
        fog.SetActive(true);
        // Start the NavMeshAgent after 1 second (adjust if needed)
        StartCoroutine(EnableNavMeshAgentWithDelay(1f));

        StartCoroutine(ChaseTimer());
        StartCoroutine(warningflag());
    }

    private IEnumerator warningflag()
    {
        warningObject.SetActive(true);
        originalPosition = warningObject.transform.localPosition; // Store the original position

        float elapsedTime = 0f;

        // Shake the GameObject for the specified duration
        while (elapsedTime < shakeDuration)
        {
            // Calculate a random offset for the shake effect
            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity)
            );

            // Apply the random offset to the GameObject's position
            warningObject.transform.localPosition = originalPosition + randomOffset;

            // Wait for the next frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the GameObject's position to its original position
        warningObject.transform.localPosition = originalPosition;

        // Disable the GameObject after the shake effect
        warningObject.SetActive(false);
    }

    private IEnumerator EnableNavMeshAgentWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        agent.enabled = true;

        // If the agent is still not on the NavMesh, manually reposition it
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning("Boss is NOT properly on the NavMesh! Repositioning...");

            // Find the closest valid NavMesh position
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                Debug.Log("Boss repositioned to nearest NavMesh point.");
            }
            else
            {
                Debug.LogError("Failed to reposition boss on NavMesh.");
            }
        }
    }



    void Update()
    {
        if (isChasing && agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }


    private IEnumerator ChaseTimer()
    {
        yield return new WaitForSeconds(chaseDuration);

        isChasing = false;
        agent.ResetPath();
        animator.ResetTrigger("move_forward_fast");
        eye.SetActive(false);
        bodyCollider.SetActive(true);
        chaseCollider.SetActive(false);
        aura.SetActive(true);

        if (teleportPosition != null)
        {
            agent.enabled = false;
            Boss.transform.position = teleportPosition.position;
            Boss.transform.rotation = teleportPosition.rotation;
            agent.enabled = true;
        }
        else
        {
            Debug.LogWarning("Teleport Position is not assigned!");
        }
        fog.SetActive(false);

    }
    private void OnDisable()
    {
        FindAnyObjectByType<AudioManager>().Stop("heart");
        fog.SetActive(false);
        isChasing = false;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.ResetPath();  // Only reset if agent is active & on NavMesh
        }
        else
        {
            Debug.LogWarning("Cannot reset path: NavMeshAgent is disabled or not on a NavMesh.");
        }

        animator.ResetTrigger("move_forward_fast");
        eye.SetActive(false);
        bodyCollider.SetActive(true);
        chaseCollider.SetActive(false);
    }

}