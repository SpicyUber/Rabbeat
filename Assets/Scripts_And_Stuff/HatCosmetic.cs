using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatCosmetic : MonoBehaviour
{
    private GameObject _bone;
    public float HeadOffset;
    private Coroutine _coroutine;
    private bool _goingUp;
    public GameObject AbilityObject;
    public GameObject HeadphoneModel;
    public GameObject PlayerModel;
    private GameObject _mesh;
    public Material BlueMaterial;
    private Material[] _material;
    public enum Ability {Attack=0, Jump=1, Heal }
    private Ability CurrentAbility = Ability.Attack;
    [Header("Ability Prefabs")]
    public GameObject Attack,Jump;
    public HeadphoneAbilityUI headphoneAbilityUI;

    // Start is called before the first frame update
    void Start()
    {
        headphoneAbilityUI = FindAnyObjectByType<HeadphoneAbilityUI>();
        _bone = GameObject.FindGameObjectWithTag("HeadBone");
        _mesh = transform.GetChild(0).gameObject;
    }

   public void SetAbility(Ability ability)
    {
        SetUI(ability);
        CurrentAbility = ability;
    }

    public void SetUI(Ability ability)
    {
        switch (ability)
        {

            case Ability.Attack:
                headphoneAbilityUI.SetAttack();
                break;
            case Ability.Jump:
                headphoneAbilityUI.SetJump();
                break;
            case Ability.Heal:
                headphoneAbilityUI.SetHeal();
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
            _mesh.SetActive(AbilityObject == null && PlayerModel.transform.localScale != Vector3.zero);


      
        transform.position = _bone.transform.position + _bone.transform.up * HeadOffset;

        transform.rotation = _bone.transform.rotation;
    }

    public void TurnBlue() {
      MeshRenderer[] mr =  transform.GetChild(0).GetComponentsInChildren<MeshRenderer>();
         _material = new Material[mr.Length]; 
        for(int i =0; i<mr.Length; i++) { _material[i] = mr[i].material; mr[i].material = BlueMaterial; }
        StartCoroutine(BlueRoutine(mr,_material)); }

    IEnumerator BlueRoutine(MeshRenderer[] mr, Material[] material)
    {
        

        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < mr.Length; i++) { mr[i].material = material[i]; }
    }

   public void BobUp() {
        if (_goingUp) return;

    if( _coroutine != null ) { StopCoroutine(_coroutine); }

        _coroutine = StartCoroutine(BobUpRoutine());

    
    }

    public bool ActivateAttack()
    {
        CustomGameManager cgm = FindAnyObjectByType<CustomGameManager>();
       
        if (cgm.currentPlayerEnergy < cgm.playerEnergyCap || AbilityObject != null) { return false; }
    else {
            headphoneAbilityUI.SetHidden();
            cgm.DepleteEnergy();
            //cgm.currentPlayerEnergy=0;
            GameObject g;
            switch (CurrentAbility)
            {
                case Ability.Attack:
                    AbilityObject = Instantiate(Attack, transform.position, Quaternion.LookRotation(GameObject.FindGameObjectWithTag("Player").transform.forward), transform.parent.parent);
                  //  if (_material != null && _material.Length > 0) { MeshRenderer[] mr = HeadphoneModel.GetComponentsInChildren<MeshRenderer>(); for (int i = 0; i < mr.Length; i++) { mr[i].material = _material[i]; } }
                    g = Instantiate(HeadphoneModel, AbilityObject.transform);

                    AbilityObject.GetComponent<SphereCollider>().transform.position = g.transform.position;
                    break;
                case Ability.Jump:
                    AbilityObject = Instantiate(Jump, transform.position, Quaternion.LookRotation(GameObject.FindGameObjectWithTag("Player").transform.forward), transform.parent.parent);
                    //  if (_material != null && _material.Length > 0) { MeshRenderer[] mr = HeadphoneModel.GetComponentsInChildren<MeshRenderer>(); for (int i = 0; i < mr.Length; i++) { mr[i].material = _material[i]; } }
                     g = Instantiate(HeadphoneModel, AbilityObject.transform);

                    AbilityObject.GetComponent<SphereCollider>().transform.position = g.transform.position;
                    break;
                case Ability.Heal:
                    break;
            }
            return true;
         

                  }   
    }
     

    IEnumerator BobUpRoutine()
    {
        _goingUp = true;
        float t = 0;
        float offset = HeadOffset;
        while (t < 1f)
        {
            HeadOffset= Mathf.Lerp(offset,1.7f, t);
            yield return null;
            t+=Time.deltaTime;
        }
        HeadOffset = 1.7f;
    }

    public void ResetBob()
    {
        if (!_goingUp) return;
        if (_coroutine != null) { StopCoroutine(_coroutine); }

        _coroutine = StartCoroutine(ResetBobRoutine());

    }

    IEnumerator ResetBobRoutine()
    { _goingUp= false;
        float t = 0;
        float offset = HeadOffset;
        while (t < 0.2f)
        {
            HeadOffset= Mathf.Lerp(offset, 1.5f, t*5);
            yield return null;
            t += Time.deltaTime;
        }
        HeadOffset = 1.5f;
    }
}
