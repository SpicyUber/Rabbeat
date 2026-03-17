using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatWall : MonoBehaviour
{
    private Material[] _wallMaterials;
    private GameObject[] _walls;
    public GameObject Player;
    private bool _isActive;
    public GameObject[] EnemyList;
    public AudioClip Up;
    public AudioClip Down;
    private AudioSource _source;
    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();
        _isActive = false;
        _wallMaterials = new Material[transform.childCount];
        _walls= new GameObject[transform.childCount];
        WallsInit(); 
    }

   private void WallsInit()
    {
        for (int i = 0; i < _wallMaterials.Length; i++)
        {
            _walls[i] = transform.GetChild(i).gameObject;
            _wallMaterials[i] = transform.GetChild(i).GetComponent<MeshRenderer>().materials[0];
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        foreach (Material m in _wallMaterials)
        {
            if (_isActive == false) return;
            m.SetVector("_PlayerPosition", new Vector4(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z, 0) );
        }
        bool takeDownBarrier = true;
        foreach(GameObject g in EnemyList)
        {
            if(g!=null)takeDownBarrier = false;
        }
        if(takeDownBarrier) { StartCoroutine(Deactivate()); }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isActive)
        {
            StartCoroutine(Activate());

            Destroy(GetComponent<BoxCollider>());
        }
    }
    IEnumerator Activate()
    {
        _source.PlayOneShot(Up);
        float time = 0f;
        foreach (GameObject go in _walls)
        {
            go.SetActive(true);
        }
        _isActive = true;
        while (time < 3f)
        {
            foreach(Material m in _wallMaterials)
            {
                m.SetFloat("_Value", time/3);
            }

            yield return null;
            time += Time.deltaTime;
        }
        foreach (Material m in _wallMaterials)
        {
            m.SetFloat("_Value", 1f);
        }

        
    }

    public void ForceDeactivate() { StartCoroutine(Deactivate()); }
    IEnumerator Deactivate()
    {
        if (_isActive) { 
        _isActive = false;
        _source.PlayOneShot(Down,0.75f);
        float time = 0f;
        while (time < 3f)
        {
            foreach (Material m in _wallMaterials)
            {
                m.SetFloat("_Value", 1f-time/3);
            }

            yield return null;
            time += Time.deltaTime;
        }
        foreach (Material m in _wallMaterials)
        {
            m.SetFloat("_Value", 0f);
        }
        foreach (GameObject go in _walls)
        {
            go.SetActive(false);
        }
        
        Destroy(gameObject);
        }
    }
}
