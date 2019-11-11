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

    private void Update()
    {
        VolumeBoundChecks();
        NarrowCollisionCheck();
        //Debug.Log(Collisions.Count);
        Collisions.Clear();

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
                    range = (hull2.transform.position - hull1.transform.position);

                    if (!CheckIfCollisionsContains(hull1, hull2))
                    {
                        radialSum = hull1.boundingVolumeRadius + hull2.boundingVolumeRadius;
                        if (radialSum >= range.magnitude) // if the radial distance is greater than the actual distance
                        {

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

                            //col.a = hull1;
                            //col.b = hull2;

                            col.closingVelocity = hull2.GetComponent<Particle3D>().velocity - hull1.GetComponent<Particle3D>().velocity;
                            dot = Vector3.Dot(range.normalized, col.closingVelocity.normalized);
                            if(dot<=0) // maybe remove
                                Collisions.Add(col);

                        }
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
                else if (col.b.GetHullType() == CollisionHull3D.hullType.OBB) // aabb obb
                {

                }
            }
            else if (col.a.GetHullType() == CollisionHull3D.hullType.OBB) // 
            {
                if (col.b.GetHullType() == CollisionHull3D.hullType.Sphere)
                {

                }
                else if (col.b.GetHullType() == CollisionHull3D.hullType.AABB)
                {

                }
                else if (col.b.GetHullType() == CollisionHull3D.hullType.OBB)
                {
                    CollisionHull3D.OBBOBBCollision(col);
                    if (col.status)
                    {
                        col.a.gameObject.GetComponent<Renderer>().material.color = Color.green;
                        col.b.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }
            }
        }
    }

    bool CheckIfCollisionsContains(Hull3D hull1, Hull3D hull2)
    {
        for (int i = 0; i < Collisions.Count; i++)
        {
            if((Collisions[i].a.name.Equals(hull1.gameObject.name) && Collisions[i].b.gameObject.name.Equals(hull2.name)) || Collisions[i].a.gameObject.name.Equals(hull2.name) && Collisions[i].b.gameObject.name.Equals(hull1.name))
            {
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    /*
    void Update()
        {
            respawnAsteroids = true;
            Debug.Log(allColliders.Count);
            for(int spaghetti = 0; spaghetti < allColliders.Count; spaghetti ++)
            {
                if (allColliders[spaghetti] == null)
                {
                    allColliders.RemoveAt(spaghetti);
                    spaghetti--;
                }
                else if (allColliders[spaghetti].gameObject.tag == "Asteroid")
                    respawnAsteroids = false;
                else allColliders[spaghetti].gameObject.GetComponent<Renderer>().material.color = Color.red;        

            }

            if(respawnAsteroids)
            {
                Debug.Log("destroyed all asteroids");
                for (int i = 0; i < asteroidAmount; i++)
                {
                    float type = Random.Range(0, 2);
                    float loc = Random.Range(0, 4);
                    Vector2 newPosOnBorder = new Vector2();
                    if (loc == 0)
                        newPosOnBorder = new Vector2(Screen.width, Random.Range(-Screen.height, Screen.height));
                    if (loc == 1)
                        newPosOnBorder = new Vector2(-Screen.width, Random.Range(-Screen.height, Screen.height));
                    if (loc == 2)
                        newPosOnBorder = new Vector2(Random.Range(-Screen.width, Screen.width), Screen.height);
                    if (loc == 3)
                        newPosOnBorder = new Vector2(Random.Range(-Screen.width, Screen.width), -Screen.height);

                    if (type == 0)
                    {
                        Instantiate(largeCircle, newPosOnBorder, Quaternion.identity);
                    }
                    if (type == 1)
                    {
                        Instantiate(largeRect, newPosOnBorder, Quaternion.identity);
                    }
                }
            }

            for (int i = 0; i < allColliders.Count; i ++)
            {
                for (int j = 0; j < allColliders.Count; j++)
                {
                    if (i != j && allColliders[i].gameObject != allColliders[j].gameObject)
                    {
                        Vector3 range = allColliders[j].transform.position - allColliders[i].transform.position;
                        if (range.magnitude - (distanceCheckRadius * 2) < 0)
                        {
                            // alternatively you could just add the collisions to a list and operate on them in another loop
                            CollisionHull2D.HullCollision newCollision = new CollisionHull2D.HullCollision();

                            if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.CIRCLE)
                            {
                                newCollision = CollisionHull2D.CircleCircleCollision(allColliders[i].GetComponent<CircleHull>(), allColliders[j].GetComponent<CircleHull>());
                                collisionHappened = newCollision.status;    
                            }

                            if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.AABB)
                            {
                                newCollision = CollisionHull2D.CircleAABBCollision(allColliders[i].GetComponent<CircleHull>(), allColliders[j].GetComponent<AABBHull>());
                                collisionHappened = newCollision.status;
                                //Debug.Log(newCollision.contacts[0].normal);
                                //Debug.Log(newCollision.status);
                                //Debug.Log(newCollision.contacts[0].normal);
                                //Debug.Log(newCollision.contacts[0].point);
                                //Debug.Log(newCollision.closingVelocity);
                            }
                            if (allColliders[i].hull == CollisionHull2D.hullType.CIRCLE && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            {
                                newCollision = CollisionHull2D.CircleOBBCollision(allColliders[i].GetComponent<CircleHull>(), allColliders[j].GetComponent<OBBHull>());
                                collisionHappened = newCollision.status;
                            }
                            if (allColliders[i].hull == CollisionHull2D.hullType.AABB && allColliders[j].hull == CollisionHull2D.hullType.AABB)
                            {
                                newCollision = CollisionHull2D.AABBAABBCollision(allColliders[i].GetComponent<AABBHull>(), allColliders[j].GetComponent<AABBHull>());
                                collisionHappened = newCollision.status;
                            }
                            if (allColliders[i].hull == CollisionHull2D.hullType.AABB && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            {
                                newCollision = CollisionHull2D.AABBOBBCollision(allColliders[i].GetComponent<AABBHull>(), allColliders[j].GetComponent<OBBHull>());
                                collisionHappened = newCollision.status;
                                //Debug.Log(newCollision.contacts[0].normal);
                                Debug.Log(newCollision.status);
                                Debug.Log(newCollision.closingVelocity);
                                //Debug.Log(newCollision.contacts[0].point);
                                //Debug.Log(newCollision.closingVelocity);
                            }
                            if (allColliders[i].hull == CollisionHull2D.hullType.OBB && allColliders[j].hull == CollisionHull2D.hullType.OBB)
                            {
                                newCollision = CollisionHull2D.OBBOBBCollision(allColliders[i].GetComponent<OBBHull>(), allColliders[j].GetComponent<OBBHull>());
                                collisionHappened = newCollision.status;
                            }


                            if (collisionHappened)
                            {
                                if(newCollision.status)
                                {
                                    //currentCollisions.Add();
                                    bool duplicate = false;
                                    for(int h = 0; h < Collisions.Count; h++)
                                    {
                                        if ((newCollision.a == Collisions[h].a || newCollision.a == Collisions[h].b) && (newCollision.b == Collisions[h].a || newCollision.b == Collisions[h].b))
                                        {
                                            duplicate = true;

                                        }
                                    }
                                    if(!duplicate)
                                    {
                                        Collisions.Add(newCollision);
                                    }
                                }

                                allColliders[i].gameObject.GetComponent<Renderer>().material.color = Color.green;
                                allColliders[j].gameObject.GetComponent<Renderer>().material.color = Color.green;

                            }
                            //collisionHappened = false;
                        }
                    }  
                }
            }
            //collisionResolution

            Collisions.Clear();
        }
        */
    public void AddCollisionHull(Hull3D hull)
    {
        allColliders.Add(hull);
    }
    
    
}
