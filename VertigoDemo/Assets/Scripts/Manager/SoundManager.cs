using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager SoManage { get; set; }
    private void Awake()
    {
        if (SoManage == null)
        {
            SoManage = this;
        }
        else if (SoManage != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip AC, float Volume)
    {
        for (int i = 0; i < GetComponents<AudioSource>().Length; i++)
        {
            if (!GetComponents<AudioSource>() [i].isPlaying)
            {
                GetComponents<AudioSource>()[i].clip = AC;
                GetComponents<AudioSource>()[i].Play();
                GetComponents<AudioSource>()[i].volume = Volume;
            }
        }
    }
}
