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
    public float armsInDiveMultiplier = 50f;
    public float divingTreshold = .95f;
    public float headDownDiveMultiplier = 5f;
    public float headUpAscendMultiplier = 5f;

    public Transform headTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    
    Rigidbody rb;

    float widestReach = 0;

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

        float leftWingHeight = leftHandTransform.position.y;
        float rightWingHeight = rightHandTransform.position.y;

        //calculate reach and wing height
        float reach = rightHandTransform.localPosition.x - leftHandTransform.localPosition.x;
        widestReach = Mathf.Max(widestReach, reach);
        float reachPrecent = reach / widestReach;

        //steer, maybe with head, maybe not
        Transform steeringTransform = useHeadToSteer ? headTransform : transform;
        Vector3 flyDir = steeringTransform.forward;
        flyForce += flyDir * forwardSpeed * reachPrecent;

        //add head direction to only downward vertical movements 
        float headDirection = headTransform.forward.y;
        Vector3 verticalHeadDir = new Vector3(0, headDirection, 0);
        if(headTransform.forward.y > 0)
        {
            flyForce += verticalHeadDir * (headUpAscendMultiplier * reachPrecent);
        }
        else
        {
            flyForce += verticalHeadDir * (headDownDiveMultiplier * (1 - reachPrecent));
        }        

        if(controllersActivated)
        {
            //turn based on wing offset
            float handVertOffset = leftWingHeight - rightWingHeight;
            float turnAmount = handVertOffset * turnSpeedMultiplier;
            transform.Rotate(0, turnAmount, 0);

            //flap wings to gain altitude
            float leftFlapPower = Mathf.Max(0, prevLeftWingHeight - leftWingHeight);
            float rightFlapPower = Mathf.Max(0, prevRightWingHeight - rightWingHeight);
            float combinedFlapPower = (leftFlapPower + rightFlapPower) * flapMultiplier;
            Vector3 flapForce = transform.forward * combinedFlapPower;
            flyForce += flapForce;
            
            
            //bring arms in to dive (fully outstretched will slightly gain altitude)
            float divePower = (divingTreshold - reachPrecent) * armsInDiveMultiplier;
            Vector3 diveForce = new Vector3(0, -divePower, 0);
            flyForce += diveForce;
        }

        prevLeftWingHeight = leftWingHeight;
        prevRightWingHeight = rightWingHeight;

        rb.AddForce(flyForce);
        
    }
}
