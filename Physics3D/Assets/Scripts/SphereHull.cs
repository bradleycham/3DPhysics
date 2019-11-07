using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereHull : CollisionHull3D
{
    // Start is called before the first frame update
    public float radius;
    //public float restitution;
    public Vector3 offset;

    void Start()
    {
        hull = hullType.Sphere;
        //GameObject.Find("CollisionManager").GetComponent<CollisionManager>().AddCollisionHull(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
