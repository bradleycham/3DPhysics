using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertiaTensor3D : MonoBehaviour
{
    // Start is called before the first frame update
    public float mass;
    public float radius;
    public float height;
    public float depth;
    public float width;
    public TensorType tensorType;

    private void Start()
    {
        mass = GetComponent<Particle3D>().mass;
    }
    public enum TensorType
    {
        Box,
        HollowBox,
        Sphere,
        HollowSphere,
        Cylinder,
        Cone
    }
    public Matrix4x4 GetInertiaTensor()
    {
        Matrix4x4 newMat = new Matrix4x4();


        //Sphere
        if (tensorType == TensorType.Sphere)
            newMat = new Matrix4x4(new Vector4(((2f / 5f) * mass * radius * radius), 0.0f, 0.0f, 0.0f),
                                   new Vector4(0.0f, (2f / 5f) * mass * radius * radius, 0.0f, 0.0f),
                                   new Vector4(0.0f, 0.0f, (2f / 5f) * mass * radius * radius, 0.0f),
                                   new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        // Hollow Sphere
        else if (tensorType == TensorType.HollowSphere)
            newMat = new Matrix4x4(new Vector4(((2f / 3f) * mass * radius * radius), 0.0f, 0.0f, 0.0f),
                                   new Vector4(0.0f, (2f / 3f) * mass * radius * radius, 0.0f, 0.0f),
                                   new Vector4(0.0f, 0.0f, (2f / 3f) * mass * radius * radius, 0.0f),
                                   new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        //Box
        else if (tensorType == TensorType.Box)
            newMat = new Matrix4x4(new Vector4((1f / 12f) * mass * (height * height + depth * depth), 0.0f, 0.0f, 0.0f),
                                   new Vector4(0.0f, (1f / 12f) * mass * (depth * depth + width * width), 0.0f, 0.0f),
                                   new Vector4(0.0f, 0.0f, (1f / 12f) * mass * (width * width + height * height), 0.0f),
                                   new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        //hollow Box
        else if (tensorType == TensorType.HollowBox)
            newMat = new Matrix4x4(new Vector4((5f / 3f) * mass * (height * height + depth * depth), 0.0f, 0.0f, 0.0f),
                                   new Vector4(0.0f, (5f / 3f) * mass * (depth * depth + width * width), 0.0f, 0.0f),
                                   new Vector4(0.0f, 0.0f, (5f / 3f) * mass * (width * width + height * height), 0.0f),
                                   new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        //Cylinder
        else if (tensorType == TensorType.Cylinder)
            newMat = new Matrix4x4(new Vector4((1f / 12f) * mass * (3f * radius * radius + height * height), 0.0f, 0.0f, 0.0f),
                                   new Vector4(0.0f, (1f / 12f) * mass * (3f * radius * radius + height * height), 0.0f, 0.0f),
                                   new Vector4(0.0f, 0.0f, (1f / 2f) * mass * radius * radius, 0.0f),
                                   new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        //Cone
        else if (tensorType == TensorType.Cone)
            newMat = new Matrix4x4(new Vector4((3f / 5f) * mass * height * height + (3f / 20f) * mass * radius * radius, 0.0f, 0.0f, 0.0f),
                                   new Vector4(0.0f, (3f / 5f) * mass * height * height + (3f / 20f) * mass * radius * radius, 0.0f, 0.0f),
                                   new Vector4(0.0f, 0.0f, (3f / 10f) * mass * radius * radius, 0.0f),
                                   new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
        else
            Debug.Log("Improper TensorType Error");

        return newMat;
    }
}
