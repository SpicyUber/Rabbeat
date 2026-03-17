using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{ private Collider _col;
    private List<Rigidbody> _bodies;
    private int _jumpForce=40;
    private AudioSource _audioSource;
    public AudioClip EnterSfx;
    private rhythmSystemScript _rs;
    private float _sfxCooldown;

    private void Start()
    {
        
        _sfxCooldown = 0;

        _bodies = new List<Rigidbody>();

        _audioSource = GetComponent<AudioSource>();

        _rs =  FindAnyObjectByType<rhythmSystemScript>();

        if( _rs != null )
        _rs.BeatChanged.AddListener(
            () => { if (_audioSource!=null ||_audioSource.isPlaying ) return;
                _audioSource.time = 0; _audioSource.Play(); 
            });


    }


    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null) { 
            _bodies.Add(rb); 
            if (rb.CompareTag("Player") && EnterSfx!=null && _sfxCooldown <= 0) 
            { _audioSource?.PlayOneShot(EnterSfx); _sfxCooldown = EnterSfx.length; } 
        }

    }

    private void Update()
    {
        _sfxCooldown -= Time.deltaTime;
        
    }
    void FixedUpdate()
    {
        

      foreach(Rigidbody rb in _bodies ){ if (rb != null) rb.AddForce(transform.up*_jumpForce*(1+ PlayerPrefs.GetInt("JumpLvl"))); }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null) { _bodies.Remove(rb); }
    }

    private void OnDestroy()
    {
        
    }
}
