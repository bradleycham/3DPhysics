using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hull3D : MonoBehaviour
{
    public Vector3 localCenter;
    public float restitution;
    public float boundingVolumeRadius;
    protected CollisionHull3D.hullType type;

    public CollisionHull3D.hullType GetHullType()
    {
        return type;
    }
}
