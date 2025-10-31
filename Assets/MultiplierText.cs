using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplierText : MonoBehaviour
{
   private CustomGameManager _gameManager;
    private TextMeshProUGUI _text ;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.FindAnyObjectByType<CustomGameManager>();
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.multiplier < 2) { _text.text = ""; } else
       _text.text="X"+_gameManager.multiplier+" MULTIPLIER";
        _text.color = Color.Lerp(Color.white,Color.red, _gameManager.multiplier/16f);
    }
}
