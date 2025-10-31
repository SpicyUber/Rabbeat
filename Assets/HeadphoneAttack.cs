using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeadphoneAttack : MonoBehaviour
{
    public Queue<enemyScript> Enemies;
    public GameObject ScanPrefab;
    public GameObject MarkPrefab;
    private SongKeys _key;
    public float ScanSpeed;
    private State _currentState=State.SCANNING;
    private rhythmSystemScript _rs;
    private float t = 0;
    public GameObject EnergyBall;
   
    private int _rounds;
    private Vector3 _startingPos;
    private Vector3 _endPos;
    public GameObject HeadphoneProjectile;
    private bool _collided = false;
    private bool _scanDone = false;
    private bool _goingUp = true;
    private float _wiggleUpDownTime=0;
    private Vector3 _startingLocalPos = Vector3.zero;
    private enum State { POSITIONING ,LOADED, SCANNING,WAITFORSCAN, NOTHING }
    // Start is called before the first frame update
    void Start()
    { Enemies = new Queue<enemyScript>();
        GameObject.Instantiate(EnergyBall,transform.position,Quaternion.identity);
        _startingPos = transform.position;
        _endPos = _startingPos+(transform.forward * 5 + transform.up).normalized * 15f;
        _currentState = State.POSITIONING;
        _startingLocalPos = transform.localPosition;
        _rounds = 4 * (1 + PlayerPrefs.GetInt("AttackLvl") );
      _rs= GameObject.FindAnyObjectByType<rhythmSystemScript>();
        _rs.AddSubscription(DoSomething);
        _key = _rs.SongKey;
    }

    private void OnDestroy()
    {
        _rs.RemoveSubscription(DoSomething);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
            int sign = (_goingUp) ? 1 : -1;
            transform.localPosition=transform.localPosition + Vector3.up*1f*Time.deltaTime*sign ;
         if(_wiggleUpDownTime > 0.5f) { _goingUp = !_goingUp; _wiggleUpDownTime= 0; }
        

       

       

        switch (_currentState)
        {
           
            case State.POSITIONING:
                //  transform.position+= (transform.forward*5+transform.up).normalized * Time.deltaTime * 60f;
                float interpTime = (1f - t/2f) * (1f - t/2f);
                if (t / 2f >= 0.99f) { _currentState = State.WAITFORSCAN; }
                if (!_collided) transform.position=   Vector3.Lerp(_startingPos, _endPos, 1-interpTime);
                t += Time.deltaTime;
              
                break;
            case State.NOTHING:
                
                break;
                default:

                break;
        }

       
        _wiggleUpDownTime += Time.deltaTime;
       
    }

    public void DoSomething()
    {
        switch (_currentState)
        {
            case State.WAITFORSCAN:
                if (_rs.beatIndex % 16 == 0 && _rs.beatMap[_rs.beatIndex].isActive) { ShootScan(); }
                break;
            case State.SCANNING:
                if (_rs.beatIndex % 8 == 0 && _rs.beatMap[_rs.beatIndex].isActive && _scanDone)
                {

                    _currentState = State.LOADED;
                }
                break;
            case State.LOADED:
                if (_rs.beatMap[_rs.beatIndex].isActive)
                {
                   // Debug.Log(_rs.beatIndex);
                    ShootEnemies();

                }

                break;
                default : break;
        }

        }

    private void ShootEnemies()
    {
        
        if (Enemies.Count == 0) { _rounds--; if (_rounds < 1) { Destroy(gameObject); _currentState = State.NOTHING; return; } _currentState = State.WAITFORSCAN; return; }
        
        while (Enemies.Count > 0)
        {
          enemyScript e =  Enemies.Dequeue();
            if (e != null) {   Instantiate(HeadphoneProjectile, transform.position, Quaternion.LookRotation(e.transform.position - transform.position), transform.parent).GetComponent<HeadphoneProjectile>().Fire(e); return; }

        }
       
        }

    public void ScanIsDone()
    {
        _scanDone = true;
    }
    private void ShootScan()
    {
        _scanDone = false;
        Enemies = new Queue<enemyScript>();
        Instantiate(ScanPrefab,transform).GetComponent<ScanWave>().Shoot((int)_key,this,ScanSpeed);
       
        _currentState = State.SCANNING;
    }

    public void AddMark(enemyScript e)
    {
        if(e!=null)
        Instantiate(MarkPrefab, e.transform).GetComponent<Mark>().Init(e);

    }

   

    public void OnTriggerEnter(Collider other)
    {
        if(_currentState == State.POSITIONING && !other.CompareTag("Player") && !other.CompareTag("Collectable") && !other.CompareTag("Hole") && !other.CompareTag("CameraTrigger")  && !other.CompareTag("HintPanel") && !other.CompareTag("DodgeBall"))
        {
            _collided = true;
        }
    }
}

