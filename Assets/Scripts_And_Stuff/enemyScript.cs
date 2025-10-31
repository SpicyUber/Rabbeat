using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

abstract public class enemyScript : MonoBehaviour
{  public enum state { IDLE=1,AGGRO=2,STUN=3,ATTACK=4,WAIT=5}
    public state currentState = state.IDLE;
    public int MAX_HEALTH;
    public float MarkOffset;
    public int currentHealth;
    public Rigidbody rb;
    public int gravity;
    public int speed;
    internal float squareAggroRange;
    internal int halfGravityForce;
    public int knockbackForce;
    public int bounceMultiplier;
    public float aggroRange;
    public float attackRange;
    public float stunTime;
    public int damageTaken;
    internal int revengeMeter = 0;
    internal bool isHalfGravity = false;
    public float halfGravityDuration;
    internal bool isBouncing = false;
    public float radius;
    private bool invincible;
    internal bool isSpinning = false;
    internal float spinDuration = 0.1f;
    internal playerScript player;
    internal Vector3 moveDir;
    public bool IsDying = false;
    internal float squareAttackRange;
    public rhythmSystemScript rs;
    
    public GameObject meObject;
    internal CustomGameManager gameManager;
    // Start is called before the first frame update
    void Start()

    {
        gameManager = GameObject.FindAnyObjectByType<CustomGameManager>();
        meObject = this.gameObject;
        player = GameObject.Find("Player").GetComponent<playerScript>();
        if (player == null) throw new UnityException("player not found.");
        halfGravityForce = gravity / 2;
        rb = GetComponent<Rigidbody>();
        if (MAX_HEALTH <= 0) throw new UnityException("MAX_HEALTH is invalid");
        currentHealth = MAX_HEALTH;
        CustomStart();
        
    }

    protected string GetMyObjectName(){ return meObject.name; }

    // Update is called once per frame
    void Update()
    {
        if (revengeMeter > 100) { revengeMeter = 0; }
        switch (currentState)
        {
            case state.IDLE:
                Idle();
                break;
            case state.AGGRO:
                Aggro();
                break;
            case state.STUN:
                Stun();
                break;
            case state.ATTACK:
                Attack();
                break;
            case state.WAIT:
                Wait();
                break;
        }
    }
    public abstract void CustomStart();
    public abstract void Wait();

    public abstract void Attack();



    public abstract void Stun();


    public abstract  void Aggro();

    public void halfGravity() {
    if(isHalfGravity) return;
    StartCoroutine(halfGravityCoroutine());
    }

    IEnumerator halfGravityCoroutine() {
        isHalfGravity = true;
        yield return new WaitForSeconds(halfGravityDuration);
        isHalfGravity = false;
    
    }

    public bool isGrounded()
    {
        if (rb == null) return false;
        bool flag = false;

        RaycastHit[] rays = Physics.SphereCastAll(transform.position, radius, Vector3.down, 4f);
        for (int i = 0; i < rays.Length; i++)
        {
            if (rays[i].collider.gameObject.tag.Equals("Ground") && rays[i].point.y < transform.position.y){ flag = true; break; }
            
        }
        return flag && (rb.velocity.y >= -0.0001f && rb.velocity.y <= 0.0001f);
    }
    public bool isGroundedBounce()
    {
        if (rb == null) return false;
            bool flag = false;

        RaycastHit[] rays = Physics.SphereCastAll(rb.position, radius, Vector3.down, 8f);
        for (int i = 0; i < rays.Length; i++)
        {
            if (rays[i].collider.gameObject.tag.Equals("Ground") && rays[i].point.y < transform.position.y) { flag = true; break; }

        }
        return flag;
    }
    public abstract  void Idle();

    public abstract void Spin();
    public abstract void Bounce(Vector3 dir);

    public abstract void TakeDamage(int damage);

   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Attack" && !invincible) {
            StartCoroutine(Invincibility());
        
        TakeDamage(damageTaken);
            player.EmitPunchParticle();
            knockback(collision.gameObject.transform.forward, knockbackForce);

            collision.gameObject.GetComponentInParent<playerScript>().attackCollision.enabled = false;


        }
        
    }

    IEnumerator Invincibility()
    {
        invincible = true;

        yield return null;

        invincible = false;
    }
    void knockback(Vector3 origin ,int force) {
       if(rb!=null) rb.AddForce(origin*force, ForceMode.VelocityChange);

    }

     
}
