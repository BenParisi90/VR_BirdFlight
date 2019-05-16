using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public bool useHeadToSteer = false;

    public float forwardSpeed = 10;

    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Transform steeringTransform;
        if(useHeadToSteer)
        {
            steeringTransform = headTransform;
        }
        else
        {
            steeringTransform = transform;
        }
        Vector3 flyDir = steeringTransform.forward;
        Vector3 flyForce = flyDir * forwardSpeed;
        rb.AddForce(flyForce);
    }
}
