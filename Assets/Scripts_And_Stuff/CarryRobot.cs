using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryRobot : BirdBot
{
    public GameObject CarryObject;
    public bool UseReferenceObject;
    public GameObject ReferenceObject;
    public Texture One, Two, Three;
    private bool switchBoolean=false;
    private float traveledDistance=0;
    private float plusMinus=5f;
    private bool goingUp;
    public bool DoNotRotate;
    public bool CustomSpeed;
    private Vector3 carryPosition;
    private Quaternion tempRotation;
    public Vector3 CarryOffset;
    public override void Aggro()
    {
        currentState = state.IDLE;
    }

    public override void Attack()
    {
        currentState = state.IDLE;
    }
    public override void Idle()
    {
       
        

        




        if (ReferenceObject != null) { location2 = ReferenceObject.transform.position; }
        if (switchBoolean && (location2 - transform.position).magnitude < 5f)
        {
            switchBoolean = false;
        }
        else if (!switchBoolean && (location1 - transform.position).magnitude < 5f)
        {
            switchBoolean = true;
            
        }
        if(UseReferenceObject)
        CarryObject?.SetActive(!switchBoolean);
        if (UseReferenceObject && ReferenceObject==null) { switchBoolean = false; CarryObject.SetActive(false); }
        if(CarryObject==null) { switchBoolean = false; CarryObject.SetActive(false); }
        CarryObject.transform.localPosition =   carryPosition;
        if (switchBoolean) { transform.LookAt(location2); if (!CustomSpeed) { transform.Translate((location2 - transform.position).normalized * 15f * Time.deltaTime, Space.World); } else { transform.Translate((location2 - transform.position).normalized * speed * Time.deltaTime, Space.World); } }
        else { if (!CustomSpeed) { transform.Translate((location1 - transform.position).normalized * 15f * Time.deltaTime, Space.World); } else { transform.Translate((location1 - transform.position).normalized * speed * Time.deltaTime, Space.World); } }

        if (UseReferenceObject && ReferenceObject == null) { transform.LookAt(location2); }
            if (CarryObject == null) { transform.LookAt(location2); }
    



        if (traveledDistance > plusMinus)
        {
            traveledDistance = 0;

            goingUp = !goingUp;
        }

        traveledDistance += (Time.deltaTime * 2f * transform.up).magnitude;

        if (goingUp)
        {
            transform.GetChild(0).localPosition += Time.deltaTime * 1f * transform.up ;
            carryPosition += Time.deltaTime * 1f * transform.up;
        }
        else
        {
            transform.GetChild(0).localPosition -= Time.deltaTime * 1f * transform.up ;
            carryPosition -= Time.deltaTime * 1f * transform.up;
        }
    }
    private void LateUpdate()
    {
        if (DoNotRotate)
        {
            transform.rotation = tempRotation;
        }
       tempRotation = transform.rotation;
    }
    public override void CustomStart()
    {
        CarryObject = GameObject.Instantiate(CarryObject, transform.position+ CarryOffset, transform.rotation, transform);
        if(!UseReferenceObject) { CarryObject.SetActive(true); }
        carryPosition = new Vector3(0, -3, 0) + CarryOffset; 
        tempRotation = transform.rotation;
        
    }
}
