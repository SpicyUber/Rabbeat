using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dancingPlatform : MonoBehaviour
{
    public CinemachineBrain cameraBrain;
    private rhythmSystemScript rs;
    public Vector3[] worldSpacePoints;
    private int currentPointIndex=0;
    private int lastBeat;
    public float transitionSpeedInSeconds = 0.15f;
    public int skipBeats;
    private int currentSkippedBeats;
    bool lockFlag = false;
    bool reverse = false;
    public bool useCustomBeatMap;
    public int[] customMap;
    public bool useEveryNth;
    public int n;
    private GameObject[] puppets;

    // Start is called before the first frame update
    void Start()
    {   if (cameraBrain == null) throw new UnityException("cameraBrain cannot be null");
        puppets = new GameObject[25];
        if(worldSpacePoints.Length == 0) { throw new UnityException("worldSpacePoints MISSING!"); } else { 
        transform.localPosition = worldSpacePoints[0];}
        rs = GameObject.FindFirstObjectByType<rhythmSystemScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((rs.beatMap[rs.beatIndex].isActive && !useCustomBeatMap&& !useEveryNth  ) && rs.beatIndex!=lastBeat) {
            lockFlag = false;
            GoNext();
            lastBeat = rs.beatIndex;
        }else if ((IsBeatIndexInCustomMap() && useCustomBeatMap) && rs.beatIndex != lastBeat) {
            lockFlag = false;
            GoNext();
            lastBeat = rs.beatIndex;
        }else if ((IsBeatIndexNth() && useEveryNth)&& rs.beatIndex != lastBeat)
        {
            lockFlag = false;
            GoNext();
            lastBeat = rs.beatIndex;
        }
    }
    private bool IsBeatIndexNth()
    {
        return rs.beatIndex % n == 0;

    }
    private bool IsBeatIndexInCustomMap()
    {
        if (customMap.Length < 1) { return false; }

        for(int i = 0; i < customMap.Length; i++)
        {
            if(rs.beatIndex== customMap[i]) { return true; }
        }

        return false;


    }
    private void GoNext()
    {
        if (currentSkippedBeats < skipBeats) { currentSkippedBeats++; } else {
            currentSkippedBeats = 0;
        lockFlag = true;
        
        StartCoroutine(GoNextCoroutine());

        }


    }

    IEnumerator GoNextCoroutine() {
        float currentTime = 0f;
       
        Vector3 startingPos = transform.localPosition;
        IncrementPointIndex();
        while (lockFlag==true && currentTime<transitionSpeedInSeconds) {
            Vector3 previousPos = transform.localPosition;
            transform.localPosition = Vector3.Lerp(startingPos, worldSpacePoints[currentPointIndex], currentTime / transitionSpeedInSeconds);
            foreach (GameObject puppet in puppets) {
                if(puppet != null) { 
                  
            puppet.transform.position = puppet.transform.position + (transform.localPosition- previousPos) ;
                }
            }
            
            yield return null;
            currentTime += Time.deltaTime; }

        lockFlag = false;
        
    }
   private  void IncrementPointIndex() {
        if (!reverse) { 
        currentPointIndex++;
        }else { currentPointIndex--; }

        if (currentPointIndex >= worldSpacePoints.Length)
        {
            reverse = true;
            currentPointIndex = worldSpacePoints.Length - 2;
        }

        if(currentPointIndex < 0) { reverse=false; currentPointIndex = 1; }

    }

   /* void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Col Enter " + collision.gameObject.name);
        if (collision.gameObject.name == "Player" || collision.gameObject.tag == "Enemy")
        {
            for (int i = 0; i < puppets.Length; i++)
            {
                if (puppets[i]==null) puppets[i] = collision.gameObject;
                break;
            }

        } 
    }*/

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Col Enter " + collision.gameObject.name);
        Debug.Log("puppets " + puppets);
        if (collision.gameObject.name == "Player" || collision.gameObject.tag.Equals("Enemy"))
        { 
            for (int i = 0; i < puppets.Length; i++)
            {
                if (puppets[i] == null)
                {
                    if (collision.gameObject.tag.Equals("Enemy"))
                    {
                        if (Exists(collision.gameObject.transform.parent.gameObject)) return;
                        puppets[i] = collision.gameObject.transform.parent.gameObject;
                        break;
                    }else
                    {
                        if (Exists(collision.gameObject)) return;
                        cameraBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
                        puppets[i] = collision.gameObject;
                        break;
                    }
                }
            }

        }
    }

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("Col Exit" + collision.gameObject.name);
        for (int i = 0; i < puppets.Length; i++)
        {
            if (puppets[i] == null) continue;
            if (collision.gameObject == puppets[i])
            {   if (collision.gameObject.name.Equals("Player")) { cameraBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate; }
                puppets[i] = null;
                break;
            }
        }

    }

    private bool Exists(GameObject gameObject)
    {
        for (int i = 0; i < puppets.Length; i++) {
            if (puppets[i] == gameObject) return true;
        }


        return false;


    }
    /* private void OnCollisionExit(Collision collision)
     { Debug.Log("Col Exit"+collision.gameObject.name);
         for(int i = 0; i <puppets.Length; i++) {
             if (puppets[i]==null) continue;
         if (collision.gameObject == puppets[i])
         {
                 puppets[i] = null;
                 break;
         }
         }
     }*/
}
