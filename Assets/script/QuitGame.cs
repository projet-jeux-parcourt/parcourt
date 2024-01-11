using UnityEngine;
using UnityEngine.UI;

public class QuitGameOnClick : MonoBehaviour
{
    // Attachez ce script à votre bouton dans l'éditeur Unity
    public Button quitButton;

    // Utilisez cette méthode pour l'initialisation
    void Start()
    {
        // Assurez-vous que le bouton est assigné
        if (quitButton != null)
        {
            // Ajoutez un auditeur d'événements pour le clic sur le bouton
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogError("Le bouton n'est pas assigné dans l'éditeur Unity.");
        }
    }

    // Méthode appelée lorsque le bouton est cliqué
    void QuitGame()
    {
        // Affiche un message dans la console (facultatif)
        Debug.Log("Quitting the game...");

        // Quitte l'application
        Application.Quit();
    }
}
