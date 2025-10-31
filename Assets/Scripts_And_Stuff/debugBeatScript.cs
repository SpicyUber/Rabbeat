using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class debugBeatScript : MonoBehaviour
{
    private rhythmSystemScript rs;
    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        rs = GameObject.FindFirstObjectByType<rhythmSystemScript>();
    }

    // Update is called once per frame
    void Update()
    {
       text.text = rs.beatIndex.ToString();
    }
}
