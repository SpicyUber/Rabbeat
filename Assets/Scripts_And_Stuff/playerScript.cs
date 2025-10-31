using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.HID;
using static playerScript;

public class playerScript : MonoBehaviour
{ public Rigidbody playerBody;
    public GameObject DodgeBall;
   
    private CustomGameManager gameManager;
    public GameObject Ghost;
    public GameObject RabbitModel;
    public Material BlueMaterial;
    public GameObject ShockwavePrefab;
    private CapsuleCollider playerCollider = null;
    public BoxCollider attackCollision = null; 
    private Vector3 inputVelocity = Vector3.zero;
    public float playerSpeed;
    public float playerJumpIntensity;
    public float gravityIntensity;
    public float jumpGravityIntensity;
    public float MAX_GRAVITY;
    public float MAX_JUMP_INTENSITY; //default is 35
    public float MAX_SPEED; //default is 30
    private GameObject cam;
    private Vector3 camForward;
    private Vector3 camRight;
    public rhythmSystemScript rs;
    public GameObject ts;
    private float attackWindUp=0.01f;
    private float attackDuration=0.15f;
    bool isAttacking = false;
    public int attackRange = 10;
    public enemyScript lockOnTarget = null;
    public int dashIntensity;
    private float dashDuration=0.3f;
    private float coyoteTime = 0.001f;
    private Animator animator;
    private int dashNumber=0;
    //private float limitMultiplier = 1f;
   /* private bool isCrouching = false;*/
    public float startingPlayerSpeed;
    private float bounceBonus = 1;
    //private float limitMultiplierLimit = 1.8f;
    public AudioSource testSound;
    private int lastActiveBeatIndex=-1;
    private bool isBouncing = false;
    private bool isSpinning=false;
    private bool isSending = false;
    private bool spinAnimationBool = false;
    private bool readyToLowerMultiplier;
    private bool didSomethingThisBeat;
    public bool IsInCombat { get {return lockOnTarget!=null; } }
    private float currentSpinDuration;
    private bool isQuickFalling=false;
    private Vector3 moveForce=Vector3.zero;
    private Vector3 moveForceN=Vector3.zero;
    private float bounceRadius=10f;
    public SkinnedMeshRenderer ModelMaterial;
    private Vector3 defaultUp;
    private Vector3 groundNormal;
    public ParticleSystem dust;
    public ParticleSystem PunchParticle;
    private bool landReady = false;
    public bool IsStunned = false;
    private bool lastBeatLock = true;
    public float StunDuration = 1f;
    private float dustCircleDelay=0.1f;
    public AudioManager audioManager;
    bool previousFrameIsActive = false;
    private int firstpreviousActiveBI;
    private int lastCurrentStringResetIndex=-1;
    private Coroutine _springCoroutine;
    private bool spamCDbool = false;
    private int currentString = 0;
    private bool isSpringing = false;
    private float totalSpringTime = 0;
    private bool _invincible;
    private bool _isDead;
    private bool isTumbling;
    private bool isTumblingAnimation;
    [Header("TUMBLE VALUES")]
    public float TumbleForce;
    public float TumbleDuration;
    public float TumbleBonus;
    public float TumbleBeatGracePeriod;
    [Header("BUNNYHOP VALUES")]
    public float HorizontalMovementThreshold;
    public float BounceBonusMax;
    public float BounceBonusMin;
    public float BounceBonusIncrementSize;
    public float BounceMoveCoefficient;
    public float BounceJumpCoefficient;
    [Header("SPIN VALUES")]
    public float SpinNerfIncrement;
    public float SpinNerf = 0;
    public float SpinNerfAfterNSpins;
    private float spinCounter;
    public float MinSpinDuration;
    public float MaxSpinDuration;
    [Header("SWEAT")]
    public ParticleSystem SweatSystem;
    public AudioSource SweatSound;
    private bool didCough=false;
    public GameObject DustCough;
    private float isTumblingAnimationDelay=0.15f;
    [Header("PIPE")]
    public bool IsInPipe;
    public PipeScript CurrentPipe;
    [Header("HEADSET")]
    public HatCosmetic Headset;

    public bool IsDead { get { return _isDead; } }
    public float BounceBonus { get { return bounceBonus; } set { bounceBonus = value; } }

    public bool IsSpringing { get {return isSpringing; }  }

    private void OnDrawGizmos()
    {
      //  Gizmos.DrawSphere(transform.position, 3f);
    }
    // Start is called before the first frame update
    void Start()
    {
       
        if (DodgeBall == null) { throw new UnityException("Please assign the DodgeBall"); }
        DodgeBall.SetActive(false);
       gameManager = GameObject.Find("CustomGameManager").GetComponent<CustomGameManager>();
        if (gameManager == null) { throw new UnityException("I knew you found forget the CustomGameManager"); }
        IsInPipe = false;
        SpinNerf = 0;
        bounceBonus = BounceBonusMin;
        defaultUp = Vector3.up;
        currentSpinDuration = MaxSpinDuration;
        startingPlayerSpeed = playerSpeed;
        animator = GetComponentInChildren<Animator>();
        attackCollision.enabled = false;
        ts = GameObject.Find("TestSphere");
        cam= GameObject.Find("PlayerCamera").transform.GetChild(0).gameObject;
       
        playerBody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        AdjustGround();
    }

    private void LateUpdate()
    {
        if (!rs.beatMap[rs.beatIndex].isActive && !previousFrameIsActive && readyToLowerMultiplier && !isTumbling) { readyToLowerMultiplier = false; if (didSomethingThisBeat) { gameManager.IncrementMultiplier(); } else { gameManager.DecrementMultiplier(); } didSomethingThisBeat = false; }
        if (_isDead) { playerBody.velocity = Vector3.zero; }
    }

    // Update is called once per frame
    void Update()
    {
        DodgeBall.SetActive(isDashing());
        if (isSending) { if (Headset.AbilityObject == null || (Headset.AbilityObject.transform.position - transform.position).magnitude <1f ) return; Vector3 temp = (Headset.AbilityObject.transform.position - transform.position); temp.y = transform.forward.y; transform.forward = temp.normalized; return; }
        if (IsInPipe) { return; }

        if (rs.beatMap[rs.beatIndex].isActive) { readyToLowerMultiplier = true; }

        DustCoughBilboard();

        if (spinCounter >= SpinNerfAfterNSpins && currentSpinDuration-SpinNerfIncrement>MinSpinDuration)
        {
            if(!SweatSound.isPlaying)  
            SweatSound.Play();
            if (!SweatSystem.isPlaying)
            {
                SweatSystem.gameObject.SetActive(true);
                SweatSystem.Play();
            }
        }
        else
        {
            if (SweatSound.isPlaying)
                SweatSound.Stop();
            if (SweatSystem.isPlaying)
            {
                
                SweatSystem.Pause();
                SweatSystem.gameObject.SetActive(false);
            }
        }
        if (spinCounter >= SpinNerfAfterNSpins && currentSpinDuration - SpinNerfIncrement <= MinSpinDuration && !didCough) { didCough = true; DustCoughAnimate(); audioManager.source.PlayOneShot(audioManager.cough); ModelMaterial.material.color= new Color(0.5f, 0.5f, 0.5f); }else if(isGrounded()) { ModelMaterial.material.color =  new Color(1f, 1f, 1f); }
        if (_isDead) { audioManager.source.volume = 0f; return; }

        if (!rs.beatMap[rs.beatIndex].isActive && rs.beatIndex!= lastCurrentStringResetIndex ) { currentString = 0; lastCurrentStringResetIndex = rs.beatIndex; }
        previousFrameIsActive = rs.GetRollback();
        //dust particle

        if (!isGrounded()) { landReady = true; }
        if (isGrounded()&&landReady==true&& isBouncing==false) { DustCircle(); landReady = false; }

      
   
       if (rs.beatIndex == rs.beatMap.Length - 1)
        {   

            lastBeatLock = false;
        }
      if(lastBeatLock== false&& rs.beatIndex==0) { lastActiveBeatIndex = -1; lastBeatLock = true; }

        //disabling LastActiveBeatIndex
       // lastActiveBeatIndex = -1;

        
        if (isGrounded() && !isBouncing) { bounceBonus = BounceBonusMin;  }
        if (isGrounded()) { SpinNerf = 0; spinCounter = 0; didCough = false; }
        /*crouch speed
        if (isCrouching && isGrounded()) {
            playerSpeed = startingPlayerSpeed /4f;
        }else { playerSpeed= startingPlayerSpeed; }*/
        
        //animation
        animator.SetBool("isGrounded", isGrounded());
        animator.SetBool("isTumbling", isTumblingAnimation);
        if (playerBody.velocity.magnitude > 0.01f) { animator.SetBool("isRunning",true); } else animator.SetBool("isRunning", false); 
        if (playerBody.velocity.y>0.001f&&! isGrounded()) { animator.SetBool("isJumping", true); } else animator.SetBool("isJumping", false);
        if (playerBody.velocity.y < -0.001f && !isGrounded()) { animator.SetBool("isFaliing", true); Headset.BobUp(); } else { animator.SetBool("isFaliing", false); Headset.ResetBob() ; }
        animator.SetBool("isDashing", isDashing());
        animator.SetBool("isSpinning", spinAnimationBool);
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("isBouncing", isBouncing&&playerBody.velocity.y<0); // task 1: fix Bouncing anim. Task 2: add stun for missing a beat. Task 3: make jump less floaty
        animator.SetBool("isStunned", IsStunned);
        animator.SetBool("isSending", isSending);

        if (rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive) { ts.transform.localScale = Vector3.one * 5f; } else ts.transform.localScale = Vector3.one;



        //character looking towards movement
        if (!isDashing()) { 
        if (lockOnTarget!=null) { Vector3 temp = (lockOnTarget.transform.position - transform.position); temp.y = transform.forward.y; transform.forward = temp.normalized; if ((transform.position - lockOnTarget.transform.position).magnitude > attackRange) { lockOnTarget = null; } }
        else if (new Vector3(playerBody.velocity.x,0,playerBody.velocity.z).magnitude>=0.0001) {Vector3 temp= playerBody.velocity; temp.y= transform.forward.y; transform.forward = temp.normalized; }
        
        
        }

        //limits

        if (playerBody.velocity.y > MAX_JUMP_INTENSITY + bounceBonus * BounceJumpCoefficient) { Vector3 tempVec = playerBody.velocity; tempVec.y = MAX_JUMP_INTENSITY + bounceBonus * BounceJumpCoefficient; playerBody.velocity = tempVec; }
         
        if (playerBody.velocity.y < MAX_GRAVITY) { Vector3 tempVec = playerBody.velocity; tempVec.y = MAX_GRAVITY; playerBody.velocity = tempVec; }
        Vector2 movementVector = new Vector2(playerBody.velocity.x, playerBody.velocity.z);
        //added bounceBonus HERE!
        //original was if(movementVector.magnitude > MAX_SPEED + bounceBonus* BounceMoveCoefficient) { movementVector = movementVector.normalized * (MAX_SPEED + bounceBonus * BounceMoveCoefficient); Vector3 tempVec = playerBody.velocity; tempVec.x = movementVector.x; tempVec.z = movementVector.y; playerBody.velocity = tempVec; }
        float tumbleBonus = (isTumbling) ? TumbleBonus : 0f ;
        if (movementVector.magnitude > MAX_SPEED+tumbleBonus + bounceBonus* BounceMoveCoefficient) { movementVector = movementVector.normalized * (MAX_SPEED+ tumbleBonus + bounceBonus * BounceMoveCoefficient); Vector3 tempVec = playerBody.velocity; tempVec.x = movementVector.x; tempVec.z = movementVector.y; playerBody.velocity = tempVec; }
        

    }

    private void DustCoughBilboard()
    {
        Vector3 position = DustCough.transform.position + cam.transform.rotation * Vector3.forward;
        DustCough.transform.LookAt(position, cam.transform.rotation * Vector3.up);
    }

    private void DustCoughAnimate()
    {
        StartCoroutine(DustCoughAnimateCoroutine());
         
    }
    IEnumerator DustCoughAnimateCoroutine()
    {
        DustCough.transform.GetComponent<SpriteRenderer>().color = Color.white;
        Vector3 startPos = DustCough.transform.localPosition;
        Vector3 endPos = DustCough.transform.localPosition + new Vector3(-1,2,0);
        float t = 0;
        while (t < 1f) {
            DustCough.transform.localPosition = startPos + new Vector3(-1, 2, 0) * t / 1f;
            DustCough.transform.GetComponent<SpriteRenderer>().color = new(0.8f, 0.8f, 0.8f, 1- t / 1f);
        yield return null;
            t += Time.deltaTime;
        }
        DustCough.transform.localPosition = endPos;
        DustCough.transform.GetComponent<SpriteRenderer>().color = new(0, 0, 0, 0);
        DustCough.transform.localPosition = startPos;
    }
    public void SetIsBouncing(bool isBouncingNew)
    {
        isBouncing = isBouncingNew;
    }
    public bool IsBouncing()
    {
        return isBouncing;
    }

   public bool IsAttacking() { return isAttacking; }
    private void UpdateDustPosition()
    {
       /* RaycastHit dustInfo = new RaycastHit();
        Physics.Raycast(new Ray(new Vector3(dust.transform.position.x, dust.transform.position.y - 2, dust.transform.position.x),-transform.up),out dustInfo);
        Debug.Log(dustInfo.transform.position.y + "HIIII");
        if (dustInfo.collider!=null) 
        dust.transform.position.Set(dust.transform.position.x, dustInfo.transform.position.y, dust.transform.position.z);*/
    }
    void AdjustGround()
    {
        bool didIAdjust = false;
        Vector3 normal = Vector3.up;
        float tempy = transform.forward.y;
        RaycastHit[] rays = Physics.RaycastAll(playerBody.position,   -1*transform.up, 4f); //maybe turn back into spherecast later
        
        foreach(RaycastHit ray in rays)
        {
            if (ray.collider.gameObject.CompareTag("Ground"))
            {
                normal = ray.normal;
                this.transform.rotation = this.transform.rotation * Quaternion.FromToRotation(this.transform.up, ray.normal);
                didIAdjust = true;  break;
            }

        }

        if (!didIAdjust||normal==new Vector3(0,1,0)) { groundNormal = Vector3.up; Vector3 temp = transform.forward; temp.y = 0; transform.forward = temp;  } else { groundNormal = normal; Vector3 temp = transform.forward; temp.y = Vector3.Cross(normal,new Vector3(0,1,0)).y; transform.forward = temp; }



       
    }

    public void Spring(float duration)
    {
        //  if (_springCoroutine != null) { StopCoroutine(_springCoroutine); isSpringing = false; }
        if (isSpringing) { return; }
        _springCoroutine = StartCoroutine(SpringCoroutine(duration));
    }

    IEnumerator SpringCoroutine(float duration)
    {
        totalSpringTime = 0; 
            isSpringing = true;
        while (totalSpringTime<duration) {
                
                yield return null;
                totalSpringTime=totalSpringTime+Time.deltaTime;
            
            }
        isSpringing=false;
        
    }

    void spamCD()
    {
        
            StartCoroutine(spamCDCoroutine());
    }
    IEnumerator spamCDCoroutine()
    {
        if (spamCDbool) { yield return null; }
        else { 
        spamCDbool = true;

        yield return new WaitForSeconds(rs.spamTime);
        spamCDbool = false;
        }

    }
    void FixedUpdate()

    {   if (isSending) { playerBody.velocity = Vector3.zero; return; }
        if (IsInPipe  ) { return; }
       
        if (inputVelocity.magnitude > 0.01f || transform.up==Vector3.up ) { 
        AdjustGround();
        }
        //forward relative to camera
        camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();
        /* crouching interlude for friction and slowdown
        if (isCrouching && isGrounded()) {
            Vector3 temp = playerBody.velocity;
            temp.y = 0;
            playerBody.AddForce(-temp*4, ForceMode.Acceleration);
        }*/
        Vector3 relativeForward = camForward * inputVelocity.z;
        Vector3 relativeRight = camRight * inputVelocity.x;

        moveForce = relativeForward + relativeRight;

       moveForce.Normalize();

        moveForceN = moveForce;
        if(isSpringing) { moveForceN= Vector3.zero; }
        //movement
        
        if (!isDashing()&&inputVelocity != Vector3.zero)
        {

            playerBody.AddForce(moveForceN * playerSpeed, ForceMode.VelocityChange);
        }

        //two gravities
       
        if (isSpringing) isQuickFalling = true;
        if (isSpinning == false) {if (isTumbling && isGrounded()) {/* Vector3 temp = playerBody.velocity; temp.y = 0; playerBody.velocity = temp;*/ }
            else if (isSpringing) { }
            else
            {
                if (playerBody.velocity.y >= playerJumpIntensity) { if (isQuickFalling) { playerBody.AddForce(-groundNormal * 2 * jumpGravityIntensity, ForceMode.VelocityChange); } else { playerBody.AddForce(-groundNormal * jumpGravityIntensity, ForceMode.VelocityChange); } }
                else { if (isQuickFalling) { playerBody.AddForce(-groundNormal * 2 * gravityIntensity, ForceMode.VelocityChange); } else { playerBody.AddForce(-groundNormal * gravityIntensity, ForceMode.VelocityChange); } }
            } }else { Vector3 temp = playerBody.velocity; temp.y = 0; playerBody.velocity = temp; }



        if(isGrounded())isQuickFalling = false;


    }
    private bool isDashing() { return dashNumber > 0; }
    private void OnMove(InputValue value) 
    {  
        inputVelocity = value.Get<Vector3>();
        
    }

    private void OnCrouch(InputValue value)
    {
        if (IsStunned || IsInPipe) { return; }
        if (spamCDbool||/*currentString>=rs.longestString||*/(lastActiveBeatIndex==rs.beatIndex/*&&rs.longestString<=1*/))
        {
            StunPlayer();
            return;
        }
        if (!isGrounded() && !isBouncing && (rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive)) {
             
            StartCoroutine(Bounce()); spamCD(); currentString++; lastActiveBeatIndex = rs.beatIndex;
            
        }
        else if(!(rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive)) { StunPlayer();  }
       
    }
    private void OnQuickFall(InputValue value) {
        isQuickFalling = true;
        
    }

    private void DustCircle()
    {

        StartCoroutine(DustCircleCoroutine());   
    }
    IEnumerator DustCircleCoroutine()
    {
        yield return new WaitForSeconds(dustCircleDelay);
        dust.Play();

    }
    IEnumerator Bounce()
    {
        didSomethingThisBeat = true;
        audioManager.source.PlayOneShot(audioManager.bounce);

        isBouncing = true;
        bool didntClip = true;
        Vector3 playerprevpos = playerBody.position;
        while (!isGroundedSpecial() && didntClip == true && isBouncing)
        {

            RaycastHit[] rh = Physics.CapsuleCastAll(playerprevpos, playerBody.position, playerCollider.radius, Vector3.down);

            Vector3? rcHit = BounceRollback(rh); if (rcHit != null) { didntClip = false; playerBody.position = (Vector3)rcHit + Vector3.up * 2f; break; }
            playerprevpos = playerBody.position;
            if (didntClip == true) {
                playerBody.AddForce(Vector3.down * (-1 / 2) * MAX_GRAVITY, ForceMode.VelocityChange);
            }

            yield return null;
        }


        
        RaycastHit hit = getGround();
        if (!hit.collider.CompareTag("RedButton")) {
           
        if (hit.collider.gameObject.name != "Player") {
            audioManager.source.PlayOneShot(audioManager.bouncehit);
            DustCircle();
            Shockwave();
            playerBody.velocity = new(playerBody.velocity.x, 0, playerBody.velocity.z);
            if (hit.collider != null) { 
            HoleCast(hit.collider.transform.up);
            BounceCast(hit.collider.transform.up);
            }
            // BounceExecution(true);
            //tumble
            float tumbleTime = 0f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                
                
                
                
                while (tumbleTime < TumbleDuration)
                {
                    if(tumbleTime>isTumblingAnimationDelay) { if (isTumbling == false) { audioManager.source.PlayOneShot(audioManager.tumble); playerBody.AddForce(moveForce.normalized * TumbleForce+ Vector3.up*2f, ForceMode.VelocityChange); ; playerSpeed = playerSpeed * 2; } isTumbling = true; isTumblingAnimation = true;  }
                    if (!Input.GetKey(KeyCode.LeftShift)) { if (!(rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive) && (tumbleTime>TumbleBeatGracePeriod)) { tumbleTime = TumbleDuration + 1; StunPlayer(); } break; }
                    
                    yield return null;
                    tumbleTime+= Time.deltaTime;
                }
                if(!IsStunned)playerSpeed = startingPlayerSpeed;
               isTumbling= false;
                isTumblingAnimation = false;
            }
            //end of tumble
            if (tumbleTime <= TumbleDuration)
            {
            DustCircle();

            BounceExecution((tumbleTime < isTumblingAnimationDelay));
            
            }
        }
        yield return null;
        groundNormal = Vector3.up;
        isQuickFalling = true;
        StartCoroutine(delayedQF());
        float horizontalMovement = new Vector3(moveForceN.x, 0, moveForceN.y).magnitude;
        if (bounceBonus < BounceBonusMax && horizontalMovement>HorizontalMovementThreshold) { 
        bounceBonus += BounceBonusIncrementSize;
        }
        currentSpinDuration = MaxSpinDuration;
        spinCounter= 0;
        SpinNerf = 0;
        //this might cause problems
        yield return new WaitForSeconds(0.04f);
        }else { yield return new WaitForSeconds(0.25f); }
        isBouncing = false;

    }

    private void Shockwave( )
    {
        Vector3 up = Vector3.up;
        Vector3 pos = transform.position;
        foreach (RaycastHit rc in Physics.RaycastAll(transform.position, -transform.up)){
            if (rc.collider.gameObject.CompareTag("Ground")) { pos = rc.point; if (rc.collider.gameObject.GetComponent<SquareMountain>() != null) up = rc.collider.gameObject.GetComponent<SquareMountain>().Normal; else up = rc.collider.gameObject.transform.up; pos = pos + up * 0.05f; break; }
        }
        
        
       GameObject go = GameObject.Instantiate(ShockwavePrefab,pos,Quaternion.identity);
        go.transform.up = up;
    }

    public bool IsTumbling()
    {
        return isTumblingAnimation;
    }

    private Vector3 BounceExecution(bool muted)
    {
        didSomethingThisBeat = true;
        Vector3 returnValue = Vector3.up;
        GameObject hitBP = GetBouncePad();
        SquareMountain squareMountain = GetSquareMountain();
        if (hitBP != null) { if (!muted) audioManager.source.PlayOneShot(audioManager.bouncehit); playerBody.AddForce((hitBP.transform.up.normalized * hitBP.GetComponent<BouncePad>().BounceForce * (playerJumpIntensity) - Vector3.down * (-1) * MAX_GRAVITY) + moveForceN * 100f - Vector3.up * 2f, ForceMode.VelocityChange); returnValue = hitBP.transform.up.normalized; }


        else if (squareMountain != null)
        {
           if(!muted) audioManager.source.PlayOneShot(audioManager.bouncehit);
            playerBody.AddForce((squareMountain.Normal * (playerJumpIntensity + bounceBonus) - Vector3.down * (-1) * MAX_GRAVITY) + moveForceN * 100f - Vector3.up * 2f, ForceMode.VelocityChange);

           returnValue= squareMountain.Normal;
        }
        else
        {
            //original was moveForceN*100
            if (!muted) audioManager.source.PlayOneShot(audioManager.bouncehit);
            playerBody.AddForce(((isGrounded() ? getGround().transform.up : Vector3.up) * (playerJumpIntensity + bounceBonus) - Vector3.down * (-1) * MAX_GRAVITY) + moveForceN * 100f - Vector3.up * 2f, ForceMode.VelocityChange);
          
            returnValue= (isGrounded() ? getGround().transform.up : Vector3.up);
        }
        return returnValue;
    }

    private SquareMountain GetSquareMountain()
    {
        RaycastHit[] rays = Physics.CapsuleCastAll(playerCollider.bounds.center, playerCollider.bounds.size, 0f, -transform.up, 4f);
        foreach (RaycastHit ray in rays)
        {
            if (ray.collider.CompareTag("Ground") && ray.collider.gameObject.GetComponent<SquareMountain>()!=null)
            {
                return ray.collider.gameObject.GetComponent<SquareMountain>();
            }
        }

        return null;
    }

    private GameObject GetBouncePad()
    {
        RaycastHit[] rays = Physics.CapsuleCastAll(playerCollider.bounds.center, playerCollider.bounds.size, 0f, -transform.up, 4f);
        foreach(RaycastHit ray in rays)
        {
            if (ray.collider.CompareTag("BouncePad"))
            {
                return ray.collider.gameObject;
            }
        }

        return null;
    }

    private Vector3? BounceRollback(RaycastHit[] rh)
{
        if(rh.Length == 0)return null;
        bool collisionFound=false;
        
        RaycastHit hit =rh[0];
   for (int i  = 0; i < rh.Length; i++)
        {
            if (collisionFound==false && rh[i].collider.gameObject.name!="Player" && rh[i].collider.gameObject.CompareTag("Collectable")==false && rh[i].collider.gameObject.CompareTag("DodgeBall") == false && rh[i].collider.gameObject.CompareTag("NoBounce")==false && rh[i].collider.gameObject.CompareTag("CameraTrigger") == false)
            {
                hit = rh[i];
                collisionFound= true; 
            }
            else if(collisionFound && rh[i].collider.name!="Player" && (rh[i].point-transform.position).magnitude< (hit.point - transform.position).magnitude && rh[i].collider.gameObject.CompareTag("Collectable") == false && rh[i].collider.gameObject.CompareTag("DodgeBall") == false &&  rh[i].collider.gameObject.CompareTag("CameraTrigger") == false)
            {
                hit = rh[i];
            }

        }
   if(collisionFound==false) { return null; }else
        return hit.point;
}

IEnumerator delayedQF()
    {
        yield return new WaitForSeconds(0.3f);
        isQuickFalling = true;

    }
    private void BounceCast(Vector3 dir)
    {
       
        RaycastHit[] rc = Physics.SphereCastAll(this.transform.position, bounceRadius, -transform.up, 8f);

        foreach(RaycastHit r in rc) {
            
            if (r.collider.gameObject.tag.Equals("Enemy"))
            {
                enemyScript enemy = r.collider.GetComponentInParent<enemyScript>();
                
                enemy.Bounce(dir);
                
            }
            if (r.collider.gameObject.CompareTag("Carrot"))
            {
                CarrotScript carrot = r.collider.GetComponentInParent<CarrotScript>();
                carrot.Bounce(this);

            }
            if (r.collider.gameObject.CompareTag("Crate"))
            {
                r.collider.GetComponent<CrateScript>().SelfDestroy();

            }
        
        }
    }
    //smaller than bounceCast
    private void HoleCast(Vector3 dir)
    {
        
        RaycastHit[] rc = Physics.SphereCastAll(this.transform.position, 3f, -transform.up, 5f);

        foreach (RaycastHit r in rc)
        {
             
            if (r.collider.gameObject.CompareTag("Hole"))
            {
                Hole hole = r.collider.GetComponent<Hole>();

                transform.position = hole.LeadsTo.position;
                audioManager.source.PlayOneShot(hole.Sound,0.5f);
            }
           

        }
    }

    public void Hurt()
    {  if(_isDead) return;
        if(isDashing() || _invincible) { return; }
        audioManager.source.PlayOneShot(audioManager.hurt, 2f);
        
      //  Debug.Log("Ouchies!");
        gameManager.currentPlayerHealth--;
        if (gameManager.GetComponent<CustomGameManager>().currentPlayerHealth <= 0)
        {
            _isDead = true;
            Instantiate(Ghost, transform.position,Quaternion.LookRotation(cam.transform.position-transform.position,transform.up) ,transform.parent);
            gameManager.GetComponent<CustomGameManager>().Death();
        }
        StartCoroutine(Invincible());

    }

    public void AddEnergy(int number,Vector3 worldPos)
    {
        gameManager.AddEnergyOrb(number, worldPos);
    }

    private void OnJump(InputValue value) {
        Headset.SetAbility(HatCosmetic.Ability.Jump);
        if (IsInPipe && !IsStunned) { if (CurrentPipe == null) return; else if (rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive) CurrentPipe.Pump(); else { StunPlayer(); CurrentPipe.SlowDown(); } return; }
        if(!isGrounded()&& (lastActiveBeatIndex == rs.beatIndex/* && rs.longestString <= 1*/)) { StunPlayer(); return; }
       // Debug.Log("beat = " + rs.beatIndex + "lastBeat = " + lastActiveBeatIndex);
        if (IsStunned || IsInPipe) { return; }
        if (isDashing()||isBouncing||isSpinning) return;
        if (isGrounded())
        {
            isQuickFalling = false;
        }
        StartCoroutine(Jump(rs.beatIndex));
        if (spamCDbool /*|| currentString >= rs.longestString*/)
        {
            if (!isGrounded())
            {
                StunPlayer();
                return;
            }
        }else
        if ((!(rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive) && !isGrounded()))
        {
            StunPlayer();
        }else
        if ((rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive) && !isGrounded()) {
            spamCD(); currentString++;
            lastActiveBeatIndex = rs.beatIndex;
        StartCoroutine(Spin());
            StartCoroutine(SpinAnimation());
            
        }
    }

    private void StunPlayer() {  if (IsStunned == false) { IsStunned = true; audioManager.source.PlayOneShot(audioManager.test,0.5f); StartCoroutine(Stun()); }else return; }

    IEnumerator Stun()
    {
        gameManager.Stun(StunDuration);
        playerSpeed = playerSpeed / 8;
        yield return new WaitForSeconds(StunDuration);
        IsStunned = false;
        playerSpeed = startingPlayerSpeed;
    }
    IEnumerator SpinAnimation()
    { float temptime = 0;
        spinAnimationBool = true;
        while (temptime < 0.09f)
        {
            if (isGrounded()) { spinAnimationBool = false;break; }
            temptime += Time.deltaTime;
            yield return null;
        }
        spinAnimationBool = false;
    }
    IEnumerator Spin()
    {
        didSomethingThisBeat = true;
        currentSpinDuration = MaxSpinDuration - SpinNerf;
        //come back to this
        if (currentSpinDuration > MaxSpinDuration) { currentSpinDuration = MaxSpinDuration; }
        if (spinCounter>=SpinNerfAfterNSpins) { 
           
            SpinNerf += SpinNerfIncrement;
        }
        spinCounter++;
        
        if (currentSpinDuration < MinSpinDuration) { currentSpinDuration = MinSpinDuration; }
        
        if(currentSpinDuration > MinSpinDuration) { 
        isSpinning = true;
        }
        audioManager.source.PlayOneShot(audioManager.spin);
        SpinEnemies();
        animator.SetTrigger("Spin");
        yield return new WaitForSeconds(currentSpinDuration);
        isSpinning=false;
        //spin fatigue if (currentSpinDuration > 0.01f) { currentSpinDuration = (float)Math.Log(currentSpinDuration); if (currentSpinDuration < 0.01f) currentSpinDuration = 0.01f; }
        

    }

    private void SpinEnemies() {
         RaycastHit[] spinHit= Physics.SphereCastAll(this.transform.position, 10f, Vector3.down, 0f); 
        foreach(RaycastHit h in spinHit) {
             if (h.collider.gameObject.tag.Equals("Enemy") ) {
                 if (h.collider.gameObject.GetComponentInParent<enemyScript>() != null) {

                  enemyScript es=  h.collider.gameObject.GetComponentInParent<enemyScript>();
                    es.Spin();
                    
                        
                        
                        } } }
    
    }
    public bool isGrounded() {
        
        bool flag = false;
        string rayname = "";
        RaycastHit[] rays = Physics.SphereCastAll(playerBody.position, 0.75f, -transform.up, 2f);
        for (int i = 0; i < rays.Length; i++) {
            if (rays[i].collider.gameObject.name != "Player" && !rays[i].collider.gameObject.CompareTag("Collectable") && rays[i].collider.gameObject.CompareTag("DodgeBall") == false && rays[i].collider.gameObject.CompareTag("CameraTrigger") == false && !rays[i].collider.gameObject.name.Contains("Attack") && rays[i].point.y < transform.position.y) { rayname =rays[i].collider.name; flag = true; break; } if (playerBody.velocity.y >= -0.0001f && playerBody.velocity.y <= 0.0001f) { }  }

        if (groundNormal == Vector3.up)
        {   if(flag && playerBody.velocity.y <= 1f) {  }
            return flag && playerBody.velocity.y <= 1f;
        }
        else return flag;


    }

    public void OnHeadphone(InputValue value)
    {
        if (IsInPipe || IsStunned || IsStunned || isSending || isBouncing || isTumbling || _isDead) return;
      if(Headset.ActivateAttack())
        {
            
            isSending = true;
            animator.SetBool("isSending", isSending);
            Headset.TurnBlue();
            StartCoroutine(HeadphoneCD(2f));
            TurnBlue();
        }



    }
    public void TurnBlue()
    {
     Material material = RabbitModel.GetComponentInChildren<SkinnedMeshRenderer>().material;
        RabbitModel.GetComponentInChildren<SkinnedMeshRenderer>().material = BlueMaterial ;
        StartCoroutine(BlueRoutine(material));
        
    }
   public IEnumerator BlueRoutine(Material material)
    {
        yield return new WaitForSeconds(0.15f);
        RabbitModel.GetComponentInChildren<SkinnedMeshRenderer>().material = material;
    }
    IEnumerator HeadphoneCD(float seconds) {

        yield return new WaitForSeconds(seconds);
        isSending = false;
        IsInPipe = false;
    }
    private bool isGroundedSpecial() { RaycastHit hit; return Physics.SphereCast(playerBody.position, 0.75f, -transform.up, out hit, 4f)&&hit.point.y<playerBody.position.y && !hit.collider.CompareTag("Collectable")  && hit.collider.gameObject.CompareTag("DodgeBall") == false && hit.collider.gameObject.CompareTag("CameraTrigger") == false; }
  
    private RaycastHit getGround() { RaycastHit[] rays = Physics.CapsuleCastAll(playerCollider.bounds.center, playerCollider.bounds.size, 0f, -transform.up, 4f);
        for (int i = 0; i < rays.Length; i++)
        {
            if (rays[i].collider.gameObject.name != "Player" && rays[i].point.y < transform.position.y) return rays[i];
                


                    }
        
        return rays[0];

    }
    private void OnAttack(InputValue value)
    {
        Headset.SetAbility(HatCosmetic.Ability.Attack);

        if (IsStunned || isTumbling || isSending) return;
        if (spamCDbool || /*currentString >= rs.longestString||*/ (lastActiveBeatIndex == rs.beatIndex /*&& rs.longestString <= 1*/))
        {
            StunPlayer();
            return;
        }
        if ((rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive)) { StartCoroutine(Attack()); spamCD(); currentString++; lastActiveBeatIndex = rs.beatIndex;  } else if (!(rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive)) { StunPlayer(); }


    }
    private void OnDash(InputValue value) {
        Headset.SetAbility(HatCosmetic.Ability.Heal);
        if (IsStunned || isTumbling  || IsInPipe || isSending) return;
        if (spamCDbool /*|| currentString >= rs.longestString */|| (lastActiveBeatIndex == rs.beatIndex /*&& rs.longestString <= 1*/)) {
            StunPlayer();
            return;
        }
        if ((rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive) && !isAttacking) {   StartCoroutine(Dash()); spamCD(); currentString++; lastActiveBeatIndex = rs.beatIndex; animator.SetTrigger("DashTrigger");  } else if (!(rs.beatMap[rs.beatIndex].isActive || previousFrameIsActive)) { StunPlayer(); }

    }

    IEnumerator Jump(int beatIndex)
    {
        
        float time = 0;
        while (time<=coyoteTime) {
            
            if (isGrounded()&&!isBouncing)
            {
                /* if (isCrouching && rs.beatMap[beatIndex].isActive) { if(limitMultiplier < limitMultiplierLimit) limitMultiplier += 0.4f;
            }*/
                DustCircle();
                audioManager.source.PlayOneShot(audioManager.jump);
                 playerBody.AddForce(transform.up * playerJumpIntensity, ForceMode.VelocityChange);
               // dust.Play();
                currentSpinDuration = MaxSpinDuration;
                break;
            }
            time += Time.deltaTime;
            yield return null;
        }

    }
   IEnumerator Dash(){
        didSomethingThisBeat = true;
       
        DodgeBall.transform.position=transform.position;
        float tempY = playerBody.velocity.y;
        dashNumber++;
        playerBody.velocity = new Vector3(0,tempY,0);
        playerBody.AddForce(-transform.forward*dashIntensity, ForceMode.VelocityChange);
        yield return new WaitForSeconds(dashDuration);
        tempY = playerBody.velocity.y;
        playerBody.velocity = new Vector3(0, tempY, 0);
        dashNumber--;

    }
        IEnumerator Attack()
    {
        didSomethingThisBeat = true;
        animator.SetTrigger("Attack");
        audioManager.source.PlayOneShot(audioManager.punch,1f);
        isAttacking = true;
        
        //yield return new WaitForSeconds(attackWindUp);
        bool didLunge=lunge();
        attackCollision.enabled = true;
        

        yield return new WaitForSeconds(attackDuration);


        attackCollision.enabled = false;

        if (didLunge) { playerBody.velocity = Vector3.zero; }
        
        isAttacking = false;
        
    }
    bool lunge() {
     enemyScript[] enemies = FindObjectsOfType<enemyScript>();
        if (enemies.Length == 0) { return false; }
        enemyScript closest = enemies[0];
        foreach (enemyScript enemy in enemies) {
            if (((enemy.transform.position - transform.position).magnitude < (closest.transform.position - transform.position).magnitude)&&enemy.IsDying==false) {
                closest = enemy;
            }
        }
        if ((closest.transform.position - transform.position).magnitude <= attackRange) {
            this.playerBody.AddForce((closest.transform.position - transform.position)*30,ForceMode.VelocityChange);
            lockOn(closest);

            return true;
        } else return false;
    }
    void lockOn(enemyScript l) { lockOnTarget = l; }

    IEnumerator Invincible()
    {
        if (gameManager.currentPlayerHealth <= 0)
        {
            RabbitModel.transform.localScale = Vector3.zero;
        }
        else { 
        Vector3 startScale = RabbitModel.transform.localScale;
        _invincible =true;
        RabbitModel.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.2f);
        RabbitModel.transform.localScale = startScale;
        yield return new WaitForSeconds(0.2f);
        RabbitModel.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.2f);
        RabbitModel.transform.localScale = startScale;
        yield return new WaitForSeconds(0.2f);
        RabbitModel.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.2f);
        RabbitModel.transform.localScale = startScale;
        _invincible = false;
        }
    }

    public void Knockback(Vector3 dir, float force)
    {
        playerBody.AddForce(dir * force);
    }

    public void EmitPunchParticle()
    {
        PunchParticle.Play();
    }
}
