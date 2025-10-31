using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertBlink : MonoBehaviour
{
    public rhythmSystemScript RS;
    public bool ON;
    private MeshRenderer _mesh;
    private Boombox _boombox;
    private bool _isTalking;
     
    // Start is called before the first frame update
    void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
        _boombox = GetComponentInParent<Boombox>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_boombox != null) { 
        _isTalking = _boombox.IsTalking;
        }
        if (RS == null    ) {  return; }
        if(_isTalking == true) { ON = false; }
        _mesh.enabled=(ON && RS.beatMap[RS.beatIndex].isActive);

       
    }
}
