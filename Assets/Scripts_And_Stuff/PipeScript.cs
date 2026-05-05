using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    private float _pipeLength = 2f;
    private float _turningAdjustment = 0.45f;

    private Vector3 _positionNeedle;
    private Vector3 _directionNeedle;//The needles keep track of where the next pipe part will be spawned and how it will be rotated LOCALLY;

    public GameObject[] Pipes;
    public Texture2D[] PipeTexture;

    public LinkedList<GameObject> PlacedPipes;
    public GameObject PipeRing;

    public Material ExpandMaterial;
    public GameObject Player;

    private bool _playerIsIn = false;
    private bool _pipeStunned;

    private float _pipeSpeed = 1f;
    private float _progression = 0f;

    private LinkedListNode<GameObject> NextStop;

    public ParticleSystem Particles;
    public ParticleSystem ParticlesExit;

    public AudioClip PlugIn;
    public AudioClip PlugOut;

    public AudioClip[] PumpSound;
    //left and right rotate the pipe direction by 45 degree angles.
    public enum PipeDirection
    {
        Forward, Left, Right
    }

    public PipeDirection[] DirectionSequence;
    private Coroutine slowDownCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        PlacedPipes = new LinkedList<GameObject>();
        GeneratePipe();
        NextStop = PlacedPipes.First;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Particles.Play();
            if(_playerIsIn == false)
            {
                GetComponent<AudioSource>().Play();
                GetComponent<AudioSource>().PlayOneShot(PlugIn);
            }
            _playerIsIn = true;
            Player.GetComponent<playerScript>().RabbitModel.SetActive(false);
            Player.GetComponent<playerScript>().Headset.gameObject.SetActive(false);
            Player.GetComponent<playerScript>().IsInPipe = true;
            Player.GetComponent<playerScript>().CurrentPipe = this;
            _pipeSpeed = 1f;


            Player.GetComponent<playerScript>().playerBody.velocity = Vector3.zero;
            Player.gameObject.transform.position = NextStop.Value.transform.position;
            DisableCollision();
        }
    }

    private void DisableCollision()
    {
        foreach(GameObject go in PlacedPipes)
        {
            go.GetComponent<MeshCollider>().enabled = false;
        }
    }

    private void EnableCollision()
    {
        foreach(GameObject go in PlacedPipes)
        {
            go.GetComponent<MeshCollider>().enabled = true;
        }
    }

    private void GeneratePipe()
    {
        _positionNeedle = transform.position;
        _directionNeedle = transform.rotation * Vector3.forward;
        AddPipe(0, 0);
        _positionNeedle = _positionNeedle + _directionNeedle * _pipeLength;
        AddRing();
        foreach(PipeDirection direction in DirectionSequence)
        {
            switch(direction)
            {
                case PipeDirection.Forward:
                    AddPipe(1, 0);
                    _positionNeedle = _positionNeedle + _directionNeedle * _pipeLength;
                    break;
                case PipeDirection.Left:

                    AddPipe(2, -90);
                    _positionNeedle = _positionNeedle + _directionNeedle * _pipeLength * 0.74f;
                    _directionNeedle = Quaternion.Euler(0, -45, 0) * _directionNeedle;
                    _positionNeedle = _positionNeedle + _directionNeedle * _turningAdjustment;
                    break;
                case PipeDirection.Right:

                    AddPipe(2, 90);
                    _positionNeedle = _positionNeedle + _directionNeedle * _pipeLength * 0.74f;
                    _directionNeedle = Quaternion.Euler(0, +45, 0) * _directionNeedle;
                    _positionNeedle = _positionNeedle + _directionNeedle * _turningAdjustment;

                    break;
            }
            AddRing();
        }
        AddPipeFlipped(0, 0);
        _positionNeedle = _positionNeedle + _directionNeedle * _pipeLength;
        ParticlesExit.transform.position = _positionNeedle;
        AddCollision();
    }

    private void AddCollision()
    {
        foreach(GameObject go in PlacedPipes)
        {
            MeshCollider mc = go.AddComponent<MeshCollider>();
            mc.convex = true;
        }
    }

    private void AddPipe(int pipeIndex, float pipeRotationAroundItselfInDegrees)
    {
        GameObject pipeObject = GameObject.Instantiate(Pipes[pipeIndex], _positionNeedle, Quaternion.LookRotation(_directionNeedle, transform.up), this.transform);

        ExpandMaterial.mainTexture = PipeTexture[pipeIndex];
        Material[] newMaterials = new Material[1];
        newMaterials[0] = ExpandMaterial;
        pipeObject.GetComponent<MeshRenderer>().materials
         = newMaterials;


        if(pipeRotationAroundItselfInDegrees != 0)
        {
            pipeObject.transform.Rotate(new(0, 0, pipeRotationAroundItselfInDegrees), Space.Self);
        }
        PlacedPipes.AddLast(pipeObject);

    }

    private void AddPipeFlipped(int pipeIndex, float pipeRotationAroundItselfInDegrees)
    {
        GameObject pipeObject = GameObject.Instantiate(Pipes[pipeIndex], _positionNeedle, Quaternion.LookRotation(_directionNeedle, transform.up), this.transform);
        ExpandMaterial.mainTexture = PipeTexture[pipeIndex];
        Material[] newMaterials = new Material[1];
        newMaterials[0] = ExpandMaterial;
        pipeObject.GetComponent<MeshRenderer>().materials
         = newMaterials;
        if(pipeRotationAroundItselfInDegrees != 0)
        {
            pipeObject.transform.Rotate(new(0, 0, pipeRotationAroundItselfInDegrees), Space.Self);
        }
        pipeObject.transform.Rotate(new(transform.rotation.x, transform.rotation.y + 180, transform.rotation.z), Space.Self);
        pipeObject.transform.position += _pipeLength * _directionNeedle;

        PlacedPipes.AddLast(pipeObject);
    }
    private void AddRing()
    {
        return;
        GameObject ring = GameObject.Instantiate(PipeRing, _positionNeedle, Quaternion.LookRotation(_directionNeedle, transform.up), this.transform);
        ring.transform.localScale = Vector3.one * 1.2f;
    }

    public void Pump()
    {
        _pipeSpeed += 2;
        if(_pipeSpeed > 50) { _pipeSpeed = 50; }
        GetComponent<AudioSource>().PlayOneShot(PumpSound[UnityEngine.Random.Range(0, 4)]);
    }


    void Update()
    {

        if(_pipeSpeed > 2f)
        {
            _pipeSpeed = _pipeSpeed - 0.5f * Time.deltaTime;
            if(_pipeSpeed < 2f && !_pipeStunned) _pipeSpeed = 2f;

        }



        if(_playerIsIn)
        {
            if(NextStop == null) { EjectPlayer(); }
            else
            {
                Player.GetComponent<playerScript>().transform.position = Vector3.Lerp(NextStop.Value.transform.position, NextStop.Next.Value.transform.position, _progression);
                while(_progression > 1)
                {
                    NextStop = NextStop.Next;
                    if(NextStop.Next != null)
                    {
                        _progression--;
                        Player.GetComponent<playerScript>().transform.position = Vector3.Lerp(NextStop.Value.transform.position, NextStop.Next.Value.transform.position, _progression);
                    }
                    else
                    {
                        Player.GetComponent<playerScript>().transform.position = NextStop.Value.transform.position;
                        EjectPlayer(); break;
                    }
                }

                _progression += _pipeSpeed * Time.deltaTime;
            }

        }
        else
        {
            NextStop = PlacedPipes.First;
        }

        LinkedListNode<GameObject> temp = PlacedPipes.First;
        while(temp != null)
        {
            Vector4 playerPosVector = (_playerIsIn) ? new(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z, 1) : Vector4.zero;

            temp.Value.GetComponent<MeshRenderer>().materials[0].SetVector("_PlayerPosition", playerPosVector);
            temp = temp.Next;
        }
    }

    private void EjectPlayer()
    {
        _playerIsIn = false;
        Player.GetComponent<playerScript>().RabbitModel.SetActive(true);
        Player.GetComponent<playerScript>().IsInPipe = false;
        Player.GetComponent<playerScript>().CurrentPipe = null;
        _pipeSpeed = 2f;
        Player.GetComponent<playerScript>().playerBody.velocity = Vector3.zero;
        Player.GetComponent<playerScript>().playerBody.AddForce(_directionNeedle * 5000f);
        EnableCollision();
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(PlugOut);
        Player.GetComponent<playerScript>().Headset.gameObject.SetActive(true);
        ParticlesExit.Play();
    }

    private void OnDrawGizmos()
    {

    }

    public void SlowDown()
    {
        if(slowDownCoroutine != null) { StopCoroutine(slowDownCoroutine); }
        slowDownCoroutine = StartCoroutine(SlowDownCoroutine());

    }

    IEnumerator SlowDownCoroutine()
    {
        _pipeStunned = true;
        _pipeSpeed = 0.25f;
        yield return new WaitForSeconds(Player.GetComponent<playerScript>().StunDuration);
        _pipeSpeed = 2f;
        _pipeStunned = false;
    }
}
