using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnim : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;
    public string[] stringArray;
    [SerializeField] float timeBtwnChars;
    [SerializeField] float timeBtwnWords;
    public GameObject textskip;
    private TextMeshProUGUI textskipTMP;

    int i = 0;

    void Start()
    {
        FindAnyObjectByType<AudioManager>().Play("type");
        if (textskip != null)
        {
            textskipTMP = textskip.GetComponent<TextMeshProUGUI>();
            StartCoroutine(BlinkText());
        }

        EndCheck();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FindAnyObjectByType<AudioManager>().Stop("type");
            FindAnyObjectByType<AudioManager>().Play("button");
            StartCoroutine(scenechange());    
        }
    }

    private IEnumerator scenechange()
    {
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        textskip.SetActive(false);
    }

    public void EndCheck()
    {
        if (i <= stringArray.Length - 1)
        {
            _textMeshPro.text = stringArray[i];
            StartCoroutine(TextVisible());
        }
    }

    private IEnumerator TextVisible()
    {
        _textMeshPro.ForceMeshUpdate();
        int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            _textMeshPro.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
            {
                i += 1;
                Invoke("EndCheck", timeBtwnWords);
                break;
            }

            counter += 1;
            yield return new WaitForSeconds(timeBtwnChars);
        }
        FindAnyObjectByType<AudioManager>().Stop("type");
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            if (textskipTMP != null)
            {
                for (float alpha = 1f; alpha >= 0f; alpha -= 0.05f)
                {
                    SetAlpha(textskipTMP, alpha);
                    yield return new WaitForSeconds(0.05f);
                }

                for (float alpha = 0f; alpha <= 1f; alpha += 0.05f)
                {
                    SetAlpha(textskipTMP, alpha);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
    }

    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}
