using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunSprite : MonoBehaviour
{
    public Sprite One, Two;
    private enemyScript enemy;
    public bool ON = true;
    // Start is called before the first frame update
    void Start()
    {
        enemy=GetComponentInParent<enemyScript>();
        StartCoroutine(StunAnimation());
    }

    IEnumerator StunAnimation()
    {
        while (ON) { 
            if(enemy.currentState== enemyScript.state.STUN) {
                GetComponent<SpriteRenderer>().sprite = One;
        yield return new WaitForSeconds(0.25f);
                GetComponent<SpriteRenderer>().sprite = Two;
                yield return new WaitForSeconds(0.25f);
            }
            else GetComponent<SpriteRenderer>().sprite = null;
            yield return null;
        }
           
        }
    
    }



