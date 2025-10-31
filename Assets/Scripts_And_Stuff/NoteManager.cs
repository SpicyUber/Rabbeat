using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public Vector3[] notePositions;
    public GameObject notePrefab;
    public CustomGameManager gameManager;
    
 
    private SongKeys songKey;
    

    // Start is called before the first frame update
    void Start()
    {
        songKey = GameObject.FindAnyObjectByType<rhythmSystemScript>().SongKey;
        int i = 0;
        foreach (Vector3 position in notePositions)
        {
           GameObject note = Instantiate(notePrefab, position, Quaternion.identity);
            note.GetComponent<NoteScript>().gameManager = gameManager;
            note.GetComponent<NoteScript>().songKey = (int)songKey;
            if (i % 2 == 0) { note.GetComponent<NoteScript>().Move(); note.GetComponent<NoteScript>().Move(); note.GetComponent<NoteScript>().Move(); note.GetComponent<NoteScript>().Move(); }
                i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        
        if(notePositions != null)
        {
            foreach(Vector3 position in notePositions) {
            Gizmos.color = Color.black;
                Gizmos.DrawSphere(position, 1f);
            }
        }
        
    }

    
}
