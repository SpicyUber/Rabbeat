using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class CircleScript : MonoBehaviour
{ public float linePosition = 0;
    private GameObject beatLine;
    private RectTransform rT;
    public float beatNumber = 0;
    private float rightLimit = 10000;
    private rhythmSystemScript rs;
    private float uiOffset = 0.35f;
    private float startPos = 0;
    private float existanceLength = 3000;
    private float lineY;
    public Image SpriteThingy;
    public CircleSpawner CircleSpawnerComponent;
    
   
    // Start is called before the first frame update
    void Start()
    {  
        SpriteThingy = GetComponent<Image>();
        beatLine = GameObject.Find("BeatLine");
        rT = beatLine.GetComponent<RectTransform>();
        linePosition = rT.position.x;
        lineY = rT.position.y;
        rs = GameObject.FindFirstObjectByType<rhythmSystemScript>();
        rightLimit = transform.position.x + 2 * transform.position.x;
        startPos = GoalPosition();
        Debug.Log("SPAWNED! " + beatNumber);
    }

    private float GoalPosition()
    {
        if (beatNumber <= -1) { return linePosition; }
        if (linePosition < 0) { return ((-(beatNumber - rs.debugBeatIndex) / 1) + 1 + uiOffset) * linePosition; } else {
            return ((-(rs.debugBeatIndex  - beatNumber) / 1) + 1 + uiOffset) * linePosition; }

    }

    // Update is called once per frame
    void Update()
    {
        if(CircleSpawnerComponent != null) 
        SpriteThingy.sprite = CircleSpawnerComponent.CurrentSprite;
        transform.position = new Vector3(GoalPosition(),lineY,transform.position.z);
        if (Mathf.Abs(transform.position.x-startPos)>=existanceLength&&beatNumber>=0)
        {
            this.gameObject.SetActive(false);
           Destroy(this.gameObject);
        }
        
    }

    
}
