using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverAreaScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("YouLostMenu");
    }
}