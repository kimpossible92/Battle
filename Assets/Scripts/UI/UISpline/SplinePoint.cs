using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePoint 
{
    public Vector3 Position;
    public Vector3 Normal;

    public SplinePoint(Vector3 position, Vector3 normal)
    {
        Position = position;
        Normal = normal;
    }
}
