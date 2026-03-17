using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float[] layers = new float[32];
        layers[0] = 1500;
        GetComponent<Camera>().layerCullDistances = layers;
        GetComponent<Camera>().layerCullSpherical = true;
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
