using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager ScrManage { get; set; }

    [SerializeField]
    private TextMeshProUGUI CurrentScoreText = null;

    [SerializeField]
    private TextMeshProUGUI HighScoreText = null;

    [System.NonSerialized]
    public int Highscore = 0;

    [System.NonSerialized]
    public int CurrentScore = 0;
    void Awake()
    {
        if (ScrManage == null)
        {
            ScrManage = this;
        }
        else if(ScrManage != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Highscore = PlayerPrefs.GetInt("HighScore");
        HighScoreText.text = "Highscore: " + Highscore;
    }

    public void AddScore(int Score)
    {
        CurrentScore += Score;
        CurrentScoreText.text = CurrentScore.ToString();

        if (CurrentScore > Highscore)
        {
            PlayerPrefs.SetInt("HighScore", CurrentScore);
            Highscore = CurrentScore;
            HighScoreText.text = "Highscore: " + CurrentScore;
        }
      
    }
}
