using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public Image imageComponent;
    public GameObject imageObject;
    private float fadeDuration = 2.0f;
    public GameObject gameboss;

    private void Start()
    {
        FindAnyObjectByType<AudioManager>().Play("main");
    }
    IEnumerator FadeInOutImage()
    {
        imageObject.SetActive(true);

        // Ensure the image is initially hidden
        Color color = imageComponent.color;
        color.a = 0f;
        imageComponent.color = color;

        // Gradually increase the alpha value to fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            imageComponent.color = color;
            yield return null;
        }

        // Ensure the image is fully visible
        color.a = 1f;
        imageComponent.color = color;

        // Wait for a moment before fading out (optional)
        yield return new WaitForSeconds(1f);

        // Gradually decrease the alpha value to fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            imageComponent.color = color;
            yield return null;
        }

        // Ensure the image is fully invisible before deactivating
        color.a = 0f;
        imageComponent.color = color;
        imageObject.SetActive(false);
        gameboss.SetActive(true);
        self.SetActive(false);
    }
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
            if (imageObject != null)
            {
                imageComponent = imageObject.GetComponent<Image>();
                if (imageComponent != null)
                {
                    // Start the coroutine to fade in the image
                    StartCoroutine(FadeInOutImage());
                }
                else
                {
                    Debug.LogError("The assigned imageObject does not have an Image component.");
                }
            }
            else
            {
                Debug.LogError("No imageObject assigned in the Inspector.");
            }
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
    }
    private void OnDisable()
    {
        FindAnyObjectByType<AudioManager>().Stop("main");
    }
}
