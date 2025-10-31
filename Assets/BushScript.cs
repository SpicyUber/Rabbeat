using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushScript : MonoBehaviour
{
    private GameObject _cam;
    private Vector3 _startScale;
    private Vector3 _endScale;
    public AudioClip[] Rustles;
    private Coroutine _coroutine;
    private Transform _sprite;
    // Start is called before the first frame update
    void Start()
    {
        _sprite = transform.GetChild(0);
        _startScale = _sprite.localScale;
     _cam =   GameObject.FindWithTag("Camera");
        _endScale = _startScale * 1.2f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_cam != null) {
            Vector3 position = transform.position + _cam.transform.rotation * Vector3.forward;
            _sprite.LookAt(position, _cam.transform.rotation * Vector3.up);
        
        }
        
    }

    IEnumerator Wiggle()
    {
        float time = 0;
        _sprite.localScale = _startScale;
        Vector3 gap = -_startScale + _endScale;
        while (time<0.15f) { _sprite.localScale = _startScale + gap * (time/0.15f);   yield return null; time += Time.deltaTime; }
        _sprite.localScale = _endScale;
        time = 0;
        gap = _startScale - _endScale;
        while (time < 0.3f) { _sprite.localScale = _endScale + gap * (time / 0.3f); yield return null; time += Time.deltaTime; }

        _sprite.localScale = _startScale;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.CompareTag("Player") && Rustles!=null && Rustles.Length!=0 )
        {
            other.gameObject.GetComponentInChildren<AudioSource>()?.PlayOneShot(Rustles[Random.Range(0,Rustles.Length)], 0.2f);
            if(_coroutine != null) {  StopCoroutine(_coroutine); }
            _coroutine = StartCoroutine(Wiggle());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && Rustles != null && Rustles.Length != 0)
        {
            other.gameObject.GetComponentInChildren<AudioSource>()?.PlayOneShot(Rustles[Random.Range(0, Rustles.Length)],0.2f);
            if (_coroutine != null) {  StopCoroutine(_coroutine); }
            _coroutine = StartCoroutine(Wiggle());
        }
    }
}
