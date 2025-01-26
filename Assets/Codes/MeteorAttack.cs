using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAttack : MonoBehaviour
{
    public Animator animator;
    public float animationtime;
    public Transform center;            // The center of the attack circle
    public GameObject meteorPrefab;     // Prefab of the meteor to spawn
    public float circleRadius = 10f;    // Radius of the circle where meteors will spawn
    public int meteorCount = 8;         // Number of meteors to spawn
    public float meteorLifetime = 5f;  // Time (in seconds) before meteors are deactivated
    public float spawnInterval = 0.5f; // Time between spawning meteors
    public float raycastDistance = 50f; // Max distance for the raycast to check for ground
    public LayerMask groundLayer;       // Layer mask for detecting the ground
    public float fixedSpawnHeight = 10f; // Fixed height from which meteors are spawned before raycasting

    private Queue<GameObject> meteorPool; // Pool of meteors for reuse
    private int poolSize = 20;           // Size of the pool

    private void Start()
    {
        // Initialize the pool
        InitializePool();

        // Start spawning meteors
        StartCoroutine(SpawnMeteors());
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
            animator.SetTrigger("attack_long");
            Debug.Log("Attack 2");
            StartCoroutine(ResetTriggerAfterDelay("attack_long", animationtime));
        }
    }

    private void InitializePool()
    {
        meteorPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject meteor = Instantiate(meteorPrefab);
            meteor.SetActive(false); // Deactivate initially
            meteorPool.Enqueue(meteor);
        }
    }

    private IEnumerator SpawnMeteors()
    {
        for (int i = 0; i < meteorCount; i++)
        {
            // Get a meteor from the pool
            GameObject meteor = GetPooledMeteor();

            if (meteor != null)
            {
                // Get a random position within the circle, at the fixed spawn height
                Vector3 randomPosition = GetRandomPositionInCircle(center.position, circleRadius, fixedSpawnHeight);

                // Adjust position to align with the ground below
                Vector3 groundPosition = AdjustToGround(randomPosition);

                // Set meteor position
                meteor.transform.position = groundPosition;

                // Activate the meteor and schedule deactivation
                meteor.SetActive(true);
                StartCoroutine(DeactivateMeteor(meteor, meteorLifetime));
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private GameObject GetPooledMeteor()
    {
        if (meteorPool.Count > 0)
        {
            return meteorPool.Dequeue();
        }

        Debug.LogWarning("Meteor pool is empty! Consider increasing the pool size.");
        return null;
    }

    private IEnumerator DeactivateMeteor(GameObject meteor, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Deactivate and return the meteor to the pool
        meteor.SetActive(false);
        meteorPool.Enqueue(meteor);
    }

    private Vector3 GetRandomPositionInCircle(Vector3 centerPosition, float radius, float height)
    {
        // Generate a random point within a circle
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        return new Vector3(centerPosition.x + randomPoint.x, height, centerPosition.z + randomPoint.y);
    }

    private Vector3 AdjustToGround(Vector3 spawnPosition)
    {
        // Cast a ray downward to find the ground
        if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            // Adjust spawn position to the point where the ray hits the ground
            return hit.point;
        }

        Debug.LogWarning("No ground detected below the spawn position!");
        return new Vector3(spawnPosition.x, 0, spawnPosition.z); // Default to y = 0 if no ground is found
    }
    private IEnumerator ResetTriggerAfterDelay(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null)
        {
            animator.ResetTrigger(triggerName);
        }
    }
    private void OnDrawGizmos()
    {
        if (center != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center.position, circleRadius);
        }
    }
}
