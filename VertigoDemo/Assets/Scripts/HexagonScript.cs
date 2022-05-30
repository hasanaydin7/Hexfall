using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonScript : MonoBehaviour
{
    [System.NonSerialized]
    public Vector2Int Coordinates = Vector2Int.zero;

    [System.NonSerialized]
    public bool isBomb = false;
}
