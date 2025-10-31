using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadphoneFillManager : MonoBehaviour
{
    public CustomGameManager GameManager;
    private Slider _slider;
    // Start is called before the first frame update
    void Start()
    {
    _slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        _slider.value = GameManager.currentPlayerEnergy;      
    }
}
