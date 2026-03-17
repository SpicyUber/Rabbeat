using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadphoneJump : HeadphoneAttack
{
    private bool _instantiatedJumpPad = false;
    public GameObject JumpPad;
    private float _iterations = 600;
    override public int GetLevel() { return (1 + PlayerPrefs.GetInt("JumpLvl")); }

    public override void DoSomething()
    {
        if (_instantiatedJumpPad || currentState==State.POSITIONING) return;
        _instantiatedJumpPad = true;
       GameObject go =Instantiate(JumpPad, transform.position,CalculateRotation());
       go.transform.SetParent(transform);
    }

    private void OnDestroy()
    {
        RemoveSubscription();
    }
    public void Start()
    {
        currentState= State.POSITIONING;
        InitAbility();
    }
    private Quaternion CalculateRotation()
    {
        RaycastHit hit;
        Quaternion rotation = Quaternion.identity;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 15f))
        {
          return  Quaternion.LookRotation(Vector3.forward,hit.normal);
        }
        else
            return Quaternion.identity;

    }

    public void FixedUpdate()
    {
        if(_iterations<=0) Destroy(gameObject);

        _iterations--;
    }
}
