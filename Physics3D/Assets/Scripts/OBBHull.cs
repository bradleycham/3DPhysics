using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBBHull : Hull3D
{
    public Vector3 halfSize;
    void Start()
    {
        GameObject.Find("CollisionManager").GetComponent<CollisionManager>().AddCollisionHull(this);
        type = CollisionHull3D.hullType.OBB;
    }

    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Gizmos.color = new Color(0, 1, 0, 0.3f); // clear green
        Gizmos.DrawSphere(transform.position + localCenter, boundingVolumeRadius);
    }
}
