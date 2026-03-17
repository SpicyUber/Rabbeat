using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BezierDropDown : MonoBehaviour
{
    public float Offset = 0f;
    // Start is called before the first frame update
    void Start()
    {
         
        RaycastHit[] hits =Physics.RaycastAll(new(transform.position, -transform.up));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Ground"))
            {    
                transform.position = hit.point+Vector3.up*Offset;
                break;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
