using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PrototypeGameStart : MonoBehaviour
{
    public Behaviour[] ToEnable, ToDisable;
    public playerScript Player;
    public rhythmSystemScript RhythmSystemScript; 
    private bool _hasFired = false;
    private float _startVolume = 1f;
    void Update()
    {
        if (_hasFired) return; 
        
        if (Input.GetKeyDown(KeyCode.Space) && Time.timeSinceLevelLoad>2f)
        {
            _hasFired = true;
            RhythmSystemScript.InitializeRhythmSystem();
            foreach (Behaviour b in ToEnable) b.enabled = true;
            foreach (Behaviour b in ToDisable) b.enabled = false;
            Player.transform.Translate(0, 0, 5f);
            Player.GetComponent<PlayerInput>().enabled = true;
            RhythmSystemScript.song.volume = _startVolume;
           
        }
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        foreach (Behaviour b in ToEnable) b.enabled = false;
        Player.GetComponent<PlayerInput>().enabled = false;
       _startVolume =  RhythmSystemScript.song.volume;
        RhythmSystemScript.song.volume = 0;
    }
}
