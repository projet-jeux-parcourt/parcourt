using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public RawImage timeBar;
    public float countdownTime = 180.0f;
    public float initialWidth = 400.0f;
    private float TimeSet;
    public LoadSceneScript loadSceneScript;
    public Color startColor = Color.green;
    public Color endColor = Color.red;

    private void Start()
    {
        TimeSet = countdownTime;
        if (timerText == null || timeBar == null)
        {
            Debug.LogError("Veuillez attribuer les composants TextMeshProUGUI et Image (ou autre objet d'UI) dans l'éditeur Unity.");
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
            Cursor.lockState = CursorLockMode.None;
            LoadNewScene();
        }
        PlayerPrefs.SetFloat("FinalTime", countdownTime);
        UpdateTimer();
        UpdateTimeBar();
    }

    private void UpdateTimer()
    {
        float minutes = Mathf.Floor(countdownTime / 60);
        float seconds = Mathf.Floor(countdownTime % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    private void UpdateTimeBar()
    {
        float percentage = countdownTime / TimeSet;
        float newWidth = initialWidth * percentage;
        RectTransform rt = timeBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(newWidth, rt.sizeDelta.y);
        Color lerpedColor = Color.Lerp(endColor, startColor, percentage);
        timeBar.color = lerpedColor;
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
