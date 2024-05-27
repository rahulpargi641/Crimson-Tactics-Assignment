using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public GameObject obstaclePrefab; 
    public ObstacleData obstacleData; 

    [SerializeField] private List<Waypoint> gridTiles; 

    private List<GameObject> obstacles = new List<GameObject>();

    private void Start()
    {
        if (obstacleData == null)
        {
            Debug.LogError("ObstacleData scriptable object is not assigned.");
            return;
        }

        GenerateObstacles();
    }

    private void GenerateObstacles()
    {
        foreach (Waypoint waypoint in gridTiles)
        {
            Vector2Int gridPosition = waypoint.GridPosition;

            if (IsWithinBounds(gridPosition))
            {
                bool hasObstacle = obstacleData.obstacleGrid[gridPosition.x, gridPosition.y];

                if (hasObstacle)
                    InstantiateObstacle(waypoint);
            }
        }
    }

    private bool IsWithinBounds(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.x < 10 && coordinates.y >= 0 && coordinates.y < 10;
    }

    private void InstantiateObstacle(Waypoint waypoint)
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("Obstacle prefab is not assigned.");
            return;
        }

        Vector3 position = waypoint.transform.position + Vector3.up * 0.5f;
        GameObject obstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);
        obstacles.Add(obstacle);
    }

    public void ClearObstacles()
    {
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        obstacles.Clear();
    }
}
