using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrail : MonoBehaviour
{
    // Start is called before the first frame update
    public TrailRenderer trailRenderer;
    public playerScript player;
    public float trailDurationInSeconds;
    private bool lockBool = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.IsTumbling() && (player.isGrounded() || player.IsBouncing()))
        {
            lockBool = false;
            trailRenderer.emitting = false;
        }
        else if(!trailRenderer.emitting && !lockBool)
        {

            trailRenderer.emitting = true;
            StartCoroutine(TrailTimer());
        }
    }

    IEnumerator TrailTimer()
    { float totalTime = 0;

        while (totalTime < trailDurationInSeconds && trailRenderer.emitting)
        {

            yield return null;
            totalTime += Time.deltaTime;

        }
        trailRenderer.emitting = false;
        lockBool = true;

    }
}
