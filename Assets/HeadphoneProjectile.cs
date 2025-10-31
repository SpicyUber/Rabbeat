using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class HeadphoneProjectile : MonoBehaviour
{
    
    private enemyScript Target;
    public ParticleSystem DestroyParticles;
    public ParticleSystem NormalParticles;
    private bool _fired = false;
    private float _maxT;
    private float _t;
    private Vector3 _position;
    private bool _isDestroyed = false;
    // Start is called before the first frame update
    void Start()
    {
        _position = transform.position;
    }
    public void Fire(enemyScript e)
    {
        Target = e;
        _t = 0;
        _maxT = 60f/GameObject.FindObjectOfType<rhythmSystemScript>().bpm;
      //  _maxT += _maxT / 20f;
        _fired = true;
        GetComponent<AudioSource>().pitch = Mathf.Pow(1.059463f, (int)(GameObject.FindObjectOfType<rhythmSystemScript>().SongKey)) ;
        GetComponent<AudioSource>().Play();
    }
    // Update is called once per frame
    void Update()
    {
        if (!_fired) return;
        if (Target == null && _fired==true && _isDestroyed==false) { _isDestroyed = true; Destroy(gameObject); return; }

        if (Target != null)
        {
            transform.position = Vector3.Lerp(_position, Target.transform.position, _t / _maxT);
            transform.LookAt(Target.transform.position);
        }
        _t += Time.deltaTime;

        if (_t >= _maxT && Target != null   ) { if (Target.GetComponentInChildren<Mark>() != null) {Destroy(Target.GetComponentInChildren<Mark>().gameObject); } Target.TakeDamage(20); DestroyProjectile(); Target = null; }
    }

     

    public void DestroyProjectile()
    { if (_isDestroyed) return;
        _isDestroyed = true;
        StartCoroutine(DestroyRoutine());

    }
    IEnumerator DestroyRoutine()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        NormalParticles.Stop();
        DestroyParticles.Play();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
