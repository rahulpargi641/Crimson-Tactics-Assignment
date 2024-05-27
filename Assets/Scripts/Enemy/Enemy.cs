using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, AI
{
    [SerializeField] float moveSpeed = 0.1f;
    [SerializeField] LayerMask tileLayer;
    [SerializeField] private float raycastDistance = 1.5f;

    private bool isEnemyMoving = false;
    private List<Waypoint> path;
    private Player player;
    private Animator animator;

    private void Start()
    {
        InitializeEnemyAI();
    }

    private void Update()
    {
        if (!isEnemyMoving)
        {
            MoveToPlayer();
        }
        else if (player.IsPlayerMoving) 
        {
            RecalculatePath(); // Recalculate path if player is moving
        }
    }

    private void InitializeEnemyAI()
    {
        player = FindObjectOfType<Player>();
        animator = GetComponent<Animator>();
    }

    public void MoveToPlayer()
    {
        if (player == null) return;

        Waypoint playerWaypoint = player.CurrentWayPoint;
        if (playerWaypoint == null || IsAdjacentToPlayer(playerWaypoint)) return;

        Waypoint targetWaypoint = FindAdjacentTileToPlayer(playerWaypoint);
        if (targetWaypoint == null) return;

        path = PathFinder.Instance.GetPath(GetCurrentWaypoint(), targetWaypoint);
        if (path == null || path.Count == 0) return;

        // Check if the player's current waypoint is in the path
        if (path.Contains(playerWaypoint))
        {
            int playerWaypointIndex = path.IndexOf(playerWaypoint); // Remove the player's current waypoint and all waypoints after it from the path
            path.RemoveRange(playerWaypointIndex, path.Count - playerWaypointIndex);
        }

        StartCoroutine(FollowPath(path));
    }

    private bool IsAdjacentToPlayer(Waypoint playerWaypoint)
    {
        Waypoint currentWaypoint = GetCurrentWaypoint();
        if (currentWaypoint == null) return false;

        Vector2Int playerPosition = playerWaypoint.GridPosition;
        Vector2Int currentPosition = currentWaypoint.GridPosition;

        return Mathf.Abs(playerPosition.x - currentPosition.x) <= 1 &&
               Mathf.Abs(playerPosition.y - currentPosition.y) <= 1;
    }

    private Waypoint GetCurrentWaypoint()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, tileLayer))
        {
            return hit.collider.GetComponent<Waypoint>();
        }
        return null;
    }

    private Waypoint FindAdjacentTileToPlayer(Waypoint playerWaypoint)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        foreach (var direction in directions)
        {
            Vector2Int adjacentPosition = playerWaypoint.GridPosition + direction;
            if (PathFinder.Instance.Grid.TryGetValue(adjacentPosition, out Waypoint adjacentWaypoint))
            {
                if (adjacentWaypoint != null && !adjacentWaypoint.IsExplored)
                {
                    return adjacentWaypoint;
                }
            }
        }
        return null;
    }

    private IEnumerator FollowPath(List<Waypoint> path)
    {
        isEnemyMoving = true;
        animator.SetBool("Run", true);

        foreach (Waypoint waypoint in path)
        {
            Waypoint nextWaypoint = GetNextWaypoint(path, waypoint);
            RotateTowardsNextWaypoint(waypoint, nextWaypoint);
            yield return StartCoroutine(MoveToNextWaypoint(waypoint, nextWaypoint));
        }

        isEnemyMoving = false;
        TriggerAttackAnimation();
        FaceTowardsPlayer();
    }

    private void TriggerAttackAnimation()
    {
        animator.SetBool("Run", false);
        animator.SetTrigger("Attack 01");
    }

    private Waypoint GetNextWaypoint(List<Waypoint> path, Waypoint currentWaypoint)
    {
        int currentIndex = path.IndexOf(currentWaypoint);
        return currentIndex + 1 < path.Count ? path[currentIndex + 1] : null;
    }

    private void RotateTowardsNextWaypoint(Waypoint currentWaypoint, Waypoint nextWaypoint)
    {
        if (nextWaypoint != null)
        {
            Vector3 direction = nextWaypoint.transform.position - currentWaypoint.transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
    }

    private IEnumerator MoveToNextWaypoint(Waypoint currentWaypoint, Waypoint nextWaypoint)
    {
        if (nextWaypoint != null)
        {
            Vector3 startPos = currentWaypoint.transform.position;
            Vector3 endPos = nextWaypoint.transform.position;
            float duration = CalculateMovementDuration(startPos, endPos);

            yield return SmoothMovement(startPos, endPos, duration);
        }
    }

    private float CalculateMovementDuration(Vector3 startPos, Vector3 endPos)
    {
        float distance = Vector3.Distance(startPos, endPos);
        return moveSpeed * distance;
    }

    private IEnumerator SmoothMovement(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos; // Ensure player reaches the exact position of the next waypoint
    }

    private void FaceTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
    }

    private void RecalculatePath()
    {
        StopAllCoroutines();
        animator.SetBool("Run", false);
        isEnemyMoving = false;
        MoveToPlayer(); // Recalculate path to follow player
    }
}
