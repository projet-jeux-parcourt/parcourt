using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryAreaScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Cursor.lockState = CursorLockMode.None;
        PlayerPrefs.Save();
        SceneManager.LoadScene("YouWinMenu");
    }
}