using System;
using System.Collections;
using UnityEngine;
 

public class basicEnemy : enemyScript
{
    public float collisionRange;
    public AudioClip windUp;
    public AudioClip swing;
    public AudioClip ouch;
    public AudioClip alert;
    public AudioClip stun;
    public AudioClip bounce;
    public AudioSource moving;
    public int sensitivity;
    private bool attackIsReady = false;
    public int loopsEveryN;
    public int windUpBeat;
    public int attackBeat;
    private int[] windUps;
    private int[] attackBeats;
    public Animator animator;
    public Animator bodyAnimator;
    public Transform arm;
    private bool armIsShrinking;
    private bool armIsGrowing;
    public Transform wheel;
    private float minWheelRotationX;
        private float maxWheelRotationX;
    private AudioSource audioSource;
    private Vector3 defaultArmPos;
    private Coroutine stunRoutine;
    private Coroutine damageRoutine;
    private Color defaultColor;
    public bool IsTalking = false;
    public ParticleSystem SpeechParticles;
   
    
    
    private class EnemyOrientations
    {
        public EnemyOrientation[] orientations;


        public EnemyOrientations(Transform transform, int n)
        {
            if (n <= 0) { n = 40; }
            orientations = new EnemyOrientation[n];
            float degrees = 0;
            float increment = 360 / (float)n ;
            for(int i = 0; i < n; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad*degrees);
                float y = Mathf.Sin(Mathf.Deg2Rad * degrees);
                orientations[i] = new EnemyOrientation(new(x,0,y));
                degrees += increment;

            }

#if false
            orientations[0] = new EnemyOrientation(transform.forward);
            orientations[1] = new EnemyOrientation((transform.forward + transform.right).normalized);
            orientations[2] = new EnemyOrientation(transform.right);
            orientations[3] = new EnemyOrientation((transform.right + (-transform.forward)).normalized);
            orientations[4] = new EnemyOrientation(-transform.forward);
            orientations[5] = new EnemyOrientation((-transform.forward - transform.right).normalized);
            orientations[6] = new EnemyOrientation(-transform.right);
            orientations[7] = new EnemyOrientation((-transform.right + transform.forward).normalized); 
#endif
        }
    }
    public override void Stun()
    {
        return;
    }
    private class EnemyOrientation
    {
        public Vector3 direction;
        public bool isColliding = false;
        public float playerAngle;
        public EnemyOrientation(Vector3 newDir)
        {
            direction = newDir;
        }

        public void updateColliding(Transform origin, float collisionRange, GameObject enemyGameObject)
        {
            RaycastHit[] rc = Physics.RaycastAll(origin.position, direction, collisionRange);
            foreach (RaycastHit hit in rc)
            {
                if (hit.collider.gameObject.name != "Player" && !hit.collider.gameObject.CompareTag("Enemy")) { isColliding = true; return; }
            }
            isColliding = false; return;
        }
    }

    private Vector3 enemyToPlayer;
    private EnemyOrientations enemyOrientations;
    private bool wheelTurn;
    private Vector3 lastClosest;
    private int lastClosestIndex;
    private GameObject _explosion;
    private float _time = 0;
    public GameObject Explosion;
    private bool collisionTime=true;

    public override void CustomStart()
    {
        
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (collisionRange == 0) { collisionRange = 25; }
        enemyToPlayer = player.transform.position - transform.position;
        enemyOrientations = new EnemyOrientations(this.transform,sensitivity);
       

        if(attackRange == 0) {  attackRange = 10; }
        if(aggroRange == 0) { aggroRange = 50; }
        squareAttackRange = attackRange * attackRange;
        squareAggroRange = aggroRange * aggroRange;
        minWheelRotationX = wheel.localRotation.eulerAngles.x;
        maxWheelRotationX = wheel.localRotation.eulerAngles.x +360;
        defaultArmPos = arm.transform.localPosition;
        fillArrays();
       defaultColor = transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material.GetColor("_Color");
        StartCoroutine(CheckYap(0.25f));
    }

    IEnumerator CheckYap(float seconds)
    {
        if(IsTalking) { SpeechParticles.Play(); }
        yield return new WaitForSeconds(seconds);
        StartCoroutine(CheckYap(0.25f));
    }

    void fillArrays()
    {
        if (rs.beatMap == null || rs.beatMap.Length == 0) { StartCoroutine(TimeoutThenAction(fillArrays,1)); return; }
        windUps = new int[rs.beatMap.Length / loopsEveryN];
        attackBeats = new int[rs.beatMap.Length / loopsEveryN];
        if (loopsEveryN == 0) throw new UnityException("fillArays error when creating Enemy.");
       
        for (int i = 0; i < rs.beatMap.Length / loopsEveryN; i++)
        {
            windUps[i] = windUpBeat + loopsEveryN * i;
            attackBeats[i] = attackBeat + loopsEveryN * i;
        }

    }
    IEnumerator TimeoutThenAction(Action function, float seconds )
    {
        yield return new WaitForSeconds(seconds);

        function();
    }

    private Vector3 GetGroundNormal()
    {


        RaycastHit[] rays = Physics.SphereCastAll(transform.position, radius, Vector3.down, 4f);
        for (int i = 0; i < rays.Length; i++)
        {
            if (rays[i].collider.gameObject.tag.Equals("Ground") && rays[i].point.y < transform.position.y) { return rays[i].normal; }

        }
        return Vector3.up;

    }

    private bool isOnEdge() {
      //  Debug.Log("I AM TOUCHING?" + Physics.Raycast(transform.position + transform.forward * 2f, -transform.up, 4f));
        return (!Physics.Raycast(transform.position + moveDir * 2f, -transform.up, 4f) ) && isGrounded();
    
    }
    public override void Aggro()
    {
        if(!isOnEdge()) 
        Adjust();
        
        if (collisionRange == 0) { collisionRange = 25; }

        enemyToPlayer = player.transform.position - transform.position;
        if (enemyToPlayer.sqrMagnitude < squareAttackRange && canSeePlayer()) { this.currentState = state.ATTACK; return; }
        if (enemyToPlayer.sqrMagnitude > squareAggroRange) { this.currentState = state.IDLE; return; }
        //  Vector3 enemyToPlayerFlat = player.transform.position - transform.position;
        //  enemyToPlayerFlat.y = 0;
        // enemyToPlayerFlat.Normalize();
        if(collisionTime){ collisionTime = false; StartCoroutine(collisionTimeout());
        //check for surrounding collisions
        foreach (EnemyOrientation orientation in enemyOrientations.orientations) {
            orientation.updateColliding(transform, collisionRange, this.gameObject);

        }
        // find non colliding direction closest to player
        Vector3? closest = null;
        int closestIndex = 0;
        int lastClosestIndexTemp = -1;
        foreach (EnemyOrientation orientation in enemyOrientations.orientations)
        { if (orientation.isColliding) { closestIndex++; continue; }

            if (isItCloserThanTheClosest(orientation.direction, closest)) {
                closest = orientation.direction;
                lastClosestIndexTemp = closestIndex;
            }
            closestIndex++;
        }
       // Debug.Log("ENEMY: closest:" + closest);
        if (closest != null) {
            if (isWeirdEdgeCase((Vector3)closest)) { closest = lastClosest; }




            lastClosest = (Vector3)closest; lastClosestIndex = lastClosestIndexTemp; moveDir = (Vector3)closest; } else { lastClosest = Vector3.zero; lastClosestIndex = -1; moveDir = Vector3.zero; }
    }
        if(Vector3.Distance(new Vector3(player.transform.position.x,0, player.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z))< attackRange && Math.Abs(player.transform.position.y- transform.position.y) > 5) { moveDir = Vector3.zero; }
    }

    private IEnumerator collisionTimeout()
    {
        yield return new WaitForSeconds(1);
        collisionTime = true;
    }

    private bool canSeePlayer()
    {
        RaycastHit hitinfo = new RaycastHit();
        Physics.Raycast(transform.position + transform.up*2, player.transform.position - transform.position,  out hitinfo, aggroRange ) ;

        if(  hitinfo.collider != null && hitinfo.collider.gameObject.name!="Player" ) { return false; }else { return true; }
    }

    public bool isWeirdEdgeCase(Vector3 closest)
    {
        if(lastClosest == Vector3.zero) { return false; }
        bool comparison = Vector3.Dot(closest, (player.transform.position - this.transform.position).normalized) == Vector3.Dot(lastClosest, (player.transform.position - this.transform.position).normalized);
      //  Debug.Log("ENEMY: edge " +comparison);
        if(Vector3.Dot(closest,(player.transform.position- this.transform.position).normalized) == Vector3.Dot(lastClosest, (player.transform.position - this.transform.position).normalized) && !enemyOrientations.orientations[lastClosestIndex].isColliding ) { return true; }

        return false;

    }

    void FixedUpdate()
    {
        
        if (wheelTurn) { IncrementWheelRotation(); if (!moving.isPlaying) moving.Play(); } else { moving.Pause(); }
        if( currentState ==state.ATTACK) { growArm();  }else { shrinkArm();  }
        animator.SetInteger("State", ((int)currentState));
        bodyAnimator.SetInteger("State", ((int)currentState));
      //  Debug.Log("ENEMY: ISGROUNDED: " + isGrounded() + ", velocity:" + rb.velocity + ", state:" + currentState);
        Vector3 moveForce = moveDir * speed;
        //  if(moveDir!=Vector3.zero) transform.LookAt(transform.position + moveDir);

        if (!isHalfGravity || IsDying)
        {
            if (!isOnEdge()) rb.AddForce(Vector3.down * gravity + moveForce, ForceMode.VelocityChange);
        }
        else { if(!isOnEdge()) rb.AddForce(Vector3.down * halfGravityForce + moveForce, ForceMode.VelocityChange); }

        if (currentState == state.AGGRO || (currentState == state.ATTACK && !attackIsReady))
        { Vector3 lookPosition = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
            transform.LookAt(lookPosition, GetGroundNormal());
        }

        if (currentState != state.AGGRO) { moveDir = Vector3.zero; }
        if( rb.velocity.magnitude > 0.05f && isGrounded())
        {
            wheelTurn = true;
        }else { wheelTurn = false; }
    }

    private void IncrementWheelRotation()
    {
        wheel.transform.Rotate(0, -2, 0);
    }

    private void growArm()
    {
        
        if (armIsGrowing || arm.localScale== Vector3.one|| armIsShrinking) { return; }
        else
        {
            armIsGrowing = true;
            StartCoroutine(growArmCoroutine());
        }

    }

    private void shrinkArm()
    {
        return; //delete this later
        if (armIsShrinking || arm.localScale == Vector3.zero || armIsGrowing) { return; }else
        {
            armIsShrinking = true;
            StartCoroutine(shrinkArmCoroutine());
        }
       
    }
    IEnumerator shrinkArmCoroutine() {
        float maxTime = 0.25f;
        float currentTime = 0;
        arm.localPosition = defaultArmPos - new Vector3(0, 0, -2);
        while (maxTime > currentTime)
        {
            arm.localScale = Vector3.one- (Vector3.one * currentTime / maxTime);
            yield return null;
            currentTime += Time.deltaTime;
        }

        arm.localScale = Vector3.zero;
        armIsShrinking=false;

    }
    void OnDrawGizmos()
    {
       
        if (!Application.isPlaying || true)
        {
            EnemyOrientations eo = new(transform, sensitivity);
            foreach(EnemyOrientation dir in eo.orientations)
            {
                Gizmos.DrawLine(transform.position,transform.position+dir.direction*collisionRange);
            }
        }
        Gizmos.color= Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveDir * collisionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position+transform.forward*2, transform.position + transform.forward * 2 - transform.up*4f);
    }
    IEnumerator growArmCoroutine()
    {
        float maxTime = 0.25f;
        float currentTime = 0;
        arm.localPosition = defaultArmPos;
        while (maxTime > currentTime)
        {
            arm.localScale = Vector3.one * currentTime/maxTime;
            yield return null;
            currentTime += Time.deltaTime;
        }

        arm.localScale = Vector3.one;
        armIsGrowing = false;
    }

    private bool isItCloserThanTheClosest(Vector3 direction, Vector3? closest)
    { if (closest == null) { return true; }
        Vector3 enemyToPlayerNormalized = enemyToPlayer.normalized;
        float closestCloseness = Vector3.Dot(enemyToPlayerNormalized, (Vector3)closest);
        float closeness = Vector3.Dot(enemyToPlayerNormalized, direction);
        if (closeness > closestCloseness) { return true; }

        return false;

    }

    public override void Attack()
    {
        
       // Debug.Log("IM IN ATTACK MODE!"+ windUps.ToString()+ attackBeat.ToString());
        enemyToPlayer = player.transform.position - transform.position;

        if (attackIsReady && inAttackBeats(rs.beatIndex))
        {
            Punch();
            attackIsReady = false;
           
            if(enemyToPlayer.sqrMagnitude > squareAttackRange)
            {    
                currentState = state.AGGRO;
            }
        }
        else if (!attackIsReady && inWindUps(rs.beatIndex))
        {
            audioSource.PlayOneShot(windUp);
            attackIsReady = true;
        }else if (!attackIsReady && enemyToPlayer.sqrMagnitude > squareAttackRange) { currentState = state.AGGRO; }
        animator.SetBool("attackReady", attackIsReady);
        bodyAnimator.SetBool("attackReady", attackIsReady);
    }

    private void Adjust()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.CompareTag("Enemy")) { rb.AddForce((transform.position -hit.transform.position ).normalized * 5f, ForceMode.VelocityChange); return; }
        }
    }

    private void Punch()
    {
        
        animator.SetTrigger("Attack");
        audioSource.PlayOneShot(swing);
        attackIsReady = false;
      StartCoroutine(  TimeoutThenAction(PunchActual, 0.125f));
        if (attackRange == 0) { attackRange = MathF.Sqrt(squareAttackRange); }
        
        if (enemyToPlayer.sqrMagnitude > squareAttackRange)
        {
            currentState = state.AGGRO;
        }
    }

    private void PunchActual()
    {
       // Debug.Log("PUNCH " + gameObject.name + attackIsReady);
        RaycastHit[] hitinfo;
        hitinfo = Physics.SphereCastAll(this.transform.position, 4f, this.transform.forward, attackRange+0.5f);
        foreach(RaycastHit hit in hitinfo) {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("DodgeBall")) { gameManager.AddEnergyOrb(1,hit.collider.transform.position); break; };
        if (hit.collider != null && hit.collider.gameObject.name == "Player") { player.Hurt(); break; } else if (Vector3.Dot(enemyToPlayer.normalized,transform.forward.normalized)>=0 && enemyToPlayer.magnitude < 3) { player.Hurt(); break; }
        }
    }
    bool inAttackBeats(int number) {

        for (int i = 0; i < attackBeats.Length; i++)
        {
            if (attackBeats[i] == number) return true;
        }
        return false;

    }

    bool inWindUps(int number) {
    for(int i= 0; i < windUps.Length;i++)
        {
            if (windUps[i] == number) return true;
        }
        return false;
    }

    public override void Bounce(Vector3 dir)
    {
        if (IsDying) return;
       // Debug.Log("bounce dir" + dir);
        if (isGroundedBounce()) { 
        rb.AddForce(dir*knockbackForce*bounceMultiplier,ForceMode.VelocityChange);
            audioSource.PlayOneShot(bounce);
        halfGravity();
        }
        StunEnemy(3f);
    }
    public override void Spin()
    { if (IsDying) return;
        if (!isSpinning)
        {
            StunEnemy(1f);
            StartCoroutine(SpinCoroutine());
        }
    }

    IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        Quaternion currentRotation = transform.rotation;
        Quaternion endRotation = currentRotation * Quaternion.Euler(0, 180, 0);

        float durationCounter = 0;

        while (durationCounter<spinDuration) { 
            
          transform.rotation=  Quaternion.Slerp(currentRotation, endRotation, durationCounter/spinDuration);
            

            durationCounter += Time.deltaTime; yield return null; }

        transform.rotation= endRotation;
        isSpinning = false;


    }

    public override void Idle()
    {
        enemyToPlayer = player.transform.position - transform.position;
        if (enemyToPlayer.sqrMagnitude<squareAggroRange) { bodyAnimator.SetTrigger("Alert"); audioSource.PlayOneShot(alert,0.75f);  currentState = state.AGGRO; }
    }

    public void StunEnemy(float t)
    {
        if(stunRoutine != null) { StopCoroutine(stunRoutine); }
        audioSource.PlayOneShot(stun,0.75f);
      stunRoutine =  StartCoroutine(StunFor(t));
    }
    public IEnumerator StunFor(float seconds)
    {
        currentState = state.STUN;
       yield return new WaitForSeconds(seconds);
        currentState = state.IDLE;
    }
    public override void TakeDamage(int damage)
    {
        if (IsDying) return;
        audioSource.PlayOneShot(ouch,0.5f);
      
        if(damageRoutine != null) { StopCoroutine(damageRoutine);}
        damageRoutine = StartCoroutine(DamageCoroutine());
        currentHealth = currentHealth - damage - damage * revengeMeter / 100;
        if(currentHealth <= 0) { gameManager.AddEnergyOrb(2,transform.position); StartCoroutine(Death());  }
       // StartCoroutine(TurnRed());
    }

    // IEnumerator TurnRed()
    // {

    //     yield return WaitForSeconds(0.25f);
    //}

    IEnumerator DamageCoroutine()
    {
        Renderer r = transform.GetChild(0).GetChild(1).GetComponent<Renderer>();
       
        r.material.SetColor("_Color", Color.red);
       

        yield return new WaitForSeconds(1f);

        r.material.SetColor("_Color", defaultColor);

    }
    IEnumerator Death() {
        if (!IsDying) { 
        IsDying = true;
        knockbackForce = knockbackForce * 8;
        _explosion = Instantiate(Explosion, transform.position, Quaternion.identity, transform.parent);
        for (int i =0;i < transform.childCount;i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        currentState = state.WAIT;


        yield return new WaitForSeconds(2f);

        Destroy(_explosion);
        Destroy(gameObject);
        }

    }
     
    public override void Wait()
    {
        if (_explosion != null )
        {
            if ( _explosion.transform.localScale.magnitude < 20) { 
            _time += Time.deltaTime;
            _explosion.transform.localScale = Vector3.one * 20 * _time;
            }
            else { _explosion.transform.localScale = Vector3.zero; }
        }
        
    }

}
