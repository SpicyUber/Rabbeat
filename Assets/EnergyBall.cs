using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    private float _t=0;
    private bool _on = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(On());
    }
    IEnumerator On()
    {
        yield return new WaitForSeconds(0.15f);
        _on= true;  
        GetComponent<AudioSource>().Play();
          GetComponent<CinemachineImpulseSource>().GenerateImpulse(2f);  
    }
    // Update is called once per frame
    void Update()
    { if (!_on) return;
        transform.localScale = Vector3.one* 75f * _t;
        _t = _t+ Time.deltaTime;
        transform.Rotate(Time.deltaTime * new Vector3(0,1,0) * 550f);
        if (_t >= 2f) { Destroy(gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyScript es = other.gameObject.GetComponent<enemyScript>();
         if(es.currentState!=enemyScript.state.STUN) es.Bounce((es.transform.position-transform.position).normalized*2f);

        }
    }
}
