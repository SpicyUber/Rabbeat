using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundWiggle : MonoBehaviour
{
    Material _material;
    AudioSource _audio; 
    float[] _samples;
    public float LowPoint = -0.001f;
    public float HighPoint = 0.01f;
    public int SampleNumber = 64;
    public int ZRotationSpeed = 50;
    public int XRotationSpeed = 0;
    public int YRotationSpeed = 0;
    // Start is called before the first frame update
    void Start()
    {   _samples = new float[64];
        _material = GetComponent<MeshRenderer>().sharedMaterial;
        _audio = GameObject.FindObjectOfType<rhythmSystemScript>().song;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(XRotationSpeed * Time.deltaTime, YRotationSpeed * Time.deltaTime, ZRotationSpeed * Time.deltaTime, Space.Self);
        _audio.GetOutputData(_samples,0);
        float max = 0f;
        for (int i = 0; i < 32; i++)
        {
            float abs = Mathf.Abs(_samples[i]);
            if (abs > max)
                max = abs;
        }

        _material.SetFloat("_Value",Mathf.Lerp(LowPoint,HighPoint,max));
    }
}
