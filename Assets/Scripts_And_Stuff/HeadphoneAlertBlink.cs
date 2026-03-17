using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadphoneAlertBlink : MonoBehaviour
{
    private AlertBlink _blink;
    private GameObject _cam;
    private Vector3 _startingUp;
    private Quaternion _startingRotation;
    // Start is called before the first frame update
    void Start()
    {
        _startingUp = transform.up;
        _startingRotation = transform.rotation;
        _blink = GetComponent<AlertBlink>();
        _blink.ON = false;
        _cam = GameObject.FindGameObjectWithTag("Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if(_blink != null && _blink.ON)
        {
            transform.forward = (transform.position - _cam.transform.position).normalized;
            transform.localRotation *= _startingRotation;
        }
    }

   public void Alert(bool on)
    {
       if(on) GetComponent<AudioSource>().Play();

        if (_blink != null)  _blink.ON = on;

    }
}
