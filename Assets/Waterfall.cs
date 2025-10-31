using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterfall : MonoBehaviour
{
    public Texture[] Sprites;
    GameObject _player;
    
    public Material Source;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<MeshRenderer>().material = new Material(Source);
         
        if (Sprites != null && Sprites.Length>0)
        StartCoroutine(WaterfallRoutine());
    }

    IEnumerator WaterfallRoutine()
    {
        int i = Sprites.Length-1;
        while(i >=0)
        {
            GetComponent<MeshRenderer>().material.mainTexture= Sprites[i];
            yield return new WaitForSeconds(0.1f);
            if (_player.transform.position.y < transform.position.y)
                i--;
            else i++;
            if(i<0) i= Sprites.Length - 1;
            if (i >= Sprites.Length) {i= 0;}
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
