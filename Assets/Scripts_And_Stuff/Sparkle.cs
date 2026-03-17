using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparkle : MonoBehaviour
{

    public Sprite One, Two;
    private GameObject _cam;
    // Start is called before the first frame update
    void Start()
    {
        _cam = GameObject.FindWithTag("Camera");
        StartCoroutine(SparkleCoroutine()); 
    }

    IEnumerator SparkleCoroutine()
    {
        for (int  i = 0; i < 1; i++) { 
        GetComponent<SpriteRenderer>().sprite=One;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().sprite = Two;
        yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (_cam != null)
        {
            Vector3 position = transform.position + _cam.transform.rotation * Vector3.forward;
            transform.LookAt(position, _cam.transform.rotation * Vector3.up);

        }
    }
}
