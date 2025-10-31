using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    public Hint[] Hints;
    private HintPanel _hp;
    public bool SelfDestruct;
    public float WaitForSecondsBeforeActivating;
    private Collider _col;
    
    // Start is called before the first frame update
    void Start()
    {
        _hp = GameObject.FindAnyObjectByType<HintPanel>();
        _col = GetComponent<Collider>();

        if (WaitForSecondsBeforeActivating > 0.001f) { _col.enabled = false; StartCoroutine(WaitThenActivate()); }
       
    }
    IEnumerator WaitThenActivate()
    {
        yield return new WaitForSeconds(WaitForSecondsBeforeActivating);
        _col.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        
            
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player")) {

            _hp.Show(Hints);
            if(SelfDestruct) { Destroy(gameObject); }
        }
    }
    private void OnTriggerExit(Collider other)
    {
       
    }
    
}
