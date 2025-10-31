using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Spring : MonoBehaviour
{
    public Vector3 upVector;
    private Vector3 currentUpVector;
    public float springForce;
    public float squareMagnitudeRange;
    public playerScript player;
    private bool cooldown;
    public float cooldownTime;
    public float springDuration;
    private Vector3 startingScale;
    private Vector3 endingScale;
    // Start is called before the first frame update
    void Start()
    {   
        startingScale = transform.localScale;
        endingScale = startingScale*1.5f;
        if (springDuration == 0) { springDuration =1; }
        upVector = upVector.normalized;
        transform.rotation = UnityEngine.Quaternion.LookRotation(-Vector3.Cross(transform.forward, upVector), upVector);
    }

   /* private void OnDrawGizmos()
    {
        if (upVector!=currentUpVector) {  
        currentUpVector= upVector;
       
      transform.rotation = UnityEngine.Quaternion.LookRotation(Vector3.Cross(transform.forward, upVector), upVector);
        }
        Gizmos.DrawLine(transform.position, transform.position + upVector);
    }*/

    // Update is called once per frame
    void Update()
    {
        if ((player.transform.position - transform.position).sqrMagnitude < squareMagnitudeRange&&!cooldown&& aboveCheck() && !player.IsSpringing)
        {
            player.Spring(springDuration);
            player.SetIsBouncing(false);
            player.playerBody.AddForce(-player.playerBody.velocity, ForceMode.VelocityChange);

            player.BounceBonus = 1;
            player.playerBody.AddForce(upVector.normalized*springForce,ForceMode.VelocityChange);
           
            GetComponent<AudioSource>().Play(); 
            StartCoroutine(Boing());
            
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Boing()
    {
        float currentBoingTime = 0;
        float maxBoingTime = 0.125f;


        while (currentBoingTime<maxBoingTime) {
            transform.localScale = startingScale + (endingScale - startingScale) * (currentBoingTime / maxBoingTime);
        yield return null;
            currentBoingTime = currentBoingTime+Time.deltaTime;

        }
        currentBoingTime = 0;
        maxBoingTime = maxBoingTime*2;
        while (currentBoingTime < maxBoingTime)
        {
            transform.localScale = endingScale - (endingScale - startingScale) * (currentBoingTime / maxBoingTime);
            yield return null;
            currentBoingTime = currentBoingTime + Time.deltaTime;

        }
    }
    bool aboveCheck()
    {
       
        return transform.TransformVector(player.transform.position-transform.position).y>0.3f;
    }
    IEnumerator Cooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        cooldown = false;
    }
}
