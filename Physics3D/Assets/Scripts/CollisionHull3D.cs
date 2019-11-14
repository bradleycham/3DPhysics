using System.Collections;
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
    public static void SphereOBBCollision(CollisionManager.HullCollision col)
    {
        
        Particle3D sphereParticle;
        Particle3D boxParticle;
        if (col.a.GetComponent<Hull3D>().GetHullType() == CollisionHull3D.hullType.Sphere)
        {
            sphereParticle = col.a.GetComponent<Particle3D>();
            boxParticle = col.b.GetComponent<Particle3D>();
        }
        else
        {
            sphereParticle = col.b.GetComponent<Particle3D>();
            boxParticle = col.a.GetComponent<Particle3D>();
        }

        Vector3 closestPoint;
        Vector3 localRange = (boxParticle.position) - (boxParticle.GetLocalToWorldtransform(false).MultiplyPoint(sphereParticle.position));
        Vector3 range = (boxParticle.position) - (sphereParticle.position);


        closestPoint = new Vector3(Mathf.Clamp(localRange.x, -boxParticle.GetComponent<OBBHull>().halfSize.x, boxParticle.GetComponent<OBBHull>().halfSize.x),
                                   Mathf.Clamp(localRange.y, -boxParticle.GetComponent<OBBHull>().halfSize.y, boxParticle.GetComponent<OBBHull>().halfSize.y),
                                   Mathf.Clamp(localRange.z, -boxParticle.GetComponent<OBBHull>().halfSize.z, boxParticle.GetComponent<OBBHull>().halfSize.z));

       
        Vector3 closingVel = sphereParticle.GetComponent<Particle3D>().velocity - boxParticle.GetComponent<Particle3D>().velocity;
        Vector3 penetration = range - closestPoint;

        Debug.DrawLine(new Vector3(), range);
        if (Mathf.Abs(penetration.magnitude) <= sphereParticle.GetComponent<SphereHull>().radius)
        {
            col.status = true;
        }
        else col.status = false;

    }
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
    public static void AABBOBBCollision(CollisionManager.HullCollision collision)
    {
        Matrix4x4 transformatA;
        Matrix4x4 transformatB;
        bool flip = false;
        if (collision.a.GetComponent<Hull3D>().GetHullType() == CollisionHull3D.hullType.AABB)
        {
            transformatA = collision.a.GetComponent<Particle3D>().GetLocalToWorldtransform(false);
            transformatB = collision.b.GetComponent<Particle3D>().GetLocalToWorldtransform(false);
        }
        else
        {
            transformatA = collision.b.GetComponent<Particle3D>().GetLocalToWorldtransform(false);
            transformatB = collision.a.GetComponent<Particle3D>().GetLocalToWorldtransform(false);
            flip = true;
        }
        

        Vector3 toCentre = collision.b.GetComponent<Particle3D>().position - collision.a.GetComponent<Particle3D>().position;

        List<Vector3> axis = new List<Vector3>();
        axis.Add(transformatB.GetColumn(0));
        axis.Add(transformatB.GetColumn(1));
        axis.Add(transformatB.GetColumn(2));

        float bestOverlap = float.MaxValue;
        int bestIndex;
        bool colStatus = true;
        for (int i = 0; i < axis.Count; i++)
        {
            if (axis[i].sqrMagnitude < 0.001f) continue;
            axis[i].Normalize();
            float overlap = -1f;
            if(!flip)
            {
              overlap = OverlapOnAxis(collision.a, collision.b, collision.a.GetComponent<AABBHull>().halfSize, collision.b.GetComponent<OBBHull>().halfSize, axis[i], toCentre);
            }
            if (flip)
            {
                overlap = OverlapOnAxis(collision.b, collision.a, collision.b.GetComponent<AABBHull>().halfSize, collision.a.GetComponent<OBBHull>().halfSize, axis[i], toCentre);
            }
            if (overlap < 0)
            {
                colStatus = false;
                return;
            }

            if (overlap < bestOverlap)
            {
                bestOverlap = overlap;
                bestIndex = i;
            }
        }
        collision.status = colStatus;
    }


    static float TransformToAxis(Hull3D box, Vector3 halfsize, Vector3 axis)
    {
        Matrix4x4 boxTrans = box.GetComponent<Particle3D>().GetLocalToWorldtransform(false);
        return
            halfsize.x * Mathf.Abs(Vector3.Dot(boxTrans.GetColumn(0), (Vector3)axis)) +
            halfsize.y * Mathf.Abs(Vector3.Dot(boxTrans.GetColumn(1), (Vector3)axis)) +
            halfsize.z * Mathf.Abs(Vector3.Dot(boxTrans.GetColumn(2), (Vector3)axis));
    }
    static float OverlapOnAxis(Hull3D one, Hull3D two, Vector3 oneHalfSize, Vector3 twoHalfSize, Vector3 axis, Vector3 toCentre)
    {
    // Project the half-size of one onto axis
    float oneProject = TransformToAxis(one, oneHalfSize, axis);
    float twoProject = TransformToAxis(two, twoHalfSize, axis);

    // Project this onto the axis
    float distance = Mathf.Abs(Vector3.Dot(toCentre, axis));
    float overlap = (oneProject + twoProject) - distance;
    // Check for overlap
    return overlap;
}
    public static void OBBOBBCollision(CollisionManager.HullCollision collision)
    {
        Matrix4x4 transformatA = collision.a.GetComponent<Particle3D>().GetLocalToWorldtransform(false);
        Matrix4x4 transformatB = collision.b.GetComponent<Particle3D>().GetLocalToWorldtransform(false);

        Vector3 toCentre = collision.b.GetComponent<Particle3D>().position - collision.a.GetComponent<Particle3D>().position;

        List<Vector3> axis = new List<Vector3>();
        axis.Add(transformatA.GetColumn(0));
        axis.Add(transformatA.GetColumn(1));
        axis.Add(transformatA.GetColumn(2));

        axis.Add(transformatB.GetColumn(0));
        axis.Add(transformatB.GetColumn(1));
        axis.Add(transformatB.GetColumn(2));

        axis.Add(Vector3.Cross(transformatA.GetColumn(0), transformatB.GetColumn(0)));
        axis.Add(Vector3.Cross(transformatA.GetColumn(0), transformatB.GetColumn(1)));
        axis.Add(Vector3.Cross(transformatA.GetColumn(0), transformatB.GetColumn(2)));
        axis.Add(Vector3.Cross(transformatA.GetColumn(1), transformatB.GetColumn(0)));
        axis.Add(Vector3.Cross(transformatA.GetColumn(1), transformatB.GetColumn(1)));
        axis.Add(Vector3.Cross(transformatA.GetColumn(1), transformatB.GetColumn(2)));
        axis.Add(Vector3.Cross(transformatA.GetColumn(2), transformatB.GetColumn(0)));
        axis.Add(Vector3.Cross(transformatA.GetColumn(2), transformatB.GetColumn(1)));
        axis.Add(Vector3.Cross(transformatA.GetColumn(2), transformatB.GetColumn(2)));

        float bestOverlap = float.MaxValue;
        int bestIndex;
        bool colStatus = true;
        for(int i = 0; i < axis.Count; i++)
        {
            if (axis[i].sqrMagnitude < 0.001f) continue;
            axis[i].Normalize();

            float overlap = OverlapOnAxis(collision.a, collision.b, collision.a.GetComponent<OBBHull>().halfSize, collision.b.GetComponent<OBBHull>().halfSize, axis[i], toCentre);
            if (overlap < 0)
            {
                colStatus = false;
                return;
            }

            if(overlap < bestOverlap)
            {
                bestOverlap = overlap;
                bestIndex = i;
            }
        }
        collision.status = colStatus;
    }

    

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
        return minMax;
    }

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
