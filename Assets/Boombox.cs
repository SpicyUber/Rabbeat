using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class Boombox : MonoBehaviour
{
    public enum ChatIcon {
    
        None=-1,Boombox=0 
    }
    public TextMeshProUGUI Message;
    public  Image SelectedIcon;
    public Sprite[] Icons;
    public ChatIcon IconIndex;
    public string Text;
    public int EnterRange;
    public int ExitRange;
    private bool _isTalking;
    private GameObject _player;
    private Coroutine _coroutine;
    public AudioClip SpeakSFX;
    public float PauseDurationInSeconds;
    public float LongPauseDurationInSeconds;
    public GameObject ChatPanel;
    private Vector3 _startingScale;
    private bool _goingUp;
    private bool _talkAnimation = false;

    public bool IsTalking { get { return _isTalking; }    }

    // Start is called before the first frame update
    void Start()
    {
        _startingScale = transform.localScale;  
        if (EnterRange <= 0) { EnterRange = 5; }
        if (EnterRange > ExitRange) { ExitRange = EnterRange+1; }
        if (PauseDurationInSeconds < 0.01f) { PauseDurationInSeconds = 0.1f; }
        if (LongPauseDurationInSeconds < 0.5f) { LongPauseDurationInSeconds = 0.5f; }
        _player = GameObject.FindGameObjectWithTag("Player");
        if (Message == null || SelectedIcon == null || ChatPanel ==null) { throw new UnityException("Boombox not properly initiated!"); }
        if(IconIndex != ChatIcon.None)
        {
            SelectedIcon.sprite = Icons[((int)IconIndex)];

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isTalking && (transform.position-_player.transform.position).magnitude<=EnterRange ) {
        _isTalking = true;
            SayText();
        }else
        if((transform.position - _player.transform.position).magnitude > ExitRange && _isTalking)
        {
           
            _isTalking = false;
            StopText();
        }
        if (_isTalking)
        {
            Vector3 temp = _player.transform.position;
            temp.y = transform.position.y;
            if ((_player.transform.position - transform.position).magnitude > 0.5f)
            {
                transform.LookAt(temp);
            }
        }

        if(_talkAnimation) {
            int sign = (_goingUp? 1 : -1);
            transform.localScale = transform.localScale * (1 +  Time.deltaTime * 1.5f * sign );
            
            if(transform.localScale.magnitude >= _startingScale.magnitude*1.05f ) { _goingUp = false; }
             else  if (transform.localScale.magnitude<= _startingScale.magnitude * 0.95f ) { _goingUp = true; }
           

            
        } else
        {
           transform.localScale= _startingScale;
        }
        
    }
 

    void SayText( )
    {
        string[] tokens = Text.Split(" ");
        ChatPanel.SetActive(true);
        Message.text = "";
        _coroutine = StartCoroutine(Chatting(tokens));
    }
    
    void StopText( )
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _talkAnimation = false;
        ChatPanel.SetActive( false );
        
    }

    IEnumerator Chatting(string[] tokens)
    {
        float speed = 1f; 
        bool clearFlag = false;
        int i = 0;
        while (_isTalking && i<tokens.Length)
        {
           
            
            if (clearFlag) { Message.text = ""; clearFlag = false; yield return null;  }
           
                
                
            
             

            if (tokens[i].EndsWith(";")) {
                clearFlag = true;
                _talkAnimation = false;
                Message.text = Message.text + tokens[i].TrimEnd(';');
                GetComponent<AudioSource>().PlayOneShot(SpeakSFX,1f/speed);
                yield return null;
                while (!Input.GetKeyDown(KeyCode.Q))
                yield return null;
                i++;

            }
            else if (tokens[i].EndsWith(".") || tokens[i].EndsWith("?") || tokens[i].EndsWith("!"))
            {
                Message.text = Message.text + tokens[i] + " ";

                if (!Input.GetKeyDown(KeyCode.Q))
                    GetComponent<AudioSource>().PlayOneShot(SpeakSFX, 1f / speed);
                _talkAnimation = false;
                float tempTime = 0;
                while(!Input.GetKeyDown(KeyCode.Q) && tempTime< PauseDurationInSeconds * 3 / speed)
                {
                     
                    yield return null;
                    tempTime += Time.deltaTime;
                }
                _talkAnimation=true;
               
                i++;
            }
            else { 
            Message.text = Message.text + tokens[i]+ " ";


                float tempTime = 0;
                if(!Input.GetKeyDown(KeyCode.Q))
                GetComponent<AudioSource>().PlayOneShot(SpeakSFX, 1f / speed);
                while (!Input.GetKeyDown(KeyCode.Q) && tempTime < PauseDurationInSeconds / speed)
                {
                    
                    yield return null;
                    tempTime += Time.deltaTime;
                }
                _talkAnimation=true;
                i++;
            }
        }
        _talkAnimation = false;

    }
}
