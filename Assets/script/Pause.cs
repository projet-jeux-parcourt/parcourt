using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public bool isPaused = false;

    private void Start()
    {
        PauseMenuDisable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused == true)
            {
                PauseMenuDisable();
            }
            else
            {
                PauseMenuEnable();
            }
        }
    }

    public void PauseMenuEnable()
    {
        pauseCanvas.SetActive(true);
        //Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void PauseMenuDisable()
    {
        pauseCanvas.SetActive(false);
        //Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }
}
