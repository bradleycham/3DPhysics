using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGenerator : MonoBehaviour
{
    public static Vector3 GenerateForce_gravity(Vector3 worldUp, float gravitationalConstant, float particleMass)
    {
        Vector3 f_gravity = particleMass * gravitationalConstant * worldUp;
        return f_gravity;
    }
    
    public static Vector3 GenerateForce_normal(Vector3 f_gravity, Vector3 surfaceNormal_unit)
    {
        Vector3 f_normal = Vector3.Project(f_gravity, surfaceNormal_unit);
        return f_normal;
    }
    
    public static Vector3 GenerateForce_sliding(Vector3 f_gravity, Vector3 f_normal)
    {
        Vector3 f_sliding = f_normal + f_gravity;
        return f_sliding;
    }

    public static Vector3 GenerateForce_friction_static(Vector3 f_normal, Vector3 f_opposing, float frictionCoefficient_static)
    {
        float max = f_normal.magnitude * frictionCoefficient_static;
        Vector3 f_friction_s;
        if (f_opposing.magnitude * frictionCoefficient_static < max)
        {
            f_friction_s = -f_opposing;
            return f_friction_s;
        }

        else
        {
            f_friction_s = -frictionCoefficient_static * f_normal;
            return f_friction_s;
        }
    }
    public static Vector3 GenerateForce_friction_kinetic(Vector3 f_normal, Vector3 particleVelocity, float frictionCoefficient_kinetic)
    {
        Vector3 f_friction_k;
        f_friction_k = -frictionCoefficient_kinetic * f_normal.magnitude * particleVelocity;
        return f_friction_k;
    }

    public static Vector3 GenerateForce_drag(Vector3 particleVelocity, Vector2 fluidVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient)
    {
        Vector3 f_drag = -particleVelocity.normalized * (fluidDensity * (fluidVelocity * fluidVelocity) * objectArea_crossSection * objectDragCoefficient);
        return f_drag;
    }
    
    public static Vector3 GenerateForce_spring(Vector3 particlePosition, Vector3 anchorPosition, float springRestingLength, float springStiffnessCoefficient)
    {
        Vector3 force = particlePosition - anchorPosition;

        float magnitude = force.magnitude;
        magnitude = (magnitude - springRestingLength) * springStiffnessCoefficient;
        //magnitude *= springStiffnessCoefficient;
        force.Normalize();
        force *= magnitude;
        return -force;
    }
    
}
