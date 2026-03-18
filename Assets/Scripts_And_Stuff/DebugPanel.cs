using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public TextMeshProUGUI DebugText;
    private bool _on = false;
    private rhythmSystemScript _rs;
    // Start is called before the first frame update
    void Start()
    {
        _rs = FindAnyObjectByType<rhythmSystemScript>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) _on = !_on;

        DebugText.text = _on ? 
            
            $"DEBUG [PRESS 1 TO TOGGLE]\n" +
            $"song time:{_rs.song.time}\n" +
            $"beat index: {_rs.beatIndex}\n" +
            $"beat index before rounding: {_rs.debugBeatIndex}\n"



            : "";
    }
}
