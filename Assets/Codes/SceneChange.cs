using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public string sceneName; // Set this in the Inspector

    public GameObject objectToDisable;
    public GameObject objectToEnable;
    public float fadeDuration = 0.5f; // Duration of fade effect

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void SmoothToggleGameObjects()
    {
        StartCoroutine(FadeOutAndSwitch());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator FadeOutAndSwitch()
    {
        if (objectToDisable != null)
        {
            CanvasGroup canvasGroup = objectToDisable.GetComponent<CanvasGroup>();

            if (canvasGroup != null) // If it has a CanvasGroup, fade out
            {
                float timer = 0;
                while (timer < fadeDuration)
                {
                    timer += Time.deltaTime;
                    canvasGroup.alpha = 1 - (timer / fadeDuration);
                    yield return null;
                }
                canvasGroup.alpha = 0;
            }

            objectToDisable.SetActive(false);
        }

        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);

            CanvasGroup newCanvasGroup = objectToEnable.GetComponent<CanvasGroup>();
            if (newCanvasGroup != null)
            {
                newCanvasGroup.alpha = 0;
                float timer = 0;
                while (timer < fadeDuration)
                {
                    timer += Time.deltaTime;
                    newCanvasGroup.alpha = timer / fadeDuration;
                    yield return null;
                }
                newCanvasGroup.alpha = 1;
            }
        }
    }
}
