using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public bool useHeadToSteer = false;

    public float forwardSpeed = 10;
    public float turnSpeedMultiplier = .05f;

    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    

    Rigidbody rb;

    float widestReach = 0;

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
        float handVertOffset = leftHandTransform.position.y - rightHandTransform.position.y;
        steeringTransform = useHeadToSteer ? headTransform : transform;
        Vector3 flyDir = steeringTransform.forward;
        Vector3 flyForce = flyDir * forwardSpeed;
        rb.AddForce(flyForce);
        Quaternion newRotation = transform.rotation;
        newRotation.y += handVertOffset * turnSpeedMultiplier;
        transform.rotation = newRotation;
    }
}
