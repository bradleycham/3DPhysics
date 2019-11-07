﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle3D : MonoBehaviour
{
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 force;
    public Quaternion Rotation;
    //public Vector3 norm;

    public Vector3 torque;
    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;

    Matrix4x4 worldToLocalTransform;
    Matrix4x4 localToWorldTransform;

    Matrix4x4 inertiaTensor;
    Matrix4x4 inverseInertiaTensor;
    Vector3 localCenterOfMass;
    Vector3 worldCenterOfMass;
    public bool applyForce = false;
    public Vector3 newForce = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 pointOfForce = new Vector3(0.0f, 0.0f, 0.0f);

    InertiaTensor3D tensorComponent;
    //public bool applyDrag;
    public bool applyGravity;
    //public bool kineticFriction;
    //public bool staticFriction;
    //public bool isSliding;
    //public bool isSpring;
    public bool iskinematic = true;

    private const float GRAVITY = -10;
    private Vector3 GRAVITY_VEC = new Vector3(0.0f, -GRAVITY, 0.0f);

    [Range(0, Mathf.Infinity)]
    public float mass;
    public enum EUpdateMethod
    {
        Euler,
        Kinematic
    };
    
    // MASS AND FORCE
    private float invMass;
    private float Mass
    {
        set
        {
            mass = mass > 0.0f ? mass : 0.0f;
            invMass = mass > 0.0f ? 1.0f / mass : 0.0f;
        }
        get
        {
            return mass;
        }
    }

    public void AddForce(Vector3 newForce)
    {
        force += newForce;
    }

    public void UpdateAcceleration()
    {
        acceleration = force * invMass;
        force.Set(0.0f, 0.0f, 0.0f);

        Vector3 updateAngularAcceleration = ((inverseInertiaTensor * worldToLocalTransform.transpose) * worldToLocalTransform) * torque;
        angularAcceleration = updateAngularAcceleration;
    }

    void UpdatePositionEulerExplicit(float deltaTime)
    {
        position += velocity * deltaTime;
        velocity += acceleration * deltaTime;
    }

    void updatePositionKinematic(float deltaTime)
    {
        position += (velocity * deltaTime) + ((acceleration * deltaTime * deltaTime)/2);
        velocity += acceleration * deltaTime;
    }
    
    void UpdateRotationEulerExplicit(float deltaTime)
    {
        Quaternion newVel = new Quaternion(angularVelocity.x * deltaTime / 2f, angularVelocity.y * deltaTime / 2f, angularVelocity.z * deltaTime / 2f, 0.0f);
        Quaternion rotation = newVel * Rotation;

        Rotation.x += rotation.x;
        Rotation.y += rotation.y;
        Rotation.z += rotation.z;
        Rotation.w += rotation.w;
        angularVelocity += angularAcceleration * deltaTime;
        Rotation = Rotation.normalized;
    }

    void UpdateRotationKinematic(float deltaTime)
    {
        Quaternion newVel = new Quaternion(((angularVelocity.x * deltaTime) + (angularAcceleration.x * deltaTime * deltaTime) / 2) / 2f,
                                            ((angularVelocity.y * deltaTime) + (angularAcceleration.y * deltaTime * deltaTime) / 2) / 2f,
                                            ((angularVelocity.z * deltaTime) + (angularAcceleration.z * deltaTime * deltaTime) / 2) / 2f,
                                              0.0f);
        Quaternion rotation = newVel * Rotation;
        
        Rotation.x += rotation.x;
        Rotation.y += rotation.y;
        Rotation.z += rotation.z;
        Rotation.w += rotation.w;
        angularVelocity += angularAcceleration * deltaTime;
        Rotation = Rotation.normalized;
    }

    
    void ApplyForceAtLocation(Vector3 pointOfForce, Vector3 newForce)
    {
        torque = Vector3.Cross(pointOfForce, newForce);        
    }

    // TIME LOOPS
    void Start()
    {
        tensorComponent = GetComponent<InertiaTensor3D>();
        inertiaTensor = tensorComponent.GetInertiaTensor();
        inverseInertiaTensor = inertiaTensor.transpose;

        position = transform.position;
        Mass = mass;
    }

    private void FixedUpdate()
    {
        UpdateAcceleration();
        UpdateRotation();
        UpdatePosition();
        UpdateTransformMatrix();
        UpdateCenterOfMass();

        this.transform.rotation = Rotation;
        transform.position = position;

        if (applyForce)
        {
            ApplyForceAtLocation(pointOfForce, newForce);
            applyForce = false;
        }     
    }


    private void UpdateTransformMatrix()
    {
        Matrix4x4 newTransform = 
        new Matrix4x4(new Vector4(1f - (2f * Rotation.y * Rotation.y + 2f * Rotation.z * Rotation.z), (2f * Rotation.x * Rotation.y + 2f * Rotation.z * Rotation.w), (2f * Rotation.x * Rotation.z - 2f * Rotation.y * Rotation.w), 0.0f),
                      new Vector4((2f * Rotation.x * Rotation.y - 2f * Rotation.z * Rotation.w), 1f - (2f * Rotation.x * Rotation.x + 2f * Rotation.z * Rotation.z), (2f * Rotation.y * Rotation.z + 2f * Rotation.x * Rotation.w), 0.0f),
                      new Vector4((2f * Rotation.x * Rotation.z + 2f * Rotation.y * Rotation.w), (2f * Rotation.y * Rotation.z - 2f * Rotation.x * Rotation.w), 1f - (2f * Rotation.x * Rotation.x - 2f * Rotation.y * Rotation.y), 0.0f),
                      new Vector4(position.x, position.y, position.z ,1.0f));

        worldToLocalTransform = newTransform;
        localToWorldTransform = newTransform.inverse;
    }

    private void UpdateCenterOfMass()
    {
        Vector4 newVec = new Vector4(localCenterOfMass.x, localCenterOfMass.y, localCenterOfMass.z, 1.0f);
        worldCenterOfMass = worldToLocalTransform * newVec;
    }

    void UpdateRotation()
    {
        if(iskinematic)
            UpdateRotationKinematic(Time.deltaTime);
        else
            UpdateRotationEulerExplicit(Time.deltaTime);
    }

    void UpdatePosition()
    {
        if (iskinematic)
            updatePositionKinematic(Time.deltaTime);
        else
            UpdatePositionEulerExplicit(Time.deltaTime);
    }
}
