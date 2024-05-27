using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
[RequireComponent(typeof(Waypoint))]
public class WaypointEditor : MonoBehaviour
{
    private Waypoint waypoint;
    private TMP_Text labelText;

    private void Awake()
    {
        waypoint = GetComponent<Waypoint>();
        labelText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        SnapToGridPosition();
        UpdateCoordinatesLabel();
    }

    private void SnapToGridPosition()
    {
        int gridSize = waypoint.GridCellSize;
        transform.position = new Vector3(waypoint.GridPosition.x * gridSize, 0, waypoint.GridPosition.y * gridSize);
    }

    private void UpdateCoordinatesLabel()
    {
        if (labelText)
            UpdateLabel();
        else
            Debug.LogError("label text is null");
    }

    private void UpdateLabel()
    {
        string label = waypoint.GridPosition.x + "," + waypoint.GridPosition.y;
        labelText.text = label;
        gameObject.name = label;
    }
}
