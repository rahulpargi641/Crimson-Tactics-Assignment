using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    protected const int gridCellSize = 10; // Individual fixed tile size
    public int GridCellSize => gridCellSize;

    public Vector2Int GridPosition
    {
        get => new Vector2Int(
            Mathf.RoundToInt(transform.position.x / GridCellSize),
            Mathf.RoundToInt(transform.position.z / GridCellSize));
    }
}
