using System.Collections;
using UnityEngine;

public class Starter : MonoBehaviour
{
    public GameObject boss; // Boss object to be activated
    public Transform startPoint; // The teleport starting position
    public Transform finishPoint; // The final destination position
    public float moveSpeed = 6f; // Speed of movement
    public float hoverHeight = 2f; // How high the boss should rise before landing
    public float hoverDuration = 0.5f; // Duration of the hover effect
    public GameObject aura;
    public GameObject self;
    public GameObject bosshealth;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player triggers the boss activation
        if (other.CompareTag("Player"))
        {

            // Teleport the boss to the start point
            boss.transform.position = startPoint.position;
            boss.SetActive(true);

            // Start moving the boss smoothly to the finish point with hover effect
            StartCoroutine(MoveBossWithHoverEffect());
        }
    }

    private IEnumerator MoveBossWithHoverEffect()
    {
        bosshealth.SetActive(true);
        float elapsedTime = 0f;
        float duration = Vector3.Distance(startPoint.position, finishPoint.position) / moveSpeed;

        // Move boss to the end position
        while (elapsedTime < duration)
        {
            boss.transform.position = Vector3.Lerp(startPoint.position, finishPoint.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure boss reaches exact finish position before hovering
        boss.transform.position = finishPoint.position;

        // Hover effect: Move boss up slightly
        Vector3 hoverPos = finishPoint.position + Vector3.up * hoverHeight;
        elapsedTime = 0f;
        while (elapsedTime < hoverDuration)
        {
            boss.transform.position = Vector3.Lerp(finishPoint.position, hoverPos, elapsedTime / hoverDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Landing effect: Smoothly move back down to the final position
        elapsedTime = 0f;
        while (elapsedTime < hoverDuration)
        {
            boss.transform.position = Vector3.Lerp(hoverPos, finishPoint.position, elapsedTime / hoverDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final exact position
        boss.transform.position = finishPoint.position;
        aura.SetActive(true);
        self.SetActive(false);   
    }
}
