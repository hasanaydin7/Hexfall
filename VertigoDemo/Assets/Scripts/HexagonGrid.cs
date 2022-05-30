using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class HexagonGrid : MonoBehaviour
{
    [Header("Hexagon Settings")]
    public int GridSizeX = 8;
    public int GridSizeY = 9;
    [Header("Color Settings")]
    public Color[] HexagonColors = null;
    [SerializeField]
    private int BombThresold = 700;

    [Header(" ")]
    public Transform HexaGrid = null;

    [SerializeField]
    GameObject HexagonPrefab = null;
    [SerializeField]
    GameObject BombPrefab = null;

    public GameObject[,] Hexagons;

    float hexagonWidth = 1f;
    float hexagonHeight = 1f;

    [SerializeField]
    float offsetSpace = 0.05f;

    float NewHexagonStartPosY = 0;

    Vector2 startPosition = Vector2.zero;

    int InitialBombThresold = 0;



    // Start is called before the first frame update
    void Start()
    {
        HexagonPrefab.transform.localScale = new Vector2(((ScreenManager.ScManage.getScreenWidht() / (GridSizeX + 2f))) - offsetSpace,
                                                      ((ScreenManager.ScManage.getScreenWidht() / (GridSizeX + 2f))) - offsetSpace);
        hexagonWidth = HexagonPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        hexagonHeight = HexagonPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        CalcStartPos();

        Hexagons = new GameObject[GridSizeX, GridSizeY];
        CreateGrid(GridSizeX, GridSizeY);

        NewHexagonStartPosY = CalcWorldPos(Vector2.zero).y + HexagonPrefab.transform.localScale.y;

        InitialBombThresold = BombThresold;

    }

    void CalcStartPos()
    {
        float offset = HexagonPrefab.GetComponent<SpriteRenderer>().bounds.size.x / 2f;

        float x = (-ScreenManager.ScManage.getScreenWidht() / 2f) +  offset + ((offsetSpace / 2f) * (GridSizeX + 0.5f));
        float y = ScreenManager.ScManage.getScreenHeight() / 6f;

        startPosition = new Vector2(x, y);
    }

    public Vector2 CalcWorldPos(Vector2 gridPos)
    {
        float offset = 0;
        if (gridPos.y % 2 != 0)
            offset = hexagonWidth / 2f;

        float x = startPosition.x + gridPos.x * hexagonWidth + offset;
        float y = startPosition.y - gridPos.y * hexagonHeight * 0.75f;

        return new Vector2(x, y);

    }

    public void NextHexagon(GameObject[,] hexArray, List<Transform> ThreeHexagons)
    {

        for (int offset = 0; offset < 2; offset++)
        {
            for (int x = 0; x < hexArray.GetLength(0); x++)
            {
                int bottomYIndex = hexArray.GetLength(1) - offset - 1;

                List<Vector2Int> sourceIndices = new List<Vector2Int>();

                for (int y = bottomYIndex; y >= 0; y -= 2)
                {
                    if (hexArray[x, y] != null)
                    {
                        sourceIndices.Add(new Vector2Int(x, y));
                    }
                }

                for (int y = bottomYIndex; y >= 0; y -= 2)
                {
                    if (sourceIndices.Count > 0)
                    {
                        hexArray[x, y] = hexArray[sourceIndices[0].x, sourceIndices[0].y];

                        if (!ThreeHexagons.Contains(hexArray[x, y].transform))
                        {
                            //transform.DOMove(CalculateWorldPosition(new Vector2(x, y)), 0.2f,false);
                            LeanTween.move(hexArray[x, y], CalcWorldPos(new Vector2(x, y)), 0.2f);
                        }
                        sourceIndices.RemoveAt(0);
                    }
                    else
                    {
                        Vector2 NewHexPosition = CalcWorldPos(new Vector2(x, y));

                        hexArray[x, y] = Instantiate(HexagonPrefab,
                            new Vector2(NewHexPosition.x, NewHexagonStartPosY),
                            Quaternion.identity, HexaGrid.transform);

                        LeanTween.move(hexArray[x, y], NewHexPosition, 0.2f);

                        Hexagons[x, y].GetComponent<SpriteRenderer>().color = HexagonColors[Random.Range(0, HexagonColors.Length)];

                        if (BombThresold <= ScoreManager.ScrManage.CurrentScore)
                        {
                            GameObject Bomb = Instantiate(BombPrefab,
                                Hexagons[x, y].transform.position,
                                Quaternion.identity,
                                Hexagons[x, y].transform);

                            Hexagons[x, y].GetComponent<HexagonScript>().isBomb = true;

                            if (InitialBombThresold != 100 && InitialBombThresold != 150)
                            {
                                InitialBombThresold -= 100;
                            }
                            else
                            {
                                InitialBombThresold = 150;
                            }

                            BombThresold += InitialBombThresold;
                        }
                    }


                    hexArray[x, y].GetComponent<HexagonScript>().Coordinates = new Vector2Int(x, y);
                    hexArray[x, y].transform.name = "X: " + x + " | Y: " + y;

                }
            }
        }
    }




    public void CreateGrid(int gridWidth, int gridHeight)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {


                GameObject Hexagon = Instantiate(HexagonPrefab, Vector2.zero, Quaternion.identity, HexaGrid);
                Hexagons[x, y] = Hexagon;

                if (x > 0)
                {
                    do
                    {
                        Hexagons[x, y].GetComponent<SpriteRenderer>().color = HexagonColors[Random.Range(0, HexagonColors.Length)];

                    } while (Hexagons[x, y].GetComponent<SpriteRenderer>().color == Hexagons[x - 1, y].GetComponent<SpriteRenderer>().color);

                }
                else
                {
                    Hexagons[x, y].GetComponent<SpriteRenderer>().color = HexagonColors[Random.Range(0, HexagonColors.Length)];
                }

                Vector2 gridPos = new Vector2(x, y);
                Hexagon.transform.position = CalcWorldPos(gridPos);
                Hexagon.GetComponent<HexagonScript>().Coordinates = new Vector2Int(x, y);
                Hexagon.transform.name = "X: " + x + " | Y: " + y;


            }

        }

    }

}
