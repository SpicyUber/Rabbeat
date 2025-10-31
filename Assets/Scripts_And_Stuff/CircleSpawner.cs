using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSpawner : MonoBehaviour
{
    private rhythmSystemScript rs;
    public GameObject circle;
    public GameObject UI;
    private int lastActiveIndex = 0;
    private bool _didInit=false;
    private Coroutine _coroutine;
    public Sprite BlueSprite, RedSprite;

    public Sprite CurrentSprite;

    // Start is called before the first frame update
    void Start()
    {
        CurrentSprite = BlueSprite;
        
         }
    public void Init()
    {
        GameObject c;
        rs = GameObject.FindFirstObjectByType<rhythmSystemScript>();
        if (rs.beatMap[0].isActive)
        {  c=GameObject.Instantiate(circle); c.GetComponent<CircleScript>().beatNumber = 0; c.GetComponent<CircleScript>().CircleSpawnerComponent = this; }
        if (rs.beatMap[1].isActive) {c= GameObject.Instantiate(circle, transform); c.GetComponent<CircleScript>().beatNumber = 1; c.GetComponent<CircleScript>().CircleSpawnerComponent = this; }
        if (rs.beatMap[2].isActive) { c=GameObject.Instantiate(circle, transform); c.GetComponent<CircleScript>().beatNumber = 2; c.GetComponent<CircleScript>().CircleSpawnerComponent = this; }
        if (rs.beatMap[3].isActive) { c= GameObject.Instantiate(circle, transform); c.GetComponent<CircleScript>().beatNumber = 3; c.GetComponent<CircleScript>().CircleSpawnerComponent = this; }
        _didInit = true;
    }
    // Update is called once per frame
    void Update()
    {  if(!_didInit) { return; }
        if( circle ==null) { circle = GameObject.Find("Circle"); if (circle == null) { circle = GameObject.Find("Circle(Clone)"); } }
        if (rs.beatMap[IndexPlusFour()].isActive && IndexPlusFour() != lastActiveIndex) { GameObject c= GameObject.Instantiate(circle, transform); c.GetComponent<CircleScript>().CircleSpawnerComponent = this; c.GetComponent<CircleScript>().beatNumber= IndexPlusFour(); lastActiveIndex = IndexPlusFour(); }
    }

    int IndexPlusFour() {

       return ( rs.beatIndex + 4) % rs.beatMap.Length;
    }

    public void Stun(float seconds)
    {
        if (_coroutine != null) { StopCoroutine(_coroutine); }

        _coroutine = StartCoroutine(Red(seconds));

    }

    public IEnumerator Red(float s)
    {
        CurrentSprite = RedSprite;

        yield return new WaitForSeconds(s);

        CurrentSprite = BlueSprite;
    }
}
