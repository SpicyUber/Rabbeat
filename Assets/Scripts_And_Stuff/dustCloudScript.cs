using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dustCloudScript : MonoBehaviour
{
    private Vector3 startingPos;
    private bool groundFound;
    private float maxDistance = 100f;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.parent.position;
 
    }

    // Update is called once per frame
    void Update()
    {
        groundFound = false;

        RaycastHit[] hits = Physics.SphereCastAll(transform.parent.position+transform.parent.transform.up*2, 0.4f, (-1) * transform.parent.up, maxDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Ground")
            {
                transform.position = hit.point + hit.normal/2;
                this.transform.up = hit.normal;
                transform.rotation =Quaternion.Euler(new Vector3( transform.rotation.x - 90, transform.rotation.y, transform.rotation.z));



                groundFound = true;
                break;
            };

        }
       
    }
    private float currentDistance()
    {
        return Vector3.Distance(transform.position, transform.parent.position);
    }
}
