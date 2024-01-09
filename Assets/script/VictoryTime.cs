using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VictorySceneScript : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText;

    void Start()
    {
        // Récupérez le temps final à partir de PlayerPrefs
        float finalTime = PlayerPrefs.GetFloat("FinalTime", -1);

        if (finalTime >= 0)
        {
            DisplayFinalTime(finalTime);
        }
        else
        {
            Debug.LogError("Le temps final n'a pas été correctement enregistré.");
        }
    }

    void DisplayFinalTime(float finalTime)
{
    int minutes = Mathf.FloorToInt(finalTime / 60);
    int seconds = Mathf.FloorToInt(finalTime % 60);
    finalTimeText.text = "Temps final : " + minutes.ToString("00") + ":" + seconds.ToString("00");
}

}
