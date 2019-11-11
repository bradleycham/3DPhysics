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
        
        Matrix4x4 newMat = GetComponent<Particle3D>().worldToLocalTransform;
        Vector3 thisPos = GetComponent<Particle3D>().position;
        Vector3 testVec = newMat * halfSize;
        

        //testVec = thisPos + testVec;
        testVec = newMat.transpose * testVec;
        testVec += thisPos;
        //testVec = newMat.transpose * testVec;
        //testVec = newMat * testVec;
        //testVec -= thisPos;

        //Debug.Log(testVec.magnitude);
        Gizmos.color = Gizmos.color = new Color(0, 1, 0, 1f); // clear green
        Gizmos.DrawLine(this.transform.position, testVec);
      
    }
}
