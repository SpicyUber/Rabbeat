using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mark : MonoBehaviour
{
    public enemyScript Target;
   
    // Start is called before the first frame update
   

    public void Init(enemyScript e)
    {
        Target = e;
        transform.localPosition += new Vector3 (0, e.MarkOffset, 0);
        if (Target == null || Target.IsDying) Destroy(gameObject);
    }
}
