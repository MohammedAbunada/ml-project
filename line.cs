using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line : MonoBehaviour
{
    private float ObstacleDistance = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float Dist = ObstacleDistance;
        
        RaycastHit2D RH = Physics2D.Raycast(transform.position, transform.up - transform.right, ObstacleDistance, 1 << LayerMask.NameToLayer("Obstacle"));
        if (RH.collider != null)
            Dist = RH.distance;
        Debug.DrawLine(transform.position, transform.position + Dist*(transform.up - transform.right).normalized, Color.green);
        Debug.Log((ObstacleDistance - Dist) / ObstacleDistance);

        Dist = ObstacleDistance;
        RH = Physics2D.Raycast(transform.position, transform.up, ObstacleDistance, 1 << LayerMask.NameToLayer("Obstacle"));
        if (RH.collider != null)
            Dist = RH.distance;
        Debug.Log((ObstacleDistance - Dist) / ObstacleDistance);
        Debug.DrawLine(transform.position, transform.position + Dist*(transform.up).normalized, Color.red);

        Dist = ObstacleDistance;
        RH = Physics2D.Raycast(transform.position, transform.up + transform.right, ObstacleDistance, 1 << LayerMask.NameToLayer("Obstacle"));
        if (RH.collider != null)
            Dist = RH.distance;
        Debug.Log((ObstacleDistance - Dist) / ObstacleDistance);
        Debug.DrawLine(transform.position, transform.position + Dist*(transform.up + transform.right).normalized, Color.red);
    }
}
