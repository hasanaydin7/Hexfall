using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using DG.Tweening;

public class GamePlay : MonoBehaviour
{
    public Transform HexGrid = null;

    HexagonGrid HG = null;

    public bool GameInProgress = false;

    public GameObject HexagonExplosionAnim = null;

    [SerializeField]
    private AudioClip HexDestroySound = null;
    [SerializeField]
    private AudioClip HexTurnSound = null;

    bool DestroyedHexagonsTurning = false;

    [Header("Score Multiplier")]
    [SerializeField]
    int scoreThreshold = 5;
    void Awake()
    {
        HG = GetComponent<HexagonGrid>();
    }
    void Update()
    {

        if (!GameInProgress && !GameManager.GM.isGameOver)
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
#else
                if (Input.touches.Any(x => x.phase == TouchPhase.Began))
            {
#endif
#if UNITY_EDITOR
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
#else
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), Vector2.zero);
#endif

                if (hit.collider != null)
                {
                    DestroyedHexagonsTurning = false;
#if UNITY_EDITOR
                    Vector2 CurrentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#else
                    Vector2 CurrentMousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif


                    //DestroyHexagons();


                    StartCoroutine(RotateThreeTimes(CurrentMousePos));
                }
            }

        }

    }

    IEnumerator RotateThreeTimes(Vector2 MousePosition)
    {
        GameInProgress = true;
        GameObject ThreeClosest = new GameObject("ThreeClosest");

        List<Transform> Hexagon = new List<Transform>();
        Hexagon = GetThreeClosestHexagons(MousePosition);
        List<Vector2> HexagonPositions = new List<Vector2>();

        for (int i = 0; i < 3; i++)
        {
            HexagonPositions.Add(Hexagon[i].position);
        }

        ThreeClosest.transform.position = calculateCentre(Hexagon);

        for (int i = 0; i < 3; i++)
        {
            Hexagon[i].SetParent(ThreeClosest.transform);
        }

        for (int i = 0; i < 3; i++)
        {
            if (DestroyedHexagonsTurning)
            {
                for (int h = 0; h < Hexagon.Count; h++)
                {
                    if (Hexagon[h] != null)
                    {
                        Hexagon[h].SetParent(HexGrid);
                    }
                }
                    Destroy(ThreeClosest);
                while (DestroyedHexagonsTurning)
                {
                        yield return new WaitForSeconds(0.42f);
                        Hexagon = new List<Transform>();
                        CheckHexagonsAndDestroy();
                        HG.NextHexagon(HG.Hexagons, Hexagon);   
                }
                break;
            }

            RotateSelectedHexagons(ThreeClosest, Hexagon, HG.Hexagons);
            yield return new WaitForSeconds(0.28f);
        }
        for (int h = 0; h < Hexagon.Count; h++)
        {
            if (Hexagon[h] != null)
            {
                Hexagon[h].SetParent(HexGrid);
            }           
        }
        Destroy(ThreeClosest);

        while (DestroyedHexagonsTurning)
        {
            yield return new WaitForSeconds(0.42f);

            Hexagon = new List<Transform>();
            CheckHexagonsAndDestroy();
            HG.NextHexagon(HG.Hexagons, Hexagon);
        }
        yield return new WaitForSeconds(0.21f);

        GameInProgress = false;

        for (int y = 0; y < HG.GridSizeY; y++)
        {
            for (int x = 0; x < HG.GridSizeX; x++)
            {
                if (HG.Hexagons[x, y].GetComponent<HexagonScript>().isBomb)
                {
                    HG.Hexagons[x, y].transform.GetChild(0).GetComponent<BombScript>().UpdateBombDuration();
                }
            }
        }
    }

    void RotateSelectedHexagons(GameObject ThreeClosest, List<Transform> Hexagon, GameObject[,] Hexagons)
    {
        SoundManager.SoManage.PlaySound(HexTurnSound, 1f);

        Vector2Int hexagonACoordinate = Hexagon[0].GetComponent<HexagonScript>().Coordinates;
        Vector2Int hexagonBCoordinate = Hexagon[1].GetComponent<HexagonScript>().Coordinates;
        Vector2Int hexagonCCoordinate = Hexagon[2].GetComponent<HexagonScript>().Coordinates;

        //To determine the lone hexagon
        Vector2Int aloneHexagonCoord;
        Vector2Int leftHexagonCoord;
        Vector2Int rightHexagonCoord;

        if (hexagonACoordinate.y != hexagonBCoordinate.y && hexagonACoordinate.y != hexagonCCoordinate.y)
        {
            aloneHexagonCoord = hexagonACoordinate;

            leftHexagonCoord = hexagonBCoordinate;
            rightHexagonCoord = hexagonCCoordinate;
        }
        else if (hexagonBCoordinate.y != hexagonCCoordinate.y && hexagonBCoordinate.y != hexagonACoordinate.y)
        {
            aloneHexagonCoord = hexagonBCoordinate;

            leftHexagonCoord = hexagonACoordinate;
            rightHexagonCoord = hexagonCCoordinate;
        }
        else
        {
            aloneHexagonCoord = hexagonCCoordinate;

            leftHexagonCoord = hexagonACoordinate;
            rightHexagonCoord = hexagonBCoordinate;
        }

        if (leftHexagonCoord.x > rightHexagonCoord.x)
        {
            Vector2Int tempCoord = leftHexagonCoord;
            leftHexagonCoord = rightHexagonCoord;
            rightHexagonCoord = tempCoord;
        }

        if (aloneHexagonCoord.y > leftHexagonCoord.y)
        {

            GameObject tempHex = Hexagons[aloneHexagonCoord.x, aloneHexagonCoord.y];

            Hexagons[aloneHexagonCoord.x, aloneHexagonCoord.y] = Hexagons[rightHexagonCoord.x, rightHexagonCoord.y];

            Hexagons[rightHexagonCoord.x, rightHexagonCoord.y] = Hexagons[leftHexagonCoord.x, leftHexagonCoord.y];

            Hexagons[leftHexagonCoord.x, leftHexagonCoord.y] = tempHex;
        }
        else
        {
            GameObject tempHex = Hexagons[aloneHexagonCoord.x, aloneHexagonCoord.y];

            Hexagons[aloneHexagonCoord.x, aloneHexagonCoord.y] = Hexagons[leftHexagonCoord.x, leftHexagonCoord.y];

            Hexagons[leftHexagonCoord.x, leftHexagonCoord.y] = Hexagons[rightHexagonCoord.x, rightHexagonCoord.y];

            Hexagons[rightHexagonCoord.x, rightHexagonCoord.y] = tempHex;
        }

        Hexagons[aloneHexagonCoord.x, aloneHexagonCoord.y].GetComponent<HexagonScript>().Coordinates = new Vector2Int(aloneHexagonCoord.x, aloneHexagonCoord.y);

        Hexagons[leftHexagonCoord.x, leftHexagonCoord.y].GetComponent<HexagonScript>().Coordinates = new Vector2Int(leftHexagonCoord.x, leftHexagonCoord.y);

        Hexagons[rightHexagonCoord.x, rightHexagonCoord.y].GetComponent<HexagonScript>().Coordinates = new Vector2Int(rightHexagonCoord.x, rightHexagonCoord.y);

        //transform.DORotate(new Vector3(0, 0, -120) , 0.2f, RotateMode.FastBeyond360);


        LeanTween.rotateZ(ThreeClosest,
        (ThreeClosest.transform.rotation.eulerAngles.z) - 120,
        0.2f).setOnComplete(x => {
            MakeActionAfterRotation(HG.Hexagons, Hexagon);
        });

    }

    void MakeActionAfterRotation(GameObject[,]Hexagons, List<Transform> Hexagon)
    {
        CheckHexagonsAndDestroy();
        if (DestroyedHexagonsTurning)
        {
            Hexagon = new List<Transform>();
        }
        HG.NextHexagon(Hexagons, Hexagon);
    }

    void CheckHexagonsAndDestroy()
    {
        GameObject[,] Hexagons = HG.Hexagons;

        List<Vector2Int> HexagonToBeDestroyed = new List<Vector2Int>();

        for (int y = 0; y < HG.GridSizeY; y++)
        {
            for (int x = 0; x < HG.GridSizeX; x++)
            {
                Transform CurrentHexagon = Hexagons[x, y].transform;
                Color CurrentColor = CurrentHexagon.GetComponent<SpriteRenderer>().color;
                int MatchedColors = 0;

                if (y % 2 == 0)
                {
                    if (x - 1 >= 0 && y + 1 <= HG.GridSizeY - 1)
                    {
                        if (CurrentColor==Hexagons[x-1,y].GetComponent<SpriteRenderer>().color)//Left
                        {
                            MatchedColors++;
                        }
                        if (CurrentColor == Hexagons[x-1,y+1].GetComponent<SpriteRenderer>().color)//TopLeft
                        {
                            MatchedColors++;
                        }

                        if (MatchedColors == 2)
                        {
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x,y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x-1,y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x - 1, y));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x-1,y+1)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x - 1, y + 1));
                            }
                        }
                        MatchedColors = 0;

                        if (CurrentColor == Hexagons[x-1,y+1].GetComponent<SpriteRenderer>().color)//TopLeft
                        {
                            MatchedColors++;
                        }

                        if (CurrentColor == Hexagons[x,y+1].GetComponent<SpriteRenderer>().color)//TopRight
                        {
                            MatchedColors++;
                        }

                        if (MatchedColors == 2)
                        {
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y + 1)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y + 1));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x - 1, y + 1)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y + 1));
                            }
                        }
                        MatchedColors = 0;
                    }
                    if (x + 1 <= HG.GridSizeX - 1 && y + 1 <= HG.GridSizeY - 1)
                    {
                        if (CurrentColor == Hexagons[x,y+1].GetComponent<SpriteRenderer>().color)//TopRight
                        {
                            MatchedColors++;
                        }
                        if (CurrentColor == Hexagons[x+1, y].GetComponent<SpriteRenderer>().color)//Right
                        {
                            MatchedColors++;
                        }
                        if (MatchedColors == 2)
                        {
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y + 1)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y + 1));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x + 1, y)))
                            {
                                HexagonToBeDestroyed.Contains(new Vector2Int(x + 1, y));
                            }
                        }
                        MatchedColors = 0;
                    }

                    if (x + 1 <= HG.GridSizeX - 1 && y - 1 >= 0)
                    {
                        if (CurrentColor == Hexagons[x+1,y].GetComponent<SpriteRenderer>().color)//Right
                        {
                            MatchedColors++;
                        }
                        if (CurrentColor == Hexagons[x,y-1].GetComponent<SpriteRenderer>().color)//BottomRight
                        {
                            MatchedColors++;
                        }
                        if (MatchedColors == 2)
                        {
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y - 1)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y - 1));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x + 1, y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x + 1, y));
                            }
                        }
                        MatchedColors = 0;
                    }
                    if (x - 1 >= 0 && y - 1 >= 0)
                    {
                        if (CurrentColor == Hexagons[x, y - 1].GetComponent<SpriteRenderer>().color)//BottomRight
                        {
                            MatchedColors++;
                        }
                        if (CurrentColor == Hexagons[x - 1, y - 1].GetComponent<SpriteRenderer>().color)//BottomLeft
                        {
                            MatchedColors++;
                        }
                        if (MatchedColors == 2)
                        {
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y - 1)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y - 1));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x - 1, y - 1)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x - 1, y - 1));
                            }
                        }
                        MatchedColors = 0;

                        if (CurrentColor == Hexagons[x - 1, y - 1].GetComponent<SpriteRenderer>().color)//BottomLeft
                        {
                            MatchedColors++;
                        }
                        if (CurrentColor == Hexagons[x - 1,y].GetComponent<SpriteRenderer>().color)//Left
                        {
                            MatchedColors++;
                        }
                        if (MatchedColors == 2)
                        {
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x, y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x, y));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x - 1, y)))
                            {
                                HexagonToBeDestroyed.Add(new Vector2Int(x - 1, y));
                            }
                            if (!HexagonToBeDestroyed.Contains(new Vector2Int(x - 1, y - 1)))
                            {
                                HexagonToBeDestroyed.Contains(new Vector2Int(x - 1, y - 1));
                            }
                        }
                        MatchedColors = 0;
                    }
                }
            }
        }
        if (HexagonToBeDestroyed.Count > 0)
        {
            DestroyedHexagonsTurning = true;
            SoundManager.SoManage.PlaySound(HexDestroySound, 0.7f);
        }
        else
        {
            DestroyedHexagonsTurning = false;
        }

        ScoreManager.ScrManage.AddScore(HexagonToBeDestroyed.Count * scoreThreshold);

        foreach (Vector2Int item in HexagonToBeDestroyed)
        {
            if (HG.Hexagons[item.x,item.y] != null)
            {
                ParticleSystem PS = HexagonExplosionAnim.transform.GetChild(2).GetComponent<ParticleSystem>();
                ParticleSystem PS1 = HexagonExplosionAnim.transform.GetChild(3).GetComponent<ParticleSystem>();

                var main = PS.main;
                var main2 = PS.main;

                Color HexagonColor = HG.Hexagons[item.x, item.y].GetComponent<SpriteRenderer>().color;

                main.startColor = HexagonColor;
                main2.startColor = HexagonColor;

                GameObject HexagonAnim = Instantiate(HexagonExplosionAnim, HG.Hexagons[item.x, item.y].transform.position, Quaternion.identity);

                Destroy(HG.Hexagons[item.x, item.y]);
                HG.Hexagons[item.x, item.y] = null;
            }
        }
    }

    List<Transform> GetThreeClosestHexagons(Vector2 MousePosition)
    {
        List<Transform> Hexagon = new List<Transform>();
        for (int y = 0; y < HG.GridSizeY; y++)
        {
            for (int x = 0; x < HG.GridSizeX; x++)
            {
                Hexagon.Add(HG.Hexagons[x, y].transform);
            }
        }
        Hexagon = Hexagon.OrderBy(point => Vector3.Distance(MousePosition, point.transform.position)).ToList();
        return new List<Transform> { Hexagon[0], Hexagon[1], Hexagon[2] };
    }
    Vector2 calculateCentre(List<Transform> Hexagon)
    {
        Vector3 centre = Vector3.zero;
        foreach (Transform item in Hexagon)
        {
            centre += item.transform.position;
        }

        centre /= Hexagon.Count;

        return centre;
    }
}
