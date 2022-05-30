using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public GameObject BombAnim = null;

    [SerializeField]
    private AudioClip BombSound = null;

    int BombDuration = 5;

    bool FirstTimeInPerspective = true;

    Quaternion BombRotation;

    private void Awake()
    {
        BombRotation = transform.rotation;
    }
    private void Start()
    {
        transform.GetChild(0).GetComponent<TextMeshPro>().text = BombDuration.ToString();
    }
    private void Update()
    {
        transform.rotation = BombRotation;
    }

    public void UpdateBombDuration()
    {
        if (!FirstTimeInPerspective)
        {
            BombDuration--;
            transform.GetChild(0).GetComponent<TextMeshPro>().text = BombDuration.ToString();

            if (BombDuration <= 0)
            {
                SoundManager.SoManage.PlaySound(BombSound, 1f);
                Instantiate(BombAnim, transform.position, Quaternion.identity);
                GameManager.GM.isGameOver = true;
                GameManager.GM.GameOver();
                Destroy(this.gameObject);
            }
        }
        else
        {
            FirstTimeInPerspective = false;
        }
    }
}
