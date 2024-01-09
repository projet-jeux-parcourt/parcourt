using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject clickBlocker;
    private bool isPaused = false;

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
        Time.timeScale = 0f; // Arrête le temps dans le jeu
        pauseMenuUI.SetActive(true);
        clickBlocker.SetActive(true); // Active le masque pour intercepter les clics
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Reprend le temps dans le jeu
        pauseMenuUI.SetActive(false);
        clickBlocker.SetActive(false); // Désactive le masque
        isPaused = false;
    }

    // Ajoutez d'autres fonctions pour gérer les options du menu (quitter le jeu, paramètres, etc.) si nécessaire
}
