using UnityEngine;

public class TitleScreenCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;  // Adjust camera sway sensitivity
    [SerializeField] private float maxRotation = 5f;  // Limit max rotation from default
    [SerializeField] private float smoothSpeed = 5f;  // Smooth transition speed

    private Vector3 defaultRotation; // Stores the initial rotation

    void Start()
    {
        FindAnyObjectByType<AudioManager>().Play("fire");
        FindAnyObjectByType<AudioManager>().Play("music");
        Time.timeScale = 1f;
        defaultRotation = transform.eulerAngles; // Save initial rotation
    }

    void Update()
    {
        // Get mouse position relative to screen center (-1 to 1 range)
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Calculate target rotation relative to default rotation
        float rotX = Mathf.Clamp(defaultRotation.x + (-mouseY * sensitivity), defaultRotation.x - maxRotation, defaultRotation.x + maxRotation);
        float rotY = Mathf.Clamp(defaultRotation.y + (mouseX * sensitivity), defaultRotation.y - maxRotation, defaultRotation.y + maxRotation);

        Vector3 targetRotation = new Vector3(rotX, rotY, defaultRotation.z);

        // Smoothly rotate the camera
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * smoothSpeed);
    }
}
