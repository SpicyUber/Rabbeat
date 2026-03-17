using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class RobotYapHandler : MonoBehaviour
{
     public basicEnemy[] Robots;
    public AudioClip YapClip;
    public AudioClip[] AttackClip;
    private bool _isYapping = false;
    private int positionIndex;
    private bool _undisturbed = true;
    public GameObject Player;
    public AudioSource MusicAudio;
    private bool _timeout;
    private float _startMusicVolume;
    private float _range=70f;
    private bool _firstYap=true;
    private int? lastClip = null;

    public bool IdleYapOn;

    // Start is called before the first frame update
    void Start()
    {
        _startMusicVolume= MusicAudio.volume;
    }

    // Update is called once per frame
    void Update()
    {

        if (Robots[positionIndex] == null || Robots[positionIndex].IsDying ) { GetComponent<AudioSource>().Stop(); }
        if (AreIdle() && !GetComponent<AudioSource>().isPlaying) { _timeout = false; }
        if (!GetComponent<AudioSource>().isPlaying) { EveryoneShutUp(); }
        if (AllDead()) {  if (GetComponent<AudioSource>().isPlaying) { GetComponent<AudioSource>().Stop(); } return; }
     if(Robots[positionIndex]!=null) transform.position = Robots[positionIndex].transform.position; else StickToRandomRobot();
            if (IdleYapOn) { 
        GetComponent<AudioSource>().volume = Math.Clamp((1.0f - ((Player.transform.position - transform.position).magnitude / _range))*2f, 0f, 1.0f);
        if((Player.transform.position - transform.position).magnitude<= _range && _undisturbed)
        MusicAudio.volume = Math.Clamp(((Player.transform.position - transform.position).magnitude* _startMusicVolume / _range), _startMusicVolume/50, _startMusicVolume);else if((Player.transform.position - transform.position).magnitude<= _range) { MusicAudio.volume = _startMusicVolume; }
        }
        if (_undisturbed && !AreIdle())
            _undisturbed = false;
        _isYapping = GetComponent<AudioSource>().isPlaying;
      //  if (Robots[positionIndex] != null)
        
        if(!_isYapping) {

            if(AreIdle() && _undisturbed) { 
            //StartIdleYap();
            }
            else if (!AreIdle() && !_timeout) {
                  
                    StartCoroutine(TimeoutThenAction(StartAttackYap,1f));
            }

        }
        
    }

    private void EveryoneShutUp()
    {
        foreach(basicEnemy r in Robots)
        {
            if(r != null) { 
            r.IsTalking = false;
            }
        }
    }

    private void StartAttackYap()
    {
        if (AllDead()) {return; }
        
        if ( GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Stop();
        int number = Random.Range(0, AttackClip.Length);
        if (lastClip == null || lastClip != number || AttackClip.Length < 2) { 
            GetComponent<AudioSource>().clip = AttackClip[number];
        }
        else if (number == AttackClip.Length - 1)
        {
            number--;
            GetComponent<AudioSource>().clip = AttackClip[number ];
        }
        else {
            number++;
            GetComponent<AudioSource>().clip = AttackClip[number]; }
        Debug.Log("Litte db flag"); StickToRandomRobot(); GetComponent<AudioSource>().Play();
        lastClip = number;
       
    }

    private bool AllDead()
    {
        if(Robots.Length == 0) { return true; }

        for (int i = 0; i < Robots.Length; i++)
        {
            if (Robots[i] != null && Robots[i].IsDying==false) return false;

        }

        return true;
    }

    private void StickToRandomRobot()
    {

        
        int temp =Random.Range(0, Robots.Length);
        if(Robots[temp]==null) {
            for (int i = 0; i < Robots.Length; i++)
            {
                if (Robots[i] != null && Robots[i].currentState!=enemyScript.state.IDLE) { positionIndex = i; return; }

            }
        }
        Robots[positionIndex].IsTalking= true;
        
    }

    private void StartIdleYap()
    {
        if (!IdleYapOn) return;
        GetComponent<AudioSource>().clip = YapClip;
        GetComponent<AudioSource>().Play();
    }

    IEnumerator TimeoutThenAction(Action function, float seconds)
    { if (_timeout) { yield return null; } else {
            _timeout = true;
            
            yield return new WaitForSeconds(seconds);
            function();

            // _timeout = false;
        }
    }
    private bool AreIdle()
    {
      
        for(int i = 0;i<Robots.Length ;i++)
        {
            if (Robots[i] == null) continue;
            if (Robots[i].currentState != enemyScript.state.IDLE)
            {
                return false;
            }

        }

        return true;

    }


}
