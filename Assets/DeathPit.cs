using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player") { GameObject.Find("CustomGameManager").GetComponent<CustomGameManager>().Death(); this.gameObject.SetActive(false); }
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<enemyScript>().TakeDamage(99999999);
        }
    }
}
