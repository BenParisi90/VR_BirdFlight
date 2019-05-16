using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BirdController : MonoBehaviour
{
    public bool useHeadToSteer = true;
    public bool flapToGainAltitude = true;

    public float forwardSpeed = 10;
    public float turnSpeedMultiplier = 1;
    public float flapMultiplier = 350f;
    public float diveMultiplier = 50f;
    public float divingTreshold = .95f;

    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    

    Rigidbody rb;

    float widestReach = 0;

    float leftFlapPower;
    float rightFlapPower;
    float combinedFlapPower;
    float leftWingHeight;
    float rightWingHeight;
    float prevLeftWingHeight;
    float prevRightWingHeight;
    

    bool controllersActivated = false;
    public SteamVR_Action_Boolean activateLeft;
    public SteamVR_Action_Boolean activateRight;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!controllersActivated && activateLeft.state && activateRight.state)
        {
            controllersActivated = true;
        }
    }

    void FixedUpdate()
    {
        
        Vector3 flyForce = new Vector3();

        leftWingHeight = leftHandTransform.position.y;
        rightWingHeight = rightHandTransform.position.y;

        //steer, maybe with head, maybe not
        Transform steeringTransform = useHeadToSteer ? headTransform : transform;;
        Vector3 flyDir = steeringTransform.forward;
        flyForce += flyDir * forwardSpeed;

        //add head direction to only vertical movements
        Vector3 verticalHeadDir = new Vector3(0, headTransform.forward.y, 0);
        flyForce += verticalHeadDir * forwardSpeed;

        

        //calculate reach and wing height
        float reach = rightHandTransform.localPosition.x - leftHandTransform.localPosition.x;
        widestReach = Mathf.Max(widestReach, reach);
        float reachPrecent = reach / widestReach;

        

        if(controllersActivated)
        {
            //turn based on wing offset
            float handVertOffset = leftWingHeight - rightWingHeight;
            float turnAmount = handVertOffset * turnSpeedMultiplier;
            transform.Rotate(0, turnAmount, 0);

            //flap wings to gain altitude
            
            leftFlapPower = Mathf.Max(0, prevLeftWingHeight - leftWingHeight);
            rightFlapPower = Mathf.Max(0, prevRightWingHeight - rightWingHeight);
            combinedFlapPower = (leftFlapPower + rightFlapPower) * flapMultiplier;
            Vector3 flapForce = new Vector3(0, combinedFlapPower, 0);
            flyForce += flapForce;
            
            
            //bring arms in to dive (fully outstretched will slightly gain altitude)
            float divePower = (divingTreshold - reachPrecent) * diveMultiplier;
            Vector3 diveForce = new Vector3(0, -divePower, 0);
            flyForce += diveForce;
        }

        prevLeftWingHeight = leftWingHeight;
        prevRightWingHeight = rightWingHeight;

        rb.AddForce(flyForce);
        
    }
}
