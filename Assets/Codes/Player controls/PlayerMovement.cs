using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerBody; // Parent GameObject for scaling

    // SPEED
    [Space(10)]
    [Header("Speed Settings")]
    public float speed = 12f;
    public float runningSpeed = 20f;
    public float crouchSpeed = 6f; // Crouch speed
    public float speedTransitionTime = 0.5f;

    [Space(10)]
    [Header("Generals")]
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // BOBBING
    [Space(10)]
    [Header("Bobbing Settings")]
    public Transform cameraTransform;
    public float bobbingSpeed = 14f;
    public float runningBobbingSpeed = 20f;
    public float crouchBobbingSpeed = 7f;  // New crouch bobbing speed
    public float bobbingAmount = 0.05f;
    public float runningBobbingAmount = 0.1f;
    public float crouchBobbingAmount = 0.02f;  // New crouch bobbing amount
    public float bobbingTransitionTime = 0.5f;

    // CAMERA SETTINGS
    [Space(10)]
    [Header("Camera Settings")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float runningFOV = 70f;
    public float fovTransitionTime = 0.5f;

    // JUMP
    [Space(10)]
    [Header("Jump Settings")]
    public bool jumpWanted = true;
    public float jumpHeight = 3f;

    // Crouching
    [Space(10)]
    [Header("Crouching Settings")]
    public float crouchedHeight = 0.8f; // Scale for crouching
    private float originalHeight; // Store original Y size
    private bool isCrouching = false;
    public float heightTransitionSpeed = 5f;  // Speed for smooth crouch transition

    // PLAYER HEALTH
    [Space(10)]
    [Header("Player Health")]
    private Vector3 originalCameraPosition;
    private float timer = 0f;
    private float currentSpeed;
    private float currentBobbingSpeed;
    private float currentBobbingAmount;
    private bool isRunning = false;

    public int currentHealth;
    public int maxHealth = 10;
    public Slider slider;
    public LineDrawing lineDrawing;
    Vector3 velocity;
    bool isGrounded;
    public GameObject bloodEffect; // Assign in the inspector
    public GameObject blackscreen;
    bool isWalking = false;

    void Start()
    {
        currentHealth = maxHealth;
        SetMaxHealth(maxHealth);
        originalCameraPosition = cameraTransform.localPosition;
        originalHeight = playerBody.localScale.y; // Store original Y size
        currentSpeed = speed;
        currentBobbingSpeed = bobbingSpeed;
        currentBobbingAmount = bobbingAmount;
        playerCamera.fieldOfView = normalFOV;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Determine movement speed based on crouching state
        currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runningSpeed : speed);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        HandleHeadBobbing(x, z);

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrouching)
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        if (!isCrouching)
        {
            Running(isRunning);  // Only run if not crouching
        }

        if (Input.GetButtonDown("Jump") && isGrounded && jumpWanted)
        {
            FindAnyObjectByType<AudioManager>().Play("jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // Detect player movement and play walking sound without IsPlaying
        if (isGrounded && (x != 0 || z != 0))
        {
            if (!isWalking)  // Only play if not already walking
            {
                FindAnyObjectByType<AudioManager>().Play("walk");
                isWalking = true;
            }
        }
        else
        {
            if (isWalking)  // Stop sound only if it was playing
            {
                FindAnyObjectByType<AudioManager>().Stop("walk");
                isWalking = false;
            }
        }
    }

    void HandleWalkingSound(float x, float z)
    {
        bool isMoving = (x != 0 || z != 0) && isGrounded;

        if (isMoving)
        {
            FindAnyObjectByType<AudioManager>().Play("walk");
        }
        else if (!isMoving)
        {
            FindAnyObjectByType<AudioManager>().Stop("walk");
        }
    }


    void HandleHeadBobbing(float x, float z)
    {
        if (Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f)
        {
            timer += Time.deltaTime * currentBobbingSpeed;
            cameraTransform.localPosition = new Vector3(
                originalCameraPosition.x,
                originalCameraPosition.y + Mathf.Sin(timer) * currentBobbingAmount,
                originalCameraPosition.z
            );
        }
        else
        {
            timer = 0f;
            cameraTransform.localPosition = new Vector3(
                originalCameraPosition.x,
                Mathf.Lerp(cameraTransform.localPosition.y, originalCameraPosition.y, Time.deltaTime * currentBobbingSpeed),
                originalCameraPosition.z
            );
        }
    }

    void Running(bool isRunning)
    {
        float targetBobbingSpeed = isRunning ? runningBobbingSpeed : bobbingSpeed;
        float targetBobbingAmount = isRunning ? runningBobbingAmount : bobbingAmount;
        float targetFOV = isRunning ? runningFOV : normalFOV;

        // Smoothly transition the values
        currentBobbingSpeed = Mathf.Lerp(currentBobbingSpeed, targetBobbingSpeed, Time.deltaTime / bobbingTransitionTime);
        currentBobbingAmount = Mathf.Lerp(currentBobbingAmount, targetBobbingAmount, Time.deltaTime / bobbingTransitionTime);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime / fovTransitionTime);
    }
    IEnumerator CrouchTransition(float targetHeight, float targetBobbingSpeed, float targetBobbingAmount)
    {
        // Smooth transition for player height
        float startHeight = playerBody.localScale.y;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * heightTransitionSpeed;
            float newYScale = Mathf.Lerp(startHeight, targetHeight, elapsedTime);
            playerBody.localScale = new Vector3(1f, newYScale, 1f);

            yield return null;  // Wait for next frame
        }

        // Apply crouch bobbing values
        currentBobbingSpeed = targetBobbingSpeed;
        currentBobbingAmount = targetBobbingAmount;
    }

    public void doDamage(int damage)
    {
        int randomSound = Random.Range(1, 4); // Generates a random number between 1 and 3
        FindAnyObjectByType<AudioManager>().Play("retro");

        // Play different sounds based on the random number
        switch (randomSound)
        {
            case 1:
                FindAnyObjectByType<AudioManager>().Play("hurt1");
                break;
            case 2:
                FindAnyObjectByType<AudioManager>().Play("hurt2");
                break;
            case 3:
                FindAnyObjectByType<AudioManager>().Play("hurt3");
                break;
        }

        lineDrawing.InstantClearLine();

        if (bloodEffect != null)
        {
            bloodEffect.SetActive(true); // Activate blood effect
            StartCoroutine(FadeOutBlood()); // Start fading effect
        }

        StartCoroutine(CameraShake(0.3f, 0.4f)); // Shake for 0.2s with 0.2 intensity

        currentHealth -= damage;
        SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Camera Shake Coroutine
    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPosition = cameraTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            cameraTransform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        cameraTransform.localPosition = originalPosition; // Reset camera position
    }


    private void Die()
    {
        Debug.Log("Player has died!");

        this.enabled = false;
        controller.enabled = false;

        FindAnyObjectByType<AudioManager>().Play("death");

        // Optionally show game over UI
        // gameOverScreen.SetActive(true); // Uncomment if you have a Game Over screen
        blackscreen.SetActive(true);
        StartCoroutine(BlackScreen());
        StartCoroutine(DelayedSceneChange(2f, "Death")); // Wait 5 seconds before changing scene
    }

    public IEnumerator DelayedSceneChange(float delay, string scene)
    {
        yield return new WaitForSeconds(delay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene); // Change to "death" scene
    }

    public IEnumerator BlackScreen()
    {
        Image img = blackscreen.GetComponent<Image>(); // Get the Image component
        if (img == null) yield break;

        blackscreen.SetActive(true); // Ensure it's active before fading
        Color color = img.color;
        color.a = 0f;  // Start fully transparent
        img.color = color;

        float duration = 0.8f; // Fade duration
        float elapsed = 0f; // Start elapsed time at 0

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / duration); // Gradually increase alpha
            img.color = color;
            yield return null;
        }

        color.a = 1f;  // Ensure it is fully visible at the end
        img.color = color;
    }


    private IEnumerator FadeOutBlood()
    {
        Image img = bloodEffect.GetComponent<Image>(); // Get the Image component
        if (img == null) yield break;

        bloodEffect.SetActive(true); // Ensure it's active before fading
        Color color = img.color;
        color.a = 1f;  // Ensure it starts fully visible
        img.color = color;

        float duration = 0.6f; // Fade duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / duration); // Reduce alpha over time
            img.color = color;
            yield return null;
        }

        bloodEffect.SetActive(false); // Disable once fully faded
    }



    public void SetMaxHealth(int health)
    {
        slider.value = health;
        slider.maxValue = health;
    }
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}