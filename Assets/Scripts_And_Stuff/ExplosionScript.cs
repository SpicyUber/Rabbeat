using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public Material MaterialComponent;
    private float i = 0;
    // Start is called before the first frame update
    void Start()
    {
        MaterialComponent = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        i += Time.deltaTime * 2f;
        i %= 1;
        if(MaterialComponent != null)
        {
            MaterialComponent.mainTextureOffset = new(0,i);
        }
        
    }
}
