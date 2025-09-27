using UnityEngine;

public class WaypointContainer : MonoBehaviour
{
    public Vector3[] GetWaypoints()
    {
        Vector3[] waypoints = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            waypoints[i] = transform.GetChild(i).position;
        }
        return waypoints;
    }

    // Визуализация в редакторе
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.DrawWireSphere(transform.GetChild(i).position, 0.3f);
            if (i < transform.childCount - 1)
            {
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
            }
            else
            {
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(0).position);
            }
        }
    }
}