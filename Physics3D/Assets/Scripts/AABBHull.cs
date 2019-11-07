using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABBHull : CollisionHull3D
{
    // Start is called before the first frame update
    public Vector3 offset;
    public float halfX;
    public float halfY;
    public float halfZ;
    private void Start()
    {
        hull = hullType.AABB;
        //GameObject.Find("CollisionManager").GetComponent<CollisionManager>().AddCollisionHull(this);
    }
    Vector2 getMinCorner()
    {
        return this.transform.position - new Vector3(halfX, halfY, halfZ) + offset;
    }

    Vector2 getMaxCorner()
    {
        return this.transform.position + new Vector3(halfX, halfY, halfZ) + offset;
    }
}
