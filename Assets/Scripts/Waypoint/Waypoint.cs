

public class Waypoint : GridCell
{
    public bool IsExplored { get; set; } = false; // visited or not
    public Waypoint ExploredFrom { get; set; }

    private void Start()
    {
        WaypointManager.Instance.RegisterWaypoint(this);
    }
}
