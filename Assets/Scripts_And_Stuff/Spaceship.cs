using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{
    private int wiggleSpeed = 1;
   
    private Vector3 topH;
    private bool crashing;
    private Vector3 bottomH;
    private float plusMinus = 6f;
    private bool goingUp = true;
    private float traveledDistance = 0;
    public Vector3 EndLocation;
    public int Speed;
    public ParticleSystem PS1,PS2,PS3;
    public Transform Puppet;
    public GameObject Fire;
    public GameObject Firewall;
     private CinemachineImpulseSource impulse;
    public bool Crashed=false;
    public AudioClip Engine1, Engine2, SelfDestruct,ExplosionSound,FireSound;
    private AudioSource audioSource;
    public BoxCollider CameraTriggerCollider;
    public CombatWall CW;

    
   
    private void Start()
    {
         impulse = GetComponent<CinemachineImpulseSource>();
        audioSource =GetComponent<AudioSource>();
        audioSource.clip = Engine1;
        PS1.Stop();
        PS2.Stop();
        PS3.Stop();
        topH = transform.position + transform.up * plusMinus / 2;
        bottomH = transform.position - transform.up * plusMinus / 2;
        transform.position = bottomH;
    }
    void Update()
    {
        if(!crashing)
        Move();
        else
        Crashing();
    }
    private void Crashing()
    {   if (Crashed == true) return;
        if ((EndLocation - transform.position).magnitude < 0.5f) {Crashed = true; Fire.SetActive(true); Firewall.SetActive(true); Puppet.gameObject.GetComponent<playerScript>().IsInPipe = false; Puppet.gameObject.GetComponent<playerScript>().IsStunned = false; impulse.GenerateImpulse(2f); audioSource.clip = FireSound; audioSource.maxDistance = 15; audioSource.Play(); Puppet = null; CameraTriggerCollider.enabled = false; GameObject.FindObjectOfType<CameraTrigger>().ResetCamera(); return; }
        transform.Translate(Speed * Time.deltaTime * (EndLocation-transform.position).normalized, Space.World);
        if(Puppet!= null) { Puppet.GetComponent<Rigidbody>().velocity = Vector3.zero; Puppet.Translate(Speed * Time.deltaTime * (EndLocation - transform.position).normalized, Space.World); }
        transform.Rotate(new(0.5f * Time.deltaTime,0,0));

    }
    public void Move()
    {
        if (traveledDistance > plusMinus)
        {
            traveledDistance = 0;
            if (goingUp) { transform.position = topH; } else { transform.position = bottomH; }
            goingUp = !goingUp;
            
        }

        traveledDistance += (Time.deltaTime * wiggleSpeed * transform.up).magnitude;

        if (goingUp)
        {
            transform.Rotate(0, 0, Time.deltaTime * 1);
            transform.position += Time.deltaTime * wiggleSpeed * transform.up;
        }
        else
        {
            transform.Rotate(0, 0, -1*Time.deltaTime   );
            transform.position -= Time.deltaTime * wiggleSpeed * transform.up;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(EndLocation, Vector3.one);
    }

    public void Crash()
    {
        
        if(!crashing) { PS1.Play(); PS2.Play(); PS3.Play(); if(!(CW==null))CW.ForceDeactivate(); StartCoroutine(FlashRed()); audioSource.PlayOneShot(SelfDestruct);
            audioSource.clip = Engine2;
            audioSource.maxDistance = 2000;
            audioSource.Play();
            CameraTriggerCollider.enabled = true;
          
        }
        crashing=true;
        
    }

    IEnumerator FlashRed()
    {
        while(!Crashed)
        {
            float t = 0f;
            while (t<5f)
            {
               transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, (5.01f - t) / 5f, (5.01f - t) / 5f));

                yield return null;
                t += Time.deltaTime;
            }
            t = 0f;
            while (t < 5f)
            {
                transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, t/5f, t/5f));

                yield return null;
                t += Time.deltaTime;
            }
            transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(1, 1, 1));

            yield return null;

        }
        
    }

  
}
