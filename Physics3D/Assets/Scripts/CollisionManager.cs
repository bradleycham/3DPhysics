using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    /*


    //PotentialCollision potCol = new PotentialCollision(null, null);
    public List<CollisionHull3D> allColliders = new List<CollisionHull3D>();
    public List<CollisionHull3D.HullCollision> Collisions = new List<CollisionHull3D.HullCollision>();
    public List<string> currentCollisions;
    public float distanceCheckRadius = 2;
    bool collisionHappened;
    bool respawnAsteroids;
    public float asteroidAmount;
    public GameObject largeRect;
    public GameObject largeCircle;
    void Start()
    {
        collisionHappened = false;
    }

// Update is called once per frame
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
                        collisionHappened = false;
                    }
                }  
            }
        }
        //collisionResolution

        Collisions.Clear();
    }

    public void AddCollisionHull(CollisionHull3D hull)
    {
        allColliders.Add(hull);
    }
    */
}
