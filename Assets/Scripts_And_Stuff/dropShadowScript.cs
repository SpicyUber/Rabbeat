using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropShadowScript : MonoBehaviour
{
    private Vector3 startingPos;
    private Vector3 startingScale;
    public bool AdjustRotation;
    private bool groundFound;
    private float maxDistance = 100f;
    private playerScript _player;
    private Quaternion _startingRotation;
    private float minDistance;

    // Start is called before the first frame update
    void Start()
    {
        minDistance = maxDistance+1f;
        _startingRotation = transform.rotation;
        if(transform.parent!=null && transform.parent.CompareTag("Player"))
        _player = transform.parent.GetComponent<playerScript>();
        startingPos = transform.parent.position;
        startingScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {   _startingRotation = transform.parent.transform.rotation;
        if(_player != null && _player.IsDead) { gameObject.SetActive(false); }
        groundFound = false;
        string str = "";
        //  RaycastHit[] hits = Physics.SphereCastAll(transform.parent.position, 0.4f, (-1)* transform.parent.up, maxDistance);
        RaycastHit[] hits = Physics.RaycastAll(transform.parent.position+Vector3.up*2f, -transform.parent.up ,maxDistance);
        foreach (RaycastHit hit in hits)
        {
            str += "shadow from "+transform.parent.name+"  -> name: " + hit.collider.name + ", tag: " + hit.collider.tag + "distance"+hit.distance+ ";\n ";

               


                
            if (hit.collider.CompareTag("Ground")  ) {
                
                transform.position = hit.point + hit.normal/1000 + hit.normal * 0.1f;
                this.transform.up = hit.normal;
             


                groundFound = true;
                  break; 
            };

        }
       // Debug.Log(str);
        if (!groundFound)
        {
            transform.localScale = Vector3.zero;
            


        }
        else
        {
            if (AdjustRotation) transform.rotation *= _startingRotation;
            transform.localScale = startingScale*(-(currentDistance()/maxDistance)+1);
            if (currentDistance() > maxDistance) { transform.localScale = Vector3.zero; }
            if (currentDistance() <= 0) { transform.localScale = startingScale; }

        }
       
    }

    private void OnDrawGizmos()
    {
      //  Gizmos.DrawCube(transform.parent.position,Vector3.one);
    }
    private float currentDistance()
    {
       return Vector3.Distance(transform.position,transform.parent.position);
    }
}
