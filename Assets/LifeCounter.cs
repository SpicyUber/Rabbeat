using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeCounter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       GetComponent<TextMeshProUGUI>().text= "x"+PlayerPrefs.GetInt("Lives");
    }

    
}
