using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAttack : MonoBehaviour
{
    public Animator animator;
    public float animationtime;
    public Transform center;
    public GameObject meteorPrefab;
    public float circleRadius = 10f;
    public int meteorCount = 8;
    public float meteorLifetime = 5f;
    public float spawnInterval = 0.5f;
    public float raycastDistance = 50f;
    public LayerMask groundLayer;
    public float fixedSpawnHeight = 10f;

    private Queue<GameObject> meteorPool;
    private int poolSize = 20;

    void OnEnable()
    {
        if (meteorPool == null || meteorPool.Count == 0)
        {
            InitializePool();
        }

        StartCoroutine(SpawnMeteors());

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            animator.SetTrigger("attack_long");
            Debug.Log("Playing attack animation");
            StartCoroutine(ResetTriggerAfterDelay("attack_long", animationtime));
        }
        else
        {
            Debug.LogError("Animator component is missing!");
        }
    }

    private void InitializePool()
    {
        meteorPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject meteor = Instantiate(meteorPrefab);
            meteor.SetActive(false);
            meteorPool.Enqueue(meteor);
        }
    }

    private IEnumerator SpawnMeteors()
    {
        for (int i = 0; i < meteorCount; i++)
        {
            GameObject meteor = GetPooledMeteor();

            if (meteor != null)
            {
                Vector3 randomPosition = GetRandomPositionInCircle(center.position, circleRadius, fixedSpawnHeight);
                Vector3 groundPosition = AdjustToGround(randomPosition);

                meteor.transform.position = groundPosition;
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

        Debug.LogWarning("Meteor pool is empty! Increase pool size.");
        return null;
    }

    private IEnumerator DeactivateMeteor(GameObject meteor, float delay)
    {
        yield return new WaitForSeconds(delay);
        meteor.SetActive(false);
        meteorPool.Enqueue(meteor);
    }

    private Vector3 GetRandomPositionInCircle(Vector3 centerPosition, float radius, float height)
    {
        Vector2 randomPoint = Random.insideUnitCircle * radius;
        return new Vector3(centerPosition.x + randomPoint.x, height, centerPosition.z + randomPoint.y);
    }

    private Vector3 AdjustToGround(Vector3 spawnPosition)
    {
        if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            return hit.point;
        }

        Debug.LogWarning("No ground detected below spawn position!");
        return new Vector3(spawnPosition.x, 0, spawnPosition.z);
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
    private void OnDisable()
    {
        // Reset the animation trigger
        if (animator != null)
        {
            animator.ResetTrigger("attack_long");
        }

        // Stop all coroutines related to the meteor attack
        StopAllCoroutines();

        // Ensure all meteors are deactivated
        foreach (GameObject meteor in FindObjectsOfType<GameObject>())
        {
            if (meteor.activeSelf && meteor.CompareTag("Meteor")) // Check for active meteors by tag
            {
                meteor.SetActive(false);
            }
        }

        Debug.Log("All active meteors disabled, and animation reset.");
    }



}
