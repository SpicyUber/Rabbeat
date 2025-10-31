using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dustTrailScript : MonoBehaviour
{
    private Vector3 startingPos;
    private bool groundFound;
    private float maxDistance = 100f;
    private int prevBeat=-1;
    public playerScript player;
    public rhythmSystemScript rs;
    public ParticleSystem dust;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.parent.position;
        if(player == null) { throw new UnityException("Player is null"); }
        if(rs ==null) { throw new UnityException("rs is null"); }

    }

    // Update is called once per frame
    void Update()
    {
        if (rs.beatIndex != prevBeat)
        {
          //  Debug.Log("player.playerBody.velocity.magnitude>0.1f&&player.isGrounded():" + (player.playerBody.velocity.magnitude > 0.1f) + "and" + player.isGrounded());
            if (player.playerBody.velocity.magnitude>0.1f&&player.isGrounded()) { 
            dust.Play();
               // Debug.Log("small dust");
            }
            prevBeat = rs.beatIndex;
        }
        groundFound = false;

        RaycastHit[] hits = Physics.SphereCastAll(transform.parent.position + transform.parent.transform.up * 2, 0.4f, (-1) * transform.parent.up, maxDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Ground")
            {
                transform.position = hit.point + hit.normal / 2;
                this.transform.up = hit.normal;
                transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x - 90, transform.rotation.y, transform.rotation.z));



                groundFound = true;
                break;
            };

        }

    }

   
    private float currentDistance()
    {
        return Vector3.Distance(transform.position, transform.parent.position);
    }
}
