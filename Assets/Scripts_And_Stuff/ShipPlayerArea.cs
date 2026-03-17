
using UnityEngine;

public class ShipPlayerArea : MonoBehaviour
{
   private Spaceship ship;

    private void Start()
    {
        ship =transform.parent.GetComponent<Spaceship>();
    }
    private void OnTriggerEnter(Collider other)
    {   if (ship.Crashed) return;
        if (other.CompareTag("Player")) { other.gameObject.GetComponent<playerScript>().IsInPipe = true; other.gameObject.GetComponent<playerScript>().IsStunned=true; ship.Puppet = other.gameObject.transform; }
    }
    

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player")) ship.Puppet = null;

    }
}
