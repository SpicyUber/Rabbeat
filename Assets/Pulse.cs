using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pulse : MonoBehaviour
{
    public Sprite[] Frames;
    public int Fps;
   public void Start() {
        if (Fps < 1) Fps = 1;
        GetComponent<Image>().color = new(0, 0, 0, 0);
    
    }

    public void Play()
    {

        StartCoroutine(PlayRoutine());
    }

    public IEnumerator PlayRoutine()
    {
        GetComponent<Image>().color = new(1, 1, 1, 1);
        foreach (Sprite sprite in Frames)
        {
          
            GetComponent<Image>().sprite = sprite;
            yield return new WaitForSeconds(1/ (float)Fps);
        }
        GetComponent<Image>().color = new(0, 0, 0, 0);
    }
}
