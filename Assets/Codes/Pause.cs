using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{

    public GameObject pauseMenu; // Assign in Inspector
    private bool isPaused = false;

    // Update is called once per frame
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
            Time.timeScale = 0f; // Pause game
            pauseMenu.SetActive(true); // Show pause menu
            Cursor.lockState = CursorLockMode.None; // Unlock cursor
            Cursor.visible = true; // Make cursor visible
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Time.timeScale = 1f; // Resume game
            pauseMenu.SetActive(false); // Hide pause menu
            Cursor.lockState = CursorLockMode.Locked; // Lock cursor
            Cursor.visible = false; // Hide cursor
        }
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor
        Cursor.visible = false; // Hide cursor
        isPaused = false;
    }

}
