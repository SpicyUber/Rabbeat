using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    private bool _destroyed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_destroyed) return;  
        if(collision.gameObject.name == "Attack") { SelfDestroy(); }
            else
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<playerScript>().IsBouncing() && !collision.gameObject.GetComponent<playerScript>().IsTumbling() )
        {
            SelfDestroy();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            SelfDestroy();  }
    }

    public void SelfDestroy()
    {
        if (_destroyed) return;
        FindAnyObjectByType<CustomGameManager>().AddEnergyOrb(5,transform.position,false);
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false; gameObject.GetComponent<AudioSource>().Play(); gameObject.GetComponentInChildren<ParticleSystem>().Play(); _destroyed = true; StartCoroutine(DestroyAfter(3f));

    }
    IEnumerator DestroyAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
