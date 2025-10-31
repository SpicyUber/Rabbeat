using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NoteScript : MonoBehaviour
{
    private int wiggleSpeed=1;
    public CustomGameManager gameManager;
    private Vector3 topH;
    private Vector3 bottomH;
    private float plusMinus=1.5f;
    private bool goingUp=true;
    private float traveledDistance = 0;
    private bool collected = false;
    public AudioClip sfx;
    public AudioSource audioSource;
    public int songKey;
    int[] pentatonicSemitones;
    int[] majorSemitones;
    public Sparkle SparklePrefab;


    // Start is called before the first frame update
    void Start()
    {
        pentatonicSemitones = new[] { 0, 2, 4, 7, 9 };
        majorSemitones = new[] {0, 2, 4,5,7, 9,11 };
        topH = transform.position+ transform.up*plusMinus/2;
        bottomH = transform.position - transform.up * plusMinus/2;
        transform.position = bottomH;
    }

    // Update is called once per frame
    void Update()
    {

        Move();

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
            transform.position += Time.deltaTime * wiggleSpeed * transform.up;
        }
        else
        {
            transform.position -= Time.deltaTime * wiggleSpeed * transform.up;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) { return; }
       
        if (other.gameObject.name == "Player") { audioSource.pitch = Mathf.Pow(1.059463f, pentatonicSemitones[Random.Range(0,pentatonicSemitones.Length)])* Mathf.Pow(1.059463f, songKey); collected = true; audioSource.PlayOneShot(sfx,0.5f); GameObject.Instantiate(SparklePrefab,transform.position,Quaternion.identity, transform.parent); transform.localScale = Vector3.zero; gameManager.AddNote(); }
    }
}
