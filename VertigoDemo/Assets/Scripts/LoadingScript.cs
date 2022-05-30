using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Anim());
    }

    // Update is called once per frame
    IEnumerator Anim()
    {
        while (true)
        {
            for (int i = 1; i < 4; i++)
            {
                GetComponent<TextMeshPro>().text = "Loading" + HowManyDots(i);
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }

    string HowManyDots(int Dot)
    {
        string dots = "";
        for (int i = 0; i < Dot; i++)
        {
            dots += ".";
        }
        return dots;
    }
}
