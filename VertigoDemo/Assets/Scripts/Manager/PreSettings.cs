using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreSettings : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }
}
