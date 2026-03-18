using System.Collections;
using UnityEngine;

public class BirdBot : enemyScript
{
    public AudioClip LockSound, FireSound, FlyingSound;
    private AudioSource _as;
    public AudioSource _flyingSound;
    public int AttackRadius;
    private bool switchBool=false;
    public Vector3 location1,location2;
    public LineRenderer BirdLine;
    public float ReloadSpeed;
    private bool loaded = false;
    private bool isShooting = false;
    private bool isShootingLocked = false;
    private int loadedOnBeat;
    public GameObject Explosion;
    private GameObject _explosion;
    public Color colorAggro, colorShoot;
    private float _time = 0f;
    private float _noiseRange = 20f;
    public float MaxY;
    private float _defaultMaxY = 200f;
    public float FollowRange = 50f;
    public override void Aggro()
    {
        if ((location1-transform.position).magnitude>FollowRange) { currentState = state.IDLE; return; }
        if ((player.transform.position - transform.position).magnitude > 50f)
        {
            _flyingSound.volume = 0f;
        }
        else _flyingSound.volume = 0.35f;

        // if(player.transform.position.y>MaxY) { currentState = state.IDLE; return; }
        if (transform.position.y > MaxY) { transform.Translate(Vector3.down * 150f * Time.deltaTime, Space.World); }  
        BirdLine.enabled = !loaded;
        if (loaded == false && rs.beatMap[rs.beatIndex].isActive && !isShooting) { loaded = true; _as.PlayOneShot(LockSound,1f); loadedOnBeat = rs.beatIndex; }

        if (!loaded && !isShootingLocked)
        {
            BirdLine.SetColors(colorAggro, colorAggro);

        }
        else { BirdLine.SetColors(colorShoot, colorShoot); }
        BirdLine.SetPosition(0, transform.position);
        if (!isShootingLocked)
        {
            BirdLine.SetPosition(1, player.transform.position);
            transform.LookAt(player.transform.position);

            if ((player.transform.position - transform.position).magnitude < 10f)
            {
                transform.Translate(5f * Time.deltaTime * -(player.transform.position - transform.position).normalized, Space.World);
            }
            else if ((player.transform.position - transform.position).magnitude > 20f)
            {
                transform.Translate((player.transform.position - transform.position).normalized * 25f * Time.deltaTime, Space.World);
            }
            if ((transform.position - player.transform.position).y < 7f && !player.GetComponent<playerScript>().IsAttacking())
            {
                transform.Translate(Vector3.up * 5f * Time.deltaTime, Space.World);

            }

            foreach (Collider col in Physics.OverlapSphere(transform.position, 5f))
            {
                if (col.CompareTag("Enemy")) { transform.Translate((transform.position - col.transform.position).normalized * 12.5f * Time.deltaTime, Space.World); break; }
            }
        }
        if (loaded && rs.beatMap[rs.beatIndex].isActive && loadedOnBeat != rs.beatIndex) { loaded = false; StartCoroutine(Shoot()); }

    
    }
    private IEnumerator Shoot()
    {
        if (isShooting) { yield return null; } else { isShooting=true; isShootingLocked = true; 
        Vector3 pos = ShotActualHitPosition();
            transform.LookAt(player.transform.position);
            BirdLine.SetPosition(1, pos);
            
            BirdLine.SetWidth(1f, 1f);
            _as.PlayOneShot(FireSound,0.25f);
            yield return new WaitForSeconds(0.15f);
            
           
        foreach (Collider col in Physics.OverlapSphere(pos, 1f))
        {
            if (col.gameObject.name == "Player") { player.Hurt(); break; }
            
        }
            yield return new WaitForSeconds(0.175f);
            BirdLine.SetWidth(0f, 0f);
        yield return new WaitForSeconds(ReloadSpeed/2);
            isShootingLocked = false;
            BirdLine.SetPosition(1, player.transform.position);
            transform.LookAt(player.transform.position);
            BirdLine.SetWidth(0.2f, 0.2f);
            yield return new WaitForSeconds(ReloadSpeed/2);
            isShooting = false;
        }
    }

    private Vector3 ShotActualHitPosition()
    {
        RaycastHit hit;
      return (  Physics.Raycast(new Ray(transform.position, player.transform.position - transform.position),out hit))? hit.point  : player.transform.position;

    }
    public override void Attack()
    {
        if ((player.transform.position - transform.position).magnitude > 50f)
        {
            _flyingSound.volume = 0f;
        }
        else _flyingSound.volume = 0.35f;

        BirdLine.enabled = true;
        BirdLine.SetWidth(0.2f, 0.2f);
        BirdLine.SetColors(colorAggro, colorAggro);
        transform.LookAt(player.transform.position);
        BirdLine.SetPosition(0, transform.position);
        BirdLine.SetPosition(1, player.transform.position);
        return;
    }

    public override void Bounce(Vector3 dir)
    {
        return;
    }

    public override void CustomStart()
    {
        
        _as= GetComponent<AudioSource>();
        _flyingSound.clip = FlyingSound;
        _flyingSound.loop = true;
        _flyingSound.Play();



    }

    public override void Idle()
    {
        if ((player.transform.position - transform.position).magnitude > 50f)
        {
            _flyingSound.volume=0f;
        }
        else _flyingSound.volume = 0.35f;

        BirdLine.enabled = false;

        if(switchBool&& (location2 - transform.position).magnitude < 5f)
        {
            switchBool = false;
        }
        else if(!switchBool && (location1 - transform.position).magnitude < 5f) 
        {
            switchBool = true;
        }

        if(switchBool) { transform.Translate((location2 - transform.position).normalized * 15f * Time.deltaTime, Space.World);   }
        else { transform.Translate((location1 - transform.position).normalized * 15f * Time.deltaTime, Space.World); }

        foreach (Collider col in Physics.OverlapSphere(transform.position, AttackRadius))
        {
            if (col.gameObject.name == "Player") { StartCoroutine(GoAggro()); break; } 
        }
    }
    IEnumerator GoAggro() { currentState = state.ATTACK; yield return new WaitForSeconds(2f); if (currentState != state.WAIT) currentState = state.AGGRO;  }
    public override void Spin()
    {
        return;
        
    }

    public override void Stun()
    {
        return;
    }

    public override void TakeDamage(int damage)
    {
        StartCoroutine(Death());
    }
    IEnumerator Death()
    {  if(currentState== state.WAIT) { yield return null; } else {
            _explosion = Instantiate(Explosion,transform.position,Quaternion.identity,transform.parent);
            GetComponent<BoxCollider>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            
            currentState = state.WAIT;
        yield return new WaitForSeconds(0.5f);//1f
            _explosion.GetComponent<MeshRenderer>().enabled = false;
            Destroy( _explosion );
            Destroy(gameObject);
        }
    }
    public override void Wait()
    {
        if(_explosion != null) { 
        _time += Time.deltaTime;
      _explosion.transform.localScale = Vector3.one * 40 * _time ;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(location1, 2f);
        Gizmos.DrawSphere(location2, 2f);
        Gizmos.DrawSphere(new(location1.x,MaxY, location1.z), 2f);
    }

}
