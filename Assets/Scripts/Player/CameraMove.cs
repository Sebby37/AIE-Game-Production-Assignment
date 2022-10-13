using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Sebastian Cramond
public class CameraMove : MonoBehaviour
{
    public Transform target;
    public float drag;

    Vector2 moveVelocity;

    private void Start()
    {
        transform.position = target.position;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector2.SmoothDamp(transform.position, target.position, ref moveVelocity, drag);
    }
}
