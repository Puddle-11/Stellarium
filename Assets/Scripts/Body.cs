using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Body
{
    public Body(Transform _transform, Gravity _gravRef)
    {

        b_transform = _transform;
        b_gravity = _gravRef;
        b_mass = _gravRef.Mass;
    }

    public Transform b_transform;
    public Gravity b_gravity;
    public float b_mass;
}
