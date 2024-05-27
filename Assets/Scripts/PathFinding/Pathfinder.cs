using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoSingletonGeneric<PathFinder>
{
    [SerializeField] ObstacleData obstacleData;

    public Dictionary<Vector2, Waypoint> Grid => grid;
    private Dictionary<Vector2, Waypoint> grid = new Dictionary<Vector2, Waypoint>();

    private Queue<Waypoint> waypointQueue = new Queue<Waypoint>();
    private List<Waypoint> path = new List<Waypoint>();

    private Waypoint currentSearchCenter;
    private bool isAlgorithmRunning = true;

    private Vector2Int[] directions =
    {
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up,
    };

    internal List<Waypoint> GetPath(Waypoint start, Waypoint end)
    {
        ClearPathAndQueue();
        ResetExplorationState();
        SetupSearchParameters(start, end);
        LoadWayPointsInGridDictionary();
        PerformBreadthFirstSearch(end);

        if (end.ExploredFrom != null)
        {
            CreatePath(start, end);
        }

        return path;
    }

    private void ClearPathAndQueue()
    {
        path.Clear();
        waypointQueue.Clear();
    }

    private void ResetExplorationState()
    {
        foreach (Waypoint waypoint in grid.Values)
        {
            waypoint.IsExplored = false;
            waypoint.ExploredFrom = null;
        }
    }

    private void SetupSearchParameters(Waypoint start, Waypoint end)
    {
        currentSearchCenter = start;
        isAlgorithmRunning = true;

        start.IsExplored = true;
        end.IsExplored = false;
    }

    private void LoadWayPointsInGridDictionary()
    {
        //Waypoint[] waypoints = FindObjectsOfType<Waypoint>();
        List<Waypoint> waypoints = WaypointManager.Instance.Waypoints;
        foreach (Waypoint waypoint in waypoints)
        {
            Vector2 gridPos = waypoint.GridPosition;
            if (grid.ContainsKey(gridPos))
                Debug.LogWarning("Skipping Overlapping Block" + waypoint);
            else
                grid.Add(gridPos, waypoint);
        }
    }

    private void PerformBreadthFirstSearch(Waypoint endWayPoint)
    {
        waypointQueue.Enqueue(currentSearchCenter);

        while (waypointQueue.Count > 0 && isAlgorithmRunning)
        {
            currentSearchCenter = waypointQueue.Dequeue();
            HaltIfEndFound(endWayPoint);
            ExploreNeighbours();
            currentSearchCenter.IsExplored = true;
        }
    }

    private void HaltIfEndFound(Waypoint endWayPoint)
    {
        if (currentSearchCenter == endWayPoint)
        {
            isAlgorithmRunning = false;
        }
    }

    private void ExploreNeighbours()
    {
        if (!isAlgorithmRunning) return;

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbourCoordinate = currentSearchCenter.GridPosition + direction;

            if (grid.ContainsKey(neighbourCoordinate) && !IsObstacle(neighbourCoordinate))
            {
                QueueNewNeighbour(neighbourCoordinate);
            }
        }
    }

    private bool IsObstacle(Vector2Int gridPosition)
    {
        if (obstacleData != null)
        {
            return obstacleData.obstacleGrid[gridPosition.x, gridPosition.y];
        }
        return false;
    }

    private void QueueNewNeighbour(Vector2Int neighbourCoordinate)
    {
        Waypoint neighbour = grid[neighbourCoordinate];

        if (!neighbour.IsExplored && !waypointQueue.Contains(neighbour))
        {
            waypointQueue.Enqueue(neighbour);
            neighbour.ExploredFrom = currentSearchCenter;
        }
    }

    private void CreatePath(Waypoint start, Waypoint end)
    {
        SetAsPath(end);

        Waypoint previous = end.ExploredFrom;
        while (previous != start)
        {
            SetAsPath(previous);
            previous = previous.ExploredFrom;
        }

        SetAsPath(start);
        path.Reverse();
    }

    private void SetAsPath(Waypoint waypoint)
    {
        path.Add(waypoint);
    }
}
