using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    private float value=0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<MeshRenderer>().materials[0].SetFloat("_Value", value);
        value += Time.deltaTime * 1.2f;
        if (value > 0.38f ) Destroy(this.gameObject) ;
    }
}
