using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public class HullCollision
    {
        public struct Contact
        {
            public Vector3 point;
            public Vector3 normal;
            public float restitution;
        }


        public Vector3 closingVelocity;
        public Vector3 penetration;
        public Hull3D a;
        public Hull3D b;
        public Contact[] contacts = new Contact[4];
        public bool status;
        public bool wasStatus;
        public bool resolved = false;
    }
    //PotentialCollision potCol = new PotentialCollision(null, null);
    public List<Hull3D> allColliders = new List<Hull3D>();
    public List<HullCollision> Collisions = new List<HullCollision>();
    //public List<string> currentCollisions;
    //bool collisionHappened;

    void Start()
    {
        //collisionHappened = false;
    }

    private void FixedUpdate()
    {
        VolumeBoundChecks();
        NarrowCollisionCheck();
        Resolution();
        Collisions.Clear();
    }


    void Resolution()
    {
        foreach(HullCollision col in Collisions)
        {
            if(col.status)
                CollisionHull3D.ResolveCollision(col);         
        }
        
        for(int i = 0; i < Collisions.Count; i ++)
        {
            if(Collisions[i].resolved)
            {
                Collisions.RemoveAt(i);
            }
        }
    }

    void VolumeBoundChecks()
    {
        Hull3D hull1;
        Hull3D hull2;
        Vector3 range;
        float radialSum;
        float dot;
        for (int i = 0; i < allColliders.Count; i++)
        {
            allColliders[i].gameObject.GetComponent<Renderer>().material.color = Color.red;
            if (i != allColliders.Count - 1) // if i is not the last collider in the array
                for (int j = i + 1; j < allColliders.Count; j++) // compare it with all colliders after it
                {
                    HullCollision col = new HullCollision();

                    hull1 = allColliders[i];
                    hull2 = allColliders[j];

                    col.a = hull1;
                    col.b = hull2;
                    /*
                    if (hull1.transform.position.magnitude <= hull2.transform.position.magnitude)
                        {
                            col.a = hull1;
                            col.b = hull2;
                        }
                        else
                        {
                            col.a = hull2;
                            col.b = hull1;
                        }
                        */
                        range = (col.b.transform.position - col.a.transform.position);

                        radialSum = hull1.boundingVolumeRadius + hull2.boundingVolumeRadius;
                        if (radialSum * radialSum >= range.sqrMagnitude) // if the radial distance is greater than the actual distance
                        {                            

                            col.closingVelocity = col.a.GetComponent<Particle3D>().velocity - col.b.GetComponent<Particle3D>().velocity;
                            dot = Vector3.Dot(range.normalized, col.closingVelocity.normalized);
                            //if(dot<=0) // maybe remove
                                if (!CheckIfCollisionsContains(hull1, hull2))
                                    Collisions.Add(col);

                        }
                    
                }
        }
    }

    void NarrowCollisionCheck()
    {
        HullCollision col;
        for (int i = 0; i < Collisions.Count; i++)
        {
            col = Collisions[i];
            
            if (col.a.GetHullType() == CollisionHull3D.hullType.Sphere)
            {
                if (col.b.GetHullType() == CollisionHull3D.hullType.Sphere) // sphere sphere
                {
                    CollisionHull3D.SphereSphereCollision(col);
                    if(col.status)
                    {
                        col.a.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        col.b.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
                else if (col.b.GetHullType() == CollisionHull3D.hullType.AABB) // sphere aabb
                {
                    CollisionHull3D.SphereAABBCollision(col);
                    if (col.status)
                    {
                        col.a.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        col.b.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
                else if (col.b.GetHullType() == CollisionHull3D.hullType.OBB) // sphere obb
                {
                    CollisionHull3D.SphereOBBCollision(col);
                    if (col.status)
                    {
                        col.a.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        col.b.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
            }
            else if (col.a.GetHullType() == CollisionHull3D.hullType.AABB)
            {
                if (col.b.GetHullType() == CollisionHull3D.hullType.Sphere) // aabb sphere
                {
                    CollisionHull3D.SphereAABBCollision(col);
                    if (col.status)
                    {
                        col.a.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        col.b.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
                else if (col.b.GetHullType() == CollisionHull3D.hullType.AABB) // aabb aabb
                {
                    CollisionHull3D.AABBAABBCollision(col);
                    if (col.status)
                    {
                        col.a.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        col.b.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
                /*
                else if (col.b.GetHullType() == CollisionHull3D.hullType.OBB) // aabb obb
                {
                    CollisionHull3D.AABBOBBCollision(col);
                    if (col.status)
                    {
                        col.a.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        col.b.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }*/
            }
            else if (col.a.GetHullType() == CollisionHull3D.hullType.OBB) // 
            {
                if (col.b.GetHullType() == CollisionHull3D.hullType.Sphere)
                {
                    CollisionHull3D.SphereOBBCollision(col);
                    if (col.status)
                    {
                        col.a.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        col.b.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
                /*
                else if (col.b.GetHullType() == CollisionHull3D.hullType.AABB)
                {
                    CollisionHull3D.AABBOBBCollision(col);
                    if (col.status)
                    {
                        
                    }
                }
                else if (col.b.GetHullType() == CollisionHull3D.hullType.OBB)
                {
                    //Debug.Log("entered OBBOBB collision checker");
                    CollisionHull3D.OBBOBBCollision(col);
                    if (col.status)
                    {
                       
                    }
                }*/
            }
        }
    }

    bool CheckIfCollisionsContains(Hull3D hull1, Hull3D hull2)
    {
        for (int i = 0; i < Collisions.Count; i++)
        {
            if((Collisions[i].a.name.Equals(hull1.gameObject.name) && Collisions[i].b.gameObject.name.Equals(hull2.name)) || (Collisions[i].a.gameObject.name.Equals(hull2.name) && Collisions[i].b.gameObject.name.Equals(hull1.name)))
            {
                return true;
            }
        }
        return false;
    }

    
    public void AddCollisionHull(Hull3D hull)
    {
        allColliders.Add(hull);
    }
    
    
}
