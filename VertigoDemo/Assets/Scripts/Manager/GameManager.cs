using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
   public static GameManager GM { get; set; }

    [SerializeField]
    private GameObject GameOverPanel = null;

    [System.NonSerialized]
    public bool isGameOver = false;
    private void Awake()
    {
        if (GM == null)
        {
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {


        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.SetString("Username", " ");
        }
    }

    public void RePlay()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(0.65f);
        LeanTween.value(0, 255, 0.5f).setIgnoreTimeScale(true).setOnUpdate((float val) =>
        {
            if (GameOverPanel != null)
            {
                GameOverPanel.GetComponent<Image>().color = new Color32(100, 100, 100, (byte)val);
            }
        });
        GameOverPanel.transform.GetChild(0).gameObject.SetActive(true);
        GameOverPanel.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
        //Debug.Log("cikis oldu.");
    }
}
