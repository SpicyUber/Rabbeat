using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;

public class EnergyOrb : MonoBehaviour
{
    private bool _go=false;
    private Rigidbody _rb;
    private GameObject _target;
    private GameObject _overrideTarget;
    private float _trackT; //measures ammount of time the orb has been tracking the target 
    private float _t; //measures ammount of time the orb has been alive
    private Color _primaryColor,_secondaryColor;
    private int level = 1;
    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player");
        _rb = GetComponent<Rigidbody>();
        transform.position = transform.position + RandomPositionOffset();
        StartCoroutine(GetReady());
        _primaryColor = new Color(36/255f, 141 / 255f, 197 / 255f);
        _secondaryColor = new Color(200 / 255f, 200 / 255f, 200 / 255f);

    }
    private IEnumerator GetReady()
    {
        float t = 0;
        while (t<1f+RandomTimeOffset()) {
            
            transform.position += transform.up * 3f * Time.deltaTime;
            
            yield return null;
            t+=Time.deltaTime;
        }
        _go=true;
    }

    public void SetLevel(int level)
    {
        this.level= level;
        transform.localScale = transform.localScale * level;
    }
    Vector3 RandomPositionOffset()
    {

        return new Vector3(Random.Range(0,2f), Random.Range(0, 2f), Random.Range(0, 2f));
    }

    float RandomTimeOffset()
    {
        return Random.Range(0, 2f);
    }
    public void OverrideTarget(GameObject g) {
    _overrideTarget = g;

    }
    // Update is called once per frame
    void FixedUpdate()
    { if (!_go) return;
       
        if (_overrideTarget != null) { _target = _overrideTarget; }
        if (_trackT < 0.5f) _rb.AddForce((_target.transform.position - transform.position).normalized * 5f, ForceMode.Impulse);
        else _rb.velocity = (_target.transform.position - transform.position).normalized*35f;
    }

    private void LateUpdate()
    {
        _t += Time.deltaTime;
        if (!_go) return;
        _trackT += Time.deltaTime;
    }

    private void Update()
    {
       MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
     if(mr!= null) { mr.materials[0].SetColor("_Color", Color.Lerp(_primaryColor, _secondaryColor, Mathf.Abs(Mathf.Sin(_t*4)))); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _go) { _go = false; GetComponent<AudioSource>().pitch = Mathf.Pow(1.059463f, (int)(GameObject.FindAnyObjectByType<rhythmSystemScript>().SongKey));
            GetComponent<AudioSource>().Play(); GameObject.FindAnyObjectByType<CustomGameManager>().AddEnergy((level>0)?level:1); StartCoroutine(DestroyAfter(1f)); }
    }

    IEnumerator DestroyAfter(float seconds)
    {
        transform.GetChild(0).gameObject.SetActive(false);
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
