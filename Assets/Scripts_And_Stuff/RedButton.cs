using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedButton : MonoBehaviour
{
    public Spaceship Ship;
    private bool pressed=false;
    public ShipPlayerArea SPA;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FlashRed());
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private IEnumerator FlashRed() { 
    while(!pressed)
        {
            float t = 0f;
            while (t< 2f)
            {
              GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, Mathf.Clamp((2f - t) / 2f, 0, 0.75f), Mathf.Clamp((2f - t) / 2f, 0, 0.75f)));

                yield return null;
                t += Time.deltaTime;
            }
t = 0f;
while (t < 1.5f)
{
     GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1,  Mathf.Clamp(t/ 2f,0,0.75f), Mathf.Clamp(t / 2f, 0, 0.75f)));

    yield return null;
    t += Time.deltaTime;
}
 GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, 0.75f, 0.75f));

yield return null;

        }
}

    private void OnCollisionEnter(Collision collision)
    {
        if (pressed) return;

       
        if (collision.collider.CompareTag("Player") && collision.collider.gameObject.GetComponent<playerScript>().IsBouncing())
        {
            
            SPA.gameObject.SetActive(true);
            collision.collider.gameObject.GetComponent<Rigidbody>().velocity= Vector3.zero;
            transform.localScale = new(transform.localScale.x, transform.localScale.y/2, transform.localScale.z);
            pressed = true;
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, 0, 0));
            
            Ship.Crash();
        }
    }
}
