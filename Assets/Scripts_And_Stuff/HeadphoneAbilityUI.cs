using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadphoneAbilityUI : MonoBehaviour
{

    public Sprite attack, jump, heal;
    private Image current;
    // Start is called before the first frame update
    void Start()
    {
        current = GetComponent<Image>();
        current.sprite = attack;
        SetHidden();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetShow() { current.color = new Color(1, 1, 1, 1); }

    public void SetHidden() { current.color = new Color(1,1,1,0); }

    public void SetAttack() { current.sprite = attack; }

    public void SetJump() { current.sprite = jump; }

    public void SetHeal() { current.sprite = heal; }
}
