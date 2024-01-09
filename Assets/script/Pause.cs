using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    private CursorLockMode previousMouse;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            { 
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        previousMouse=Cursor.lockState;
        Time.timeScale = 0f; 
        pauseMenuUI.SetActive(true);
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Cursor.lockState=previousMouse;
        Time.timeScale = 1f; 
        pauseMenuUI.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
