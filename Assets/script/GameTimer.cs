using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float countdownTime = 60.0f;
    public LoadSceneScript loadSceneScript;

    private void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("Veuillez attribuer le composant TextMeshProUGUI dans l'éditeur Unity.");
        }
        else
        {
            UpdateTimer();
        }
    }

    private void Update()
    {
        countdownTime -= Time.deltaTime;

        if (countdownTime <= 0.0f)
        {
            countdownTime = 0.0f;
            LoadNewScene();
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timerText.text = "Temps restant : " + Mathf.Ceil(countdownTime).ToString();
    }

    private void LoadNewScene()
    {
        if (loadSceneScript != null)
        {
            loadSceneScript.LoadScene();
        }
        else
        {
            Debug.LogError("Veuillez attribuer le script LoadSceneScript dans l'éditeur Unity.");
        }
    }
}
