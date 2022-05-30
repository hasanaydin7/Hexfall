using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager ScManage { get; private set; }
    private void Awake()
    {
        if (ScManage == null)
        {
            ScManage = this;
        }
        else if(ScManage != this)
        {
            Destroy(gameObject);
        }
    }

    public float getScreenHeight()
    {
        return Camera.main.orthographicSize * 2.0f;
    }

    public float getScreenWidht()
    {
        return getScreenHeight() * Screen.width / Screen.height;
    }

}
