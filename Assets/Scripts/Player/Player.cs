using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    public static event Action onDestinationReached;
    public Waypoint CurrentWayPoint => currentWaypoint;
    public bool IsPlayerMoving => isPlayerMoving;

    [SerializeField] private float movementSpeed = 1;
    [SerializeField] private LayerMask tileLayerMask;

    private bool isPlayerMoving = false;
    private Waypoint currentWaypoint; // where player is standing on
    private List<Waypoint> currentPath;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandlePathFindingOnClick();

        UpdateCurrentWaypoint();

        if (currentWaypoint != null)
        {
            Debug.Log("Player is standing on tile: " + currentWaypoint.GridPosition);
        }
    }

    private void HandlePathFindingOnClick()
    {
        if (!isPlayerMoving && Input.GetMouseButtonDown(0))
        {
            GetPathAndFollow();
        }
    }

    private void GetPathAndFollow()
    {
        Waypoint startWaypoint = currentWaypoint;
        Waypoint endWaypoint = FindEndWaypoint();

        if (startWaypoint == null || endWaypoint == null) return;

        currentPath = PathFinder.Instance.GetPath(startWaypoint, endWaypoint);

        if (currentPath != null && currentPath.Count > 0)
        {
            StartCoroutine(FollowPath(currentPath));
        }
        else
            Debug.Log("Path is null or empty");
    }

    private Waypoint FindEndWaypoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayerMask))
        {
            Waypoint endWayPoint = hit.collider.GetComponent<Waypoint>();
            if (endWayPoint != null)
            {
                return endWayPoint;
            }
        }
        return null;
    }

    private IEnumerator FollowPath(List<Waypoint> path)
    {
        isPlayerMoving = true;
        animator.SetBool("Run", true);

        foreach (var waypoint in path)
        {
            Waypoint nextWaypoint = GetNextWaypoint(waypoint, path);
            RotateTowardsNextWaypoint(waypoint, nextWaypoint);
            yield return MoveToNextWaypoint(waypoint, nextWaypoint);
        }

        onDestinationReached?.Invoke();
        isPlayerMoving = false;
        animator.SetBool("Run", false);
    }

    private Waypoint GetNextWaypoint(Waypoint currentWaypoint, List<Waypoint> path)
    {
        int currentIndex = path.IndexOf(currentWaypoint);
        return currentIndex < path.Count - 1 ? path[currentIndex + 1] : null;
    }

    private void RotateTowardsNextWaypoint(Waypoint currentWaypoint, Waypoint nextWaypoint)
    {
        if (nextWaypoint == null) return;

        Vector3 direction = CalculateDirection(currentWaypoint, nextWaypoint);
        Quaternion targetRotation = CalculateTargetRotation(direction);

        transform.rotation = targetRotation;

        //UpdateAnimatorParameters(direction);
    }

    private Vector3 CalculateDirection(Waypoint currentWaypoint, Waypoint nextWaypoint)
    {
        Vector3 direction = nextWaypoint.transform.position - currentWaypoint.transform.position;
        direction.y = 0f; 
        return direction;
    }

    private Quaternion CalculateTargetRotation(Vector3 direction)
    {
        if (direction == Vector3.zero) return transform.rotation;
        return Quaternion.LookRotation(direction);
    }

    private IEnumerator MoveToNextWaypoint(Waypoint currentWaypoint, Waypoint nextWaypoint)
    {
        if (nextWaypoint == null) yield break;

        Vector3 startPos = currentWaypoint.transform.position;
        Vector3 endPos = nextWaypoint.transform.position;
        float duration = CalculateMovementDuration(startPos, endPos);

        yield return SmoothMovement(startPos, endPos, duration);
    }

    private float CalculateMovementDuration(Vector3 startPos, Vector3 endPos)
    {
        float distance = Vector3.Distance(startPos, endPos);
        return movementSpeed * distance; // Adjust duration based on distance
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

    private void UpdateCurrentWaypoint()
    {
        if (isPlayerMoving && currentWaypoint != null) return;

        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down); // Cast from above to avoid hitting the player
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, tileLayerMask))
        {
            Waypoint currentCell = hit.collider.GetComponent<Waypoint>();
            if (currentCell != null)
            {
                currentWaypoint = currentCell;
            }
        }
    }

    //private void UpdateAnimatorParameters(Vector3 direction)
    //{
    //    float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
    //    animator.SetFloat("Turn", angle);
    //}
}
