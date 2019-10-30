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

    public float angle;
    public Vector3 torque;
    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;

    //public float invInertia;
    //public Vector2 applyForce;
    //public Vector2 positionOfForce;

    public bool applyDrag;
    public bool applyGravity;
    public bool kineticFriction;
    public bool staticFriction;
    public bool isSliding;
    public bool isSpring;
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
    void updatePositionEulerExplicit(float deltaTime)
    {
        position += velocity * deltaTime;
        velocity += acceleration * deltaTime;
    }

    void updatePositionKinematic(float deltaTime)
    {
        position += (velocity * deltaTime) + ((acceleration * deltaTime * deltaTime)/2);
        velocity += acceleration * deltaTime;
    }
    
    void updateRotationEulerExplicit(float deltaTime)
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

    void updateRotationKinematic(float deltaTime)
    {
        //angle += deltaTime * angularVelocity;
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
        //Quaternion rotation = new Quaternion(i,j,k,w);
    }

    /*
void applyForceAtLocation(Vector2 pointOfForce, Vector2 newForce)
{
    if (Shape == shape.rect)
        invInertia = (1 / 12) * mass * ((pointOfForce.x * pointOfForce.x) + (pointOfForce.y * pointOfForce.y));
    else if (Shape == shape.circle)
        invInertia = (1 / 2) * mass * GetComponent<SphereCollider>().radius* GetComponent<SphereCollider>().radius; // *radius squared
    torque = Vector3.Cross(pointOfForce, newForce).z;
    angularVelocity += torque;

    applyForce = new Vector3(0.0f, 0.0f);
    positionOfForce = new Vector3(0.0f, 0.0f);
}
*/
    // TIME LOOPS
    void Start()
    {
        position = transform.position;
        Mass = mass;
        
        //circulate = false;
    }

    
    // Update is called once per frame
    void Update()
    {
        /*
        if(applyGravity)
            AddForce(ForceGenerator.GenerateForce_gravity(Vector2.up, GRAVITY, Mass));
        if (applyDrag)
            AddForce(ForceGenerator.GenerateForce_drag(velocity, new Vector2(0f, 5f), 1.0f, 1.0f, 0.05f));

        norm = ForceGenerator.GenerateForce_normal(new Vector2(0, GRAVITY), new Vector2(-1f, 1f));
        if (kineticFriction)
            AddForce(ForceGenerator.GenerateForce_friction_kinetic(norm , velocity, 0.55f));
        if (staticFriction)
            AddForce(ForceGenerator.GenerateForce_friction_static(norm, velocity, 0.60f));
        if (isSliding)
            AddForce(ForceGenerator.GenerateForce_sliding(GRAVITY_VEC, -velocity));
        if (isSpring)
            AddForce(ForceGenerator.GenerateForce_spring(position, new Vector2(1.0f, 0.0f), 2.5f, 100f));
        */
    }

    private void FixedUpdate()
    {
        //UpdateAcceleration();
        //applyForceAtLocation(positionOfForce, applyForce);
        updatePositionKinematic(Time.fixedDeltaTime);
        UpdateRotation();
        transform.position = position;
        this.transform.rotation = Rotation;
       // transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);   
    }

    void UpdateRotation()
    {
        if(iskinematic)
        {
            updateRotationKinematic(Time.deltaTime);
        }
        else
        {
            updateRotationEulerExplicit(Time.deltaTime);
        }
    }
}
