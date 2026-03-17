using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Firewall : MonoBehaviour
{
    public RawImage RawImageComponent;
    private float _num;
    private bool _moveUp = true;
    public GameObject Player;
    public GameObject FireSource;
    // Start is called before the first frame update
    void Start()
    {
        _num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        RawImageComponent.material.SetFloat("_Value", _num);
        RawImageComponent.material.SetFloat("_Ammount", Mathf.Clamp01(150-(Player.transform.position- FireSource.transform.position).magnitude));
        if(_moveUp) { 
        _num += Time.deltaTime/10;
        }
        else
            _num -= Time.deltaTime / 10;

        if( _num>0.15 ) {
            _moveUp = false;
        }
        else if(_num<-0.15) {
            _moveUp = true;
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(FireSource.transform.position,100);
    }
}
