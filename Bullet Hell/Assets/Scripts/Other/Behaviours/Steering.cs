using UnityEngine;
using System.Collections;

public class Steering : MonoBehaviour
{
    public float angular;
    public Vector3 linear;

    public Steering()
    {
        angular = 0.0f;
        linear = new Vector3();
    }
}