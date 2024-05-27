using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoSingletonGeneric<WaypointManager>
{
    public List<Waypoint> Waypoints => waypoints;

    private List<Waypoint> waypoints = new List<Waypoint>();

    public void RegisterWaypoint(Waypoint waypoint)
    {
        waypoints.Add(waypoint);
    }
}
