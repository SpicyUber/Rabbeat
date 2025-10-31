using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DebugValues : MonoBehaviour
{   
    public TextMeshProUGUI BounceBonusValue, SpinNerfValue;
    private playerScript _player;
    // Start is called before the first frame update
    void Start()
    {
        _player=GameObject.FindFirstObjectByType<playerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        SpinNerfValue.text = "SPIN NERF: " + _player.SpinNerf;
       BounceBonusValue.text= "BOUNCE BONUS: "+ _player.BounceBonus;
    }
}
