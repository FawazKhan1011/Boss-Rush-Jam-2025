using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Required for working with UI elements

public class Death : MonoBehaviour
{
    public GameObject imageObject; // Assign the image GameObject in the Inspector
    public GameObject secondObject; // Assign the second GameObject in the Inspector

    private Image imageComponent;
    private float fadeDuration = 2.0f; // Duration of the fade-in effect

    public string musicnamestart;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        EventSystem.current.SetSelectedGameObject(null);
        FindAnyObjectByType<AudioManager>().Play(musicnamestart);
        Cursor.visible = true;
        if (imageObject != null)
        {
            imageComponent = imageObject.GetComponent<Image>();
            if (imageComponent != null)
            {
                // Start the coroutine to fade in the image
                StartCoroutine(FadeInImage());
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

    IEnumerator FadeInImage()
    {
        // Ensure the image is initially hidden
        Color color = imageComponent.color;
        color.a = 0f;
        imageComponent.color = color;

        // Gradually increase the alpha value to make the image visible
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

        // Activate the second object
        if (secondObject != null)
        {
            secondObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No secondObject assigned in the Inspector.");
        }
    }
}