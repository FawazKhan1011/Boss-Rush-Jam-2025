using System.Collections;
using UnityEngine;

public class MeteorBall : MonoBehaviour
{
    public GameObject colliderObject; // The object to unhide
    public float delay = 1f;          // Delay in seconds before unhiding

    private void Start()
    {
        if (colliderObject != null)
        {
            // Start the coroutine to unhide the object
            StartCoroutine(UnhideAfterDelay());
        }
        else
        {
            Debug.LogWarning("No object assigned to unhide!");
        }
    }

    private IEnumerator UnhideAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Unhide the object by setting it active
        colliderObject.SetActive(true);
    }
}
