﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHull3D : MonoBehaviour
{
   
    public float restitution;
   
    public enum hullType
    {
        AABB,
        OBB,
        Sphere
    }

    //public hullType hull;

   /*

    public static void ResolveCollision(HullCollision col)
    {
        Particle3D A = col.a.GetComponent<Particle3D>();
        Particle3D B = col.b.GetComponent<Particle3D>();
        float invAMass;
        float invBMass;
        if (A.mass == 0) invAMass = 0;
        else invAMass = 1 / A.mass;
        if (B.mass == 0) invBMass = 0;
        else invBMass = 1 / B.mass;


        float velAlongNormal = Vector3.Dot(col.closingVelocity, col.contacts[0].normal);
        //Debug.Log("velAlongNormal " + velAlongNormal);

        if (velAlongNormal > 0) return; // > makes square work properly
        //Debug.Log(velAlongNormal);
        // restitustion
        float e = col.contacts[0].restitution;
        // impulse scalar
        float j = -(1 + e) * velAlongNormal;
        j /= invAMass + invBMass;
        //Debug.Log(j);

        Vector3 impulse = j * col.contacts[0].normal;
        //Debug.Log(impulse);

        //A.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        //B.velocity = new Vector3(0.0f, 0.0f, 0.0f);

        A.velocity -= invAMass * impulse;
        B.velocity += invBMass * impulse;

        // Positional Correction
        if (col.status)
        {
            float percent = 0.2f;
            float slop = 0.01f;
            Vector3 correction = Mathf.Max(velAlongNormal - slop, 0) / (invAMass + invBMass) * percent * col.contacts[0].normal;
            A.position += invAMass * correction; // started -
            B.position -= invBMass * correction; // started +
        }


    }
    */
    public static void SphereSphereCollision(CollisionManager.HullCollision col)
    {
        // *IMPORTANT* for circle and square the collision only wirks with obejct1 - object 2 and not viceversa, must be a prob in clollision resolution
        SphereHull hull1 = col.a.GetComponent<SphereHull>();
        SphereHull hull2 = col.b.GetComponent<SphereHull>();
        Vector3 range = (hull2.transform.position + hull2.localCenter) - (hull1.transform.position + hull1.localCenter); // make sure offsets arent screwing things up
        float overlap = (hull2.radius + hull1.radius) - range.magnitude;

        //HullCollision col = new CollisionManager.HullCollision();
        //col.a = hull1;
        //col.b = hull2;
        col.penetration = range * overlap;

        CollisionManager.HullCollision.Contact con0 = new CollisionManager.HullCollision.Contact();
        con0.point = (range.normalized * hull1.radius) + hull1.transform.position;
        con0.normal = range.normalized;
        con0.restitution = Mathf.Min(hull1.restitution, hull2.restitution);

        col.contacts[0] = con0;

        Particle3D c1 = hull1.GetComponentInParent<Particle3D>();
        Particle3D c2 = hull2.GetComponentInParent<Particle3D>();

        Vector3 closingVel = c2.velocity - c1.velocity; // started as c1 -c2

        col.closingVelocity = closingVel;

        if (overlap >= 0)
        {
            col.status = true;
           //Debug.Log("touch");
        }
        else
        {
            col.status = false;
        }
    }
    
    public static void SphereAABBCollision(CollisionManager.HullCollision collision)
    {
        AABBHull boxHull;
        SphereHull sphere;
        if (collision.a.GetHullType() == hullType.AABB)
        {
            boxHull = collision.a.GetComponent<AABBHull>();
            sphere = collision.b.GetComponent<SphereHull>();
        }
        else
        {
            boxHull = collision.b.GetComponent<AABBHull>();
            sphere = collision.a.GetComponent<SphereHull>();
        }
        
        Vector3 closestPoint;
        Vector3 range = (boxHull.transform.position + boxHull.localCenter) - (sphere.transform.position + sphere.localCenter);
        

        closestPoint = new Vector3(Mathf.Clamp(range.x, -boxHull.halfSize.x, boxHull.halfSize.x), Mathf.Clamp(range.y, -boxHull.halfSize.y, boxHull.halfSize.y), Mathf.Clamp(range.z, -boxHull.halfSize.z, boxHull.halfSize.z));

        Vector3 closingVel = sphere.GetComponent<Particle3D>().velocity - boxHull.GetComponent<Particle3D>().velocity;
        Vector3 penetration = range - closestPoint;
        collision.closingVelocity = closingVel;
        collision.penetration = penetration;

        CollisionManager.HullCollision.Contact con0 = new CollisionManager.HullCollision.Contact();
        con0.point = closestPoint;
        con0.restitution = Mathf.Min(boxHull.restitution, sphere.restitution);

        Vector3 collisionNormal = new Vector3();


        if (penetration.magnitude <= sphere.radius)
        {
            if (con0.point.x == boxHull.halfSize.x)//added mathf
                collisionNormal = new Vector3(1.0f, 0.0f, 0.0f);
            if (con0.point.x == -boxHull.halfSize.x)//added mathf
                collisionNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            if (con0.point.y == boxHull.halfSize.y)
                collisionNormal = new Vector3(0.0f, 1.0f, 0.0f);
            if (con0.point.y == -boxHull.halfSize.y)
                collisionNormal = new Vector3(0.0f, -1.0f, 0.0f);
            if (con0.point.y == boxHull.halfSize.y)
                collisionNormal = new Vector3(0.0f, 0.0f, 1.0f);
            if (con0.point.y == -boxHull.halfSize.y)
                collisionNormal = new Vector3(0.0f, 0.0f, -1.0f);

            con0.normal = collisionNormal;

            collision.status = true;
            collision.contacts[0] = con0;
        }
        else collision.status = false; 
    }
    /*
    public static HullCollision SphereOBBCollision(SphereHull sphereHull, OBBHull OBBHull)
    { 
        Particle3D A = sphereHull.GetComponent<Particle3D>();
        Particle3D B = OBBHull.GetComponent<Particle3D>();
        Vector3[] OBBCorners;

        OBBCorners = new Vector3[2];//was 4
        Vector3[] normals = new Vector3[2];
        float[] OBBMinMax = new float[2];
        float[] sphereMinMax = new float[2];

        OBBCorners = getRotatedCorners(OBBHull);

        normals[0] = getUpNormal(-OBBHull.currentRotation);
        normals[1] = getRightNormal(-OBBHull.currentRotation);
        //normals[2] = getUpNormal(-OBBHull2.currentRotation);
        //normals[3] = getRightNormal(-boxHull2.currentRotation);

        HullCollision col = new HullCollision();
        col.a = sphereHull;
        col.b = OBBHull;
        Vector3 range = (OBBHull.transform.position + OBBHull.offset) - (sphereHull.transform.position + sphereHull.offset);

        Vector3 rotatedRange = getRotatedPoint(range, new Vector3 (0.0f,0.0f), -OBBHull.currentRotation);// 2 circleHull.transform.position
        Vector3 point = new Vector3(Mathf.Clamp(rotatedRange.x, -OBBHull.halfX, OBBHull.halfX), Mathf.Clamp(rotatedRange.y, -OBBHull.halfY, OBBHull.halfY));
        //Debug.Log("range " + range);
        //Debug.Log("rotrange " + rotatedRange);

        //float xOverlap = boxHull1.halfX + boxHull2.halfX - Mathf.Abs(range.x);
        //float yOverlap = boxHull1.halfY + boxHull2.halfY - Mathf.Abs(range.y);

        //col.penetration = new Vector3(xOverlap, yOverlap);

        Vector3 closingVel = B.velocity - A.velocity;
        col.closingVelocity = closingVel;

        HullCollision.Contact con0 = new HullCollision.Contact();
        con0.point = new Vector3(Mathf.Clamp(range.x, -OBBHull.halfX, OBBHull.halfX), Mathf.Clamp(range.y, -OBBHull.halfY, OBBHull.halfY));
        con0.restitution = Mathf.Min(OBBHull.restitution, sphereHull.restitution);
        con0.normal = range.normalized;
        //Debug.Log("point " + point);

        col.status = false;
        if ((rotatedRange - point).magnitude - sphereHull.radius < 0 )
        {
            col.status = true;
            col.contacts[0] = con0;
        }
        return col;
    }
    */
    public static void AABBAABBCollision(CollisionManager.HullCollision col)
    {
        Vector3 min0, max0, min1, max1;

        AABBHull A = col.a.GetComponent<AABBHull>();
        AABBHull B = col.b.GetComponent<AABBHull>();

        min0 = A.transform.position - A.halfSize + A.localCenter;
        max0 = A.transform.position + A.halfSize + A.localCenter;
        min1 = B.transform.position - B.halfSize + B.localCenter;
        max1 = B.transform.position + B.halfSize + B.localCenter;

        Vector3 range = (B.transform.position + B.localCenter) - (A.transform.position + A.localCenter); // make sure offsets arent screwing things up

        //HullCollision col = new HullCollision();
        //col.a = boxHull1;
        //col.b = boxHull2;

        float xOverlap = A.halfSize.x + B.halfSize.x - Mathf.Abs(range.x);
        float yOverlap = A.halfSize.y + B.halfSize.y - Mathf.Abs(range.y);
        float zOverlap = A.halfSize.z + B.halfSize.z - Mathf.Abs(range.z);

        col.penetration = new Vector3(xOverlap, yOverlap);

        //Vector3 closingVel = A.velocity - B.velocity;
        Vector3 closingVel = B.GetComponent<Particle3D>().velocity - A.GetComponent<Particle3D>().velocity;
        col.closingVelocity = closingVel;

        CollisionManager.HullCollision.Contact con0 = new CollisionManager.HullCollision.Contact();
        con0.point = new Vector3(Mathf.Clamp(range.x, -A.halfSize.x, A.halfSize.x), Mathf.Clamp(range.y, -A.halfSize.y, A.halfSize.y), Mathf.Clamp(range.z, -A.halfSize.z, A.halfSize.z));
        con0.restitution = Mathf.Min(A.restitution, B.restitution);
        
        if (max0.x >= min1.x && max1.x >= min0.x)
        {
            if (max0.y >= min1.y && max1.y >= min0.y)
            {
                Vector3 collisionNormal = new Vector3();

                if (con0.point.x == A.halfSize.x)//added mathf
                    collisionNormal = new Vector3(1.0f, 0.0f, 0.0f);
                if (con0.point.x == -A.halfSize.x)//added mathf
                    collisionNormal = new Vector3(-1.0f, 0.0f, 0.0f);
                if (con0.point.y == A.halfSize.y)
                    collisionNormal = new Vector3(0.0f, 1.0f, 0.0f);
                if (con0.point.y == -A.halfSize.y)
                    collisionNormal = new Vector3(0.0f, -1.0f, 0.0f);
                if (con0.point.z == A.halfSize.z)
                    collisionNormal = new Vector3(0.0f, 0.0f, 1.0f);
                if (con0.point.z == -A.halfSize.z)
                    collisionNormal = new Vector3(0.0f, 0.0f, -1.0f);

                con0.normal = collisionNormal;

                col.status = true;
            }
        }    
        else col.status = false;
        col.contacts[0] = con0;
 
    }
    /*
    public static HullCollision AABBOBBCollision(AABBHull AABBHull, OBBHull OBBHull)
    {
        Particle3D A = AABBHull.GetComponent<Particle3D>();
        Particle3D B = OBBHull.GetComponent<Particle3D>();
        Vector3[] shape1Corners;
        Vector3[] shape2Corners;
        shape1Corners = new Vector3[4];
        shape2Corners = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        float[] shape1MinMax = new float[2];
        float[] shape2MinMax = new float[2];

        shape1Corners[0] = AABBHull.transform.position - new Vector3(AABBHull.halfX, AABBHull.halfY) + AABBHull.offset;
        shape1Corners[1] = AABBHull.transform.position - new Vector3(AABBHull.halfX, -AABBHull.halfY) + AABBHull.offset;
        shape1Corners[2] = AABBHull.transform.position + new Vector3(AABBHull.halfX, AABBHull.halfY) + AABBHull.offset;
        shape1Corners[3] = AABBHull.transform.position + new Vector3(AABBHull.halfX, -AABBHull.halfY) + AABBHull.offset;
        shape2Corners = getRotatedCorners(OBBHull);

        normals[0] = new Vector3(0.0f, 1.0f, 0.0f);
        normals[1] = new Vector3(1.0f, 0.0f, 0.0f);
        normals[2] = getUpNormal(-OBBHull.currentRotation);
        normals[3] = getRightNormal(-OBBHull.currentRotation);

        HullCollision col = new HullCollision();
        col.a = AABBHull;
        col.b = OBBHull;
        Vector3 range = (OBBHull.transform.position + OBBHull.offset) - (AABBHull.transform.position + AABBHull.offset);
        //float xOverlap = boxHull1.halfX + boxHull2.halfX - Mathf.Abs(range.x);
        //float yOverlap = boxHull1.halfY + boxHull2.halfY - Mathf.Abs(range.y);

        //TRANSPORTATION

        //col.penetration = new Vector3(xOverlap, yOverlap);

        //Vector3 closingVel = A.velocity - B.velocity;
        Vector3 closingVel = B.velocity - A.velocity;
        col.closingVelocity = closingVel;

        HullCollision.Contact con0 = new HullCollision.Contact();
        con0.point = new Vector3(Mathf.Clamp(range.x, -AABBHull.halfX, AABBHull.halfX), Mathf.Clamp(range.y, -AABBHull.halfY, AABBHull.halfY));
        con0.restitution = Mathf.Min(AABBHull.restitution, OBBHull.restitution);
        con0.normal = range.normalized;

        for (int i = 0; i < normals.Length; i++)
        {
            //Debug.Log("testing corner" + i);

            shape1MinMax = SatTest(normals[i], shape1Corners);
            shape2MinMax = SatTest(normals[i], shape2Corners);
            if (!Overlap(shape1MinMax[0], shape1MinMax[1], shape2MinMax[0], shape2MinMax[1]))
            {
                //Debug.Log("falure");
                col.status = false;
                return col;
            }
        }
        col.status = true;
        col.contacts[0] = con0;
        return col;
    }
    */
    public static void OBBOBBCollision(CollisionManager.HullCollision collision)
    {
        OBBHull A = collision.a.GetComponent<OBBHull>();
        OBBHull B = collision.b.GetComponent<OBBHull>();
        Particle3D aParticle = collision.a.GetComponent<Particle3D>();
        Particle3D bParticle = collision.b.GetComponent<Particle3D>();
        Vector3[] shape1Corners;
        Vector3[] shape2Corners;
        shape1Corners = new Vector3[8];
        shape2Corners = new Vector3[8];
        Vector3 rotPosition = aParticle.worldToLocalTransform.transpose * A.halfSize;
        Vector3 corner1 = aParticle.localToWorldTransform * new Vector3(A.halfSize.x, A.halfSize.y, A.halfSize.z);
        
        Vector3 corner2 = aParticle.worldToLocalTransform.transpose * new Vector3(A.halfSize.x, -A.halfSize.y, A.halfSize.z);
        Vector3 corner3 = aParticle.worldToLocalTransform.transpose * new Vector3(A.halfSize.x, A.halfSize.y, -A.halfSize.z);
        Vector3 corner4 = aParticle.worldToLocalTransform.transpose * new Vector3(A.halfSize.x, -A.halfSize.y, A.halfSize.z);
        Vector3 corner5 = aParticle.worldToLocalTransform.transpose * new Vector3(-A.halfSize.x, A.halfSize.y, -A.halfSize.z);
        Vector3 corner6 = aParticle.worldToLocalTransform.transpose * new Vector3(-A.halfSize.x, -A.halfSize.y, A.halfSize.z);
        Vector3 corner7 = aParticle.worldToLocalTransform.transpose * new Vector3(-A.halfSize.x, A.halfSize.y, -A.halfSize.z);
        Vector3 corner8 = aParticle.worldToLocalTransform.transpose * new Vector3(-A.halfSize.x, -A.halfSize.y, -A.halfSize.z);
        //Debug.Log(corner1);
        Vector3[] normals = new Vector3[6];
        float[] shape1MinMax = new float[2];
        float[] shape2MinMax = new float[2];

        
    }
    /*
    public static void OBBOBBCollision(OBBHull boxHull1, OBBHull boxHull2)
    {
        Particle3D A = boxHull1.GetComponent<Particle3D>();
        Particle3D B = boxHull2.GetComponent<Particle3D>();
        Vector3[] shape1Corners;
        Vector3[] shape2Corners;
        shape1Corners = new Vector3[4];
        shape2Corners = new Vector3[4];
        Vector3[] normals = new Vector3[6];
        float[] shape1MinMax = new float[2];
        float[] shape2MinMax = new float[2];

        shape1Corners = getRotatedCorners(boxHull1);
        shape2Corners = getRotatedCorners(boxHull2);

        normals[0] = getUpNormal(-boxHull1.currentRotation);
        normals[1] = getRightNormal(-boxHull1.currentRotation);
        normals[2] = getUpNormal(-boxHull2.currentRotation);
        normals[3] = getRightNormal(-boxHull2.currentRotation);

        HullCollision col = new HullCollision();
        col.a = boxHull1;
        col.b = boxHull2;
        Vector3 range = (boxHull2.transform.position + boxHull2.offset) - (boxHull1.transform.position + boxHull1.offset);
        //float xOverlap = boxHull1.halfX + boxHull2.halfX - Mathf.Abs(range.x);
        //float yOverlap = boxHull1.halfY + boxHull2.halfY - Mathf.Abs(range.y);

        //TRANSPORTATION

        //col.penetration = new Vector3(xOverlap, yOverlap);

        //Vector3 closingVel = A.velocity - B.velocity;
        Vector3 closingVel = B.velocity - A.velocity;
        col.closingVelocity = closingVel;

        HullCollision.Contact con0 = new HullCollision.Contact();
        con0.point = new Vector3(Mathf.Clamp(range.x, -boxHull1.halfX, boxHull1.halfX), Mathf.Clamp(range.y, -boxHull1.halfY, boxHull1.halfY));
        con0.restitution = Mathf.Min(boxHull1.restitution, boxHull2.restitution);
        con0.normal = range.normalized;

        for (int i = 0; i < normals.Length; i ++ )
        {
            //Debug.Log("testing corner" + i);

            shape1MinMax = SatTest(normals[i], shape1Corners);
            shape2MinMax = SatTest(normals[i], shape2Corners);
            if (!Overlap(shape1MinMax[0], shape1MinMax[1], shape2MinMax[0], shape2MinMax[1]))
            {
                //Debug.Log("falure");
                col.status = false;
                return col;

            }
        }
        col.status = true;
        col.contacts[0] = con0;
        return col;
    }
    */

    static Vector3 getUpNormal(float theta)
    {
        float rad = theta * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), -Mathf.Sin(rad));
    }

    static Vector3 getRightNormal(float theta)
    {
        float rad = theta * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(rad), Mathf.Cos(rad));
    }

    static float[] SatTest(Vector3 axis, Vector3[] points)
    {
        float[] minMax = new float[2];
        float minAlong = 1000000; float maxAlong = -1000000;
        for(int i = 0; i < points.Length; i++)
        {
            float dotValue = Vector3.Dot(points[i], axis);
            if (dotValue < minAlong) minAlong = dotValue;
            if (dotValue > maxAlong) maxAlong = dotValue;
        }
        minMax[0] = minAlong;
        minMax[1] = maxAlong;
        //Debug.Log(minMax[0] + " " + minMax[1]);
        return minMax;
    }
    /*
    static Vector3[] getRotatedCorners(OBBHull newHull)
    {
        Vector3[] returnPoints = new Vector3[4];
        returnPoints[0] = getRotatedPoint(new Vector3(newHull.transform.position.x - newHull.halfX, newHull.transform.position.y - newHull.halfY), newHull.transform.position, newHull.currentRotation);
        returnPoints[1] = getRotatedPoint(new Vector3(newHull.transform.position.x - newHull.halfX, newHull.transform.position.y + newHull.halfY), newHull.transform.position, newHull.currentRotation);
        returnPoints[2] = getRotatedPoint(new Vector3(newHull.transform.position.x + newHull.halfX, newHull.transform.position.y - newHull.halfY), newHull.transform.position, newHull.currentRotation);
        returnPoints[3] = getRotatedPoint(new Vector3(newHull.transform.position.x + newHull.halfX, newHull.transform.position.y + newHull.halfY), newHull.transform.position, newHull.currentRotation);

        return returnPoints;
    }
    */
    public static Vector3 getRotatedPoint(Vector3 cornerPos, Vector3 centerPos, float theta)
    {
        float rad = theta * Mathf.Deg2Rad;

        float xPos = cornerPos.x - centerPos.x;
        float yPos = cornerPos.y - centerPos.y;
        float xRot = (xPos * Mathf.Cos(rad)) - (yPos * Mathf.Sin(rad));
        float yRot = (xPos * Mathf.Sin(rad)) + (yPos * Mathf.Cos(rad));
     
        Vector3 returnVector = new Vector3(xRot, yRot);

        returnVector += centerPos;
       
        return returnVector;
    }

    static bool Overlap(float min1, float max1, float min2, float max2)
    {
        return IsBetweenOrdered(min2, min1, max1) || IsBetweenOrdered(min1, min2, max2);
    }
    static bool IsBetweenOrdered(float val, float lowerBound, float upperBound)
    {
        return lowerBound <= val && val <= upperBound;
    }

   
}
