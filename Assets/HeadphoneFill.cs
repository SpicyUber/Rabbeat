using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadphoneFill : MonoBehaviour
{
    private Image _img;
    private Coroutine _coroutine;
    private Color _startColor;
    public Pulse PulseAnim;
    private CustomGameManager _gameManager;
    public Image HeadphoneSprite;
    // Start is called before the first frame update
    void Start()
    {
        _img = GetComponent<Image>();
        _startColor = _img.color;
         _gameManager = FindAnyObjectByType<CustomGameManager>();
        
    }
    public void Pulse()
    {
        if(_coroutine != null) { StopCoroutine(_coroutine); }
        _coroutine = StartCoroutine(PulseRoutine());
        if (_gameManager.currentPlayerEnergy == _gameManager.playerEnergyCap) { Ready(); } else
        {
            HeadphoneSprite.color = Color.black;
        }
    }

    public void Ready()
    {
        GetComponent<AudioSource>().Play();
        PulseAnim.Play();
        HeadphoneSprite.color = Color.white;
    }

    IEnumerator PulseRoutine()
    {
        float t = 0f;
        _img.color = Color.white;
        while(t<0.25f) {
        _img.color = new Color(Mathf.Lerp(255,_startColor.r,t*4), Mathf.Lerp(255, _startColor.g, t * 4), Mathf.Lerp(255, _startColor.b, t * 4), _startColor.a);


            yield return null;
            t += Time.deltaTime;


        }

        _img.color = _startColor;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
