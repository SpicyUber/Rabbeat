using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearAfterXSeconds : MonoBehaviour
{
    public GameObject Target;
    public float Seconds;
    private float _t = 0;
    // Start is called before the first frame update
    void Start()
    {
     Target.SetActive(false);   
    }

    private void Update()
    {
        _t += Time.deltaTime;
        if(_t>Seconds)Target.SetActive(true);
    }


}
