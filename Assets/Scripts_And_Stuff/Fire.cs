using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Fire : MonoBehaviour
{
    public Sprite[] Sprites;
    private GameObject _cam;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FireRoutine());
        _cam = GameObject.FindWithTag("Camera");
         
    }

    void LateUpdate()
    {
        if (_cam != null)
        {
            Vector3 position = transform.position + _cam.transform.rotation * Vector3.forward;
            transform.LookAt(position, _cam.transform.rotation * Vector3.up);

        }

    }
    IEnumerator FireRoutine()
    {
        int i = Sprites.Length - 1;
        while (i >= 0)
        {
            GetComponent<SpriteRenderer>().sprite = Sprites[i];
            yield return new WaitForSeconds(0.2f);
             i++;
            if (i < 0) i = Sprites.Length - 1;
            if (i >= Sprites.Length) { i = 0; }
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
