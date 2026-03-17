using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceBuilder : MonoBehaviour
{
    public GameObject Fence;
    public int Count;
    // Start is called before the first frame update
    void Start()
    {
        float reader = 0;
        for (int i = 0; i < Count; i++)
        {
            Instantiate(Fence, transform.position + transform.rotation * new Vector3(reader, 0, 0), transform.rotation,transform);
            reader += 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        float reader = -1.5f;
        for (int i = 0; i <= Count; i++)
        {
            Gizmos.DrawCube(transform.position + transform.rotation * new Vector3(reader, 0, 0), Vector3.one); 
            reader += 3;
        }
    }
}
