using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private rhythmSystemScript rs;
    private playerScript ps;
    private bool retracted=false;
    public float RetractDuration;
    public float PokeDuration;
    public float UpDownDistance;
    private float endScale;
    public float Offset;
    public Coroutine RetractRoutineVar;
    public Coroutine PokeRoutineVar;
    private bool hurts=false;
    private float RetractedZ =0f;

    // Start is called before the first frame update
    void Start()
    {

        endScale = transform.localScale.z;
        if (UpDownDistance<0f)
        {
            UpDownDistance = 0f;
        }
        
        if (RetractDuration <= 0) RetractDuration = 0.01f;
        if (RetractDuration <= 0) PokeDuration = 0.01f;
        ps = GameObject.FindFirstObjectByType<playerScript>();
        rs = GameObject.FindFirstObjectByType<rhythmSystemScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rs == null || ps==null) { return; }
        if(hurts && ps.isGrounded() && (ps.transform.position-transform.parent.position).magnitude<3f && Vector3.Dot((ps.transform.position - transform.parent.position).normalized,-transform.forward)<0f){ ps.Hurt(); ps.Knockback(transform.forward.normalized,80f); }
        if (rs.beatMap[rs.beatIndex].isActive && !retracted) { retracted = true; Retract();   }
        if(!rs.beatMap[rs.beatIndex].isActive && retracted) { retracted = false; Poke(); }
    }

    private bool ConeShapeCheck()
    {
        Vector3 player2Spike = transform.InverseTransformPoint(ps.transform.position - transform.position);
        //   player2Spike.z = 0;
        return Mathf.Abs(player2Spike.y) < 2.8f && Mathf.Abs(player2Spike.x) < 2.8f;
    }

    void Retract()
    {
        hurts = false;
        if (PokeRoutineVar != null) { StopCoroutine(PokeRoutineVar);  }
        if(RetractRoutineVar!= null) { StopCoroutine(RetractRoutineVar); }
        transform.localScale = new(transform.localScale.x, transform.localScale.y, endScale);
      //  transform.localPosition=new(transform.localPosition.x, transform.localPosition.y, 0f);
        RetractRoutineVar = StartCoroutine(RetractRoutine());

    }
    IEnumerator RetractRoutine( )
    {
        yield return new WaitForSeconds(Offset);
        float t = 0;
        while (t<RetractDuration)
        {
            transform.localScale = new(transform.localScale.x, transform.localScale.y,endScale-endScale * t / RetractDuration);
           // transform.localPosition=new(transform.localPosition.x, transform.localPosition.y, RetractedZ * t / PokeDuration);
            yield return null;
            t += Time.deltaTime;
        }
        transform.localScale = new(transform.localScale.x, transform.localScale.y, 0); 
    }
    void Poke()
    {
        if (PokeRoutineVar != null)
        {
            StopCoroutine(PokeRoutineVar);
        }
            if (RetractRoutineVar!=null) { StopCoroutine(RetractRoutineVar);  }
        transform.localScale = new(transform.localScale.x, transform.localScale.y, 0);
       // transform.localPosition=new(transform.localPosition.x, transform.localPosition.y, RetractedZ);
        PokeRoutineVar = StartCoroutine(PokeRoutine());
    }
    IEnumerator PokeRoutine()
    {
        yield return new WaitForSeconds(Offset);
        hurts = true;
        float t = 0;
        while (t < PokeDuration)
        {
            transform.localScale = new(transform.localScale.x, transform.localScale.y, 0 + endScale * t / PokeDuration);
          //  transform.localPosition=new(transform.localPosition.x, transform.localPosition.y, RetractedZ - RetractedZ*t/PokeDuration);
            yield return null;
            t += Time.deltaTime;
        }
        transform.localScale = new(transform.localScale.x, transform.localScale.y,   endScale  );
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,0.5f);
       // Gizmos.DrawSphere(transform.position, 4f);
    }
}
