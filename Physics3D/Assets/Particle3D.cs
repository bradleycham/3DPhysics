using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle3D : MonoBehaviour
{
    public float speed;

    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 force;
    public Quaternion Rotation; 
    //public Vector3 norm;

    //public float angle;
    public Vector3 torque;
    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;

    Matrix4x4 worldToLocalTransform;
    Matrix4x4 localToWorldTransform;

    Matrix4x4 inertiaTensor;
    Matrix4x4 inverseInertiaTensor;
    Vector3 localCenterOfmass;
    Vector3 worldCenterOfMass;
    public bool applyForce = false;
    public Vector3 newForce = new Vector3(0.0f,0.0f,0.0f);
    public Vector3 pointOfForce = new Vector3(0.0f, 0.0f, 0.0f);

    //public bool applyDrag;
    //public bool applyGravity;
    //public bool kineticFriction;
    //public bool staticFriction;
    //public bool isSliding;
    //public bool isSpring;
    public bool iskinematic = true;

    private const float GRAVITY = -10;
    private Vector2 GRAVITY_VEC = new Vector2(0, GRAVITY);

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

    private void AddForce(Vector3 newForce)
    {
        force += newForce;
    }

    // POSITION AND ROTATION UPDATE FUNCTIONS
    /*
    public void UpdateAcceleration()
    {
        acceleration = force * invMass;
        force.Set(0.0f, 0.0f, 0.0f);
    }
    */
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
        // box inertia tensor
        Matrix4x4 newMat = new Matrix4x4(new Vector4((1 / 12) * mass * (1 * 1 + 1 * 1), 0f, 0f, 0f),
                                      new Vector4(0f, (1 / 12) * mass * (1 * 1 + 1 * 1), 0f, 0f),
                                      new Vector4(0f, 0f, (1 / 12) * mass * (1 * 1 + 1 * 1), 0f),
                                      new Vector4(0f, 0f, 0f, 1f));
        inertiaTensor = newMat;
        inverseInertiaTensor = inertiaTensor.transpose;

        torque = Vector3.Cross(pointOfForce, newForce);

        Vector4 newVec = (localToWorldTransform * torque);
        //Vector4 newVec = (worldToLocalTransform * torque);

        Debug.Log(newVec);

        angularAcceleration.x += newVec.x;
        angularAcceleration.y += newVec.y;
        angularAcceleration.z += newVec.z;

    }

    // TIME LOOPS
    void Start()
    {
        position = transform.position;
        //Mass = mass;
        mass = Mass;
    }


    private void FixedUpdate()
    {
        UpdateTransformMatrix();
        UpdateRotation();
        UpdatePosition();
        transform.position = position;
        this.transform.rotation = Rotation;
        if (applyForce)
        {
            ApplyForceAtLocation(pointOfForce, newForce);
            applyForce = false;
        }
        // transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);   
    }



    private void UpdateTransformMatrix()
    {
        Matrix4x4 newTransform = 
        new Matrix4x4(new Vector4(1 - (2 * Rotation.y * Rotation.y + 2 * Rotation.z * Rotation.z), (2 * Rotation.x * Rotation.y + 2 * Rotation.z * Rotation.w), (2 * Rotation.x * Rotation.z - 2 * Rotation.y * Rotation.w), position.x),
                      new Vector4((2 * Rotation.x * Rotation.y - 2 * Rotation.z * Rotation.w), 1 - (2 * Rotation.x * Rotation.x + 2 * Rotation.z * Rotation.z), (2 * Rotation.y * Rotation.z + 2 * Rotation.x * Rotation.w), position.y),
                      new Vector4((2 * Rotation.x * Rotation.z + 2 * Rotation.y * Rotation.w), (2 * Rotation.y * Rotation.z - 2 * Rotation.x * Rotation.w), 1 - (2 * Rotation.x * Rotation.x - 2 * Rotation.y * Rotation.y), position.z),
                      new Vector4(0.0f,0.0f,0.0f,1.0f));

        localToWorldTransform = newTransform;
        worldToLocalTransform = newTransform.transpose;
    }
    private void UpdateCenterOfMass()
    {

    }
    void UpdateRotation()
    {
        if(iskinematic)
        {
            UpdateRotationKinematic(Time.deltaTime);
        }
        else
        {
            UpdateRotationEulerExplicit(Time.deltaTime);
        }
    }

    void UpdatePosition()
    {
        if (iskinematic)
            updatePositionKinematic(Time.deltaTime);
        else
            UpdatePositionEulerExplicit(Time.deltaTime);
    }
}
