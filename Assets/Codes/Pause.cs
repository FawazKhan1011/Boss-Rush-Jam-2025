using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu; // Assign in Inspector
    public GameObject instuct;   // Assign in Inspector

    private bool isPaused = false;

    private void Start()
    {
        LockCursor(); // Ensure cursor is locked at start
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
         TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PlayHoverSound()
    {
        FindAnyObjectByType<AudioManager>().Play("hover");
    }
    void PauseGame()
    {
        FindAnyObjectByType<AudioManager>().Play("button");
        Time.timeScale = 0f; // Pause game
        pauseMenu.SetActive(true); // Show pause menu
        UnlockCursor();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        FindAnyObjectByType<AudioManager>().Play("button");
        Time.timeScale = 1f; // Resume game
        pauseMenu.SetActive(false); // Hide pause menu
        LockCursor();
        isPaused = false;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
