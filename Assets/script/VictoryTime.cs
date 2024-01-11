using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VictorySceneScript : MonoBehaviour
{
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI finalScore;

    void Start()
    {
        // Récupérez le temps final à partir de PlayerPrefs
        float finalTime = PlayerPrefs.GetFloat("FinalTime", 0);

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
    finalScore.text = "Score final : " + timeToScore(finalTime);
    }

    String timeToScore(float finalTime)
    {
        String ret;

        if (finalTime < 90)
        {
            ret = " Or ";
        }else if (finalTime < 120)
        {
            ret = " Argent ";
        }
        else
        {
            ret = " Bronze ";
        }
        return ret;
    }
}
