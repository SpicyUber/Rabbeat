using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class rhythmSystemScript : MonoBehaviour
{
    public SongKeys SongKey;
    public UnityEvent BeatChanged;
    public CircleSpawner CircleSpawnerComponent;
    public float spamTime;
    public int longestString;
    public bool testing;
    public AudioSource testAudioSource;
    private int loopNumberPlusOne;
    private int loopNumber;
    public float debugBeatIndex = 0;
    public int bars;
    public AudioSource song;
    public float bpm=0;
    private double startTime;
    private double beatDuration;
    private double previousTime=-1;
    private class rollbackFrame
    {
       public bool isActive = false;
        public int beatIndex=-1;
    }
    private rollbackFrame[] rollbackFrames;
    //activeBeats represents a pattern that will be copied into the beat map (1 is active,-1 inactive)
    public enum beatType {INACTIVE,ACTIVE,SPECIAL }
    public beatType[] activeBeats;
    public int beatIndex;
    public float offset;

    public bool LongestStringIsRelevant;
    private bool _longestStringRelevant;
    public class Beat{
        public bool isActive { get; set; }
        public bool isSpecial { get; set; }
    }
    //each beat is actually one eight of a bar
    public Beat[] beatMap;
    public int stringMapLoopsEveryNBeats;
    private int prevBeat = -1;
    private int lastLongestStringBeatIndex = -1;
    
    public class longestStringClass
    {  
        public int beatIndex;
        public int longestStr;
       public longestStringClass(int beatIndex, int longestString)
        {
            longestStr = longestString;
            this.beatIndex = beatIndex;
        }
    }
    public int[] longestStringIndexes;
    public int[] longestStringStrings;
    public longestStringClass[] longestStringMap;
    private int previousBeat;

    // Start is called before the first frame update
    void Start()
    {
        InitializeRhythmSystem();
    }

    public void InitializeRhythmSystem() {
        _longestStringRelevant = LongestStringIsRelevant;
        rollbackFrames = new rollbackFrame[3];
        beatIndex = 0;
        song = GetComponent<AudioSource>();
        startTime = 0f;
        song.PlayScheduled(1f);
        if (testing) testAudioSource.PlayScheduled(1f);
        if (bars <= 0) throw new UnityException("Bpm is zero");
        if (activeBeats.Length == 0) throw new UnityException("activeBeats is empty");
        if (bars * 8 % activeBeats.Length != 0) throw new UnityException("activeBeats is in the wrong proportion");

        bpm = Mathf.RoundToInt((bars * 4) * 60 / song.clip.length);
        beatMap = new Beat[bars * 8];
        fillBeatMap();
        fillLongestStringMap();


        beatDuration = 60f / (bpm * 2);
    }
    void fillLongestStringMap()
    {
        if (!_longestStringRelevant) return; //shouldnt be here but im overriding.
        if(longestStringIndexes.Length!=longestStringStrings.Length || stringMapLoopsEveryNBeats==0 || beatMap.Length% stringMapLoopsEveryNBeats!=0) { Debug.Log("FILLED STRING MAP(BI: LS)-> FAILED"); throw new UnityException("LongestStringMap Init failed."); return; }
        Debug.Log("FILLED STRING MAP(BI: LS)-> beatMap.Length / stringMapLoopsEveryNBeats" + beatMap.Length / stringMapLoopsEveryNBeats+ "longestStringStrings.Length"+ longestStringStrings.Length);
        longestStringMap = new longestStringClass[longestStringStrings.Length*( beatMap.Length/stringMapLoopsEveryNBeats)];
        for(int k= 0; k< beatMap.Length / stringMapLoopsEveryNBeats; k++) { 
        for (int i = 0; i < longestStringStrings.Length; i++) { longestStringMap[i+ stringMapLoopsEveryNBeats*k] =new longestStringClass( longestStringIndexes[i]+stringMapLoopsEveryNBeats * k, longestStringStrings[i]);
            
                Debug.Log(" FILLED STRING MAP(BI: LS)-> i,k "+i+","+k);
            }
            Debug.Log(" FILLED STRING MAP(BI: LS)-> k " + k);
        }
        string DBLog = "FILLED STRING MAP (BI:LS) ->";
        for(int j=0; j<longestStringMap.Length; j++)
        {
            DBLog = DBLog + " " + longestStringMap[j].beatIndex + ":"+ longestStringMap[j].longestStr;
        }

        Debug.Log(DBLog);

    }
    public int GetLastActiveBeatIndex(int number) {
        if (beatMap.Length - 1 < number) { return -1; }
        for (int i=number; i >= 0; i--)
        {
         if (beatMap[i].isActive) { return i; }
        }
            return -1;
    }

    public bool AtLeastOneDifferent(int number)
    {
        for(int i = 0; i < rollbackFrames.Length; i++)
        {
            if (rollbackFrames[i]!=null && rollbackFrames[i].isActive && rollbackFrames[i].beatIndex!=number ) { return true; }
        }
        return false;

    }
   void Rollback(bool newBool, int newBI)
    {
        for(int i = 1; i < rollbackFrames.Length; i++)
        {
            rollbackFrames[i-1] = rollbackFrames[i];

        }

        rollbackFrames[rollbackFrames.Length - 1] = new rollbackFrame();
        rollbackFrames[rollbackFrames.Length - 1].isActive = newBool;
        rollbackFrames[rollbackFrames.Length - 1].beatIndex = newBI;

    }

    public bool GetRollback()
    {
       
        for( int i = 0;i < rollbackFrames.Length; i++)
        {
            if (rollbackFrames[i]!=null&&rollbackFrames[i].isActive==true) return true;
        }
        return false;
    }

    public int GetLastGoodFrame()
    {

        for (int i = rollbackFrames.Length-1; i >= 0; i--)
        {
            if (rollbackFrames[i].isActive == true) return rollbackFrames[i].beatIndex;
        }
        return -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void Update()
    {
        
        double currentTime = song.time;
        if(currentTime == previousTime) { currentTime = previousTime + Time.unscaledDeltaTime;  }
        double elapsedSongTime = currentTime - startTime;

       
        if (elapsedSongTime > song.clip.length)
        {
            startTime += song.clip.length;
            elapsedSongTime = currentTime - startTime;
        }

        
        debugBeatIndex = (float)((elapsedSongTime - offset) / beatDuration);

       
        beatIndex = customRound(debugBeatIndex);
        if (beatIndex >= beatMap.Length || beatIndex < 0) beatIndex = 0;
        if ((previousBeat == beatMap.Length-1 && beatIndex==0)|| (beatIndex!=0 && previousBeat< beatIndex)) { previousBeat = beatIndex; BeatChanged.Invoke();  }
            Rollback(beatMap[beatIndex].isActive, beatIndex);

        
        if (beatIndex != lastLongestStringBeatIndex && IsInLongestStringMap(beatIndex))
        {
            lastLongestStringBeatIndex = beatIndex;
        }

       //for testing
      /*  if (beatMap[beatIndex].isActive && beatIndex != prevBeat && testing)
        {
            prevBeat = beatIndex;
            StartCoroutine(testBeat(beatIndex));
        }*/
      

        previousTime = currentTime;
    }
    private bool IsInLongestStringMap(int number)
    {
        if(!_longestStringRelevant)return true; //this shouldnt be here but  its a temp solution.
        for (int i = 0; i < longestStringMap.Length; i++)
        {

            if (longestStringMap[i].beatIndex == number)
            {
                longestString = longestStringMap[i].longestStr; return true;
            }

        }
        return false;

    }
    private IEnumerator testBeat(int bI)
    {
        testAudioSource.Play();
        while (bI == beatIndex || beatMap[beatIndex].isActive)
        {
            
            yield return null;
        }
        testAudioSource.Stop();

    }
    public void fillBeatMap() {
        int k = 0;
        for (int i = 0; i < beatMap.Length; i++) {
            beatMap[i] = new Beat();
        switch (activeBeats[k])
            {
                case beatType.INACTIVE:
                    beatMap[i].isActive = false;
                    beatMap[i].isSpecial = false;
                    break;
                    case beatType.ACTIVE:
                    beatMap[i].isActive = true;
                    beatMap[i].isSpecial = false;
                    break;
                case beatType.SPECIAL:
                    beatMap[i].isActive = true;
                    beatMap[i].isSpecial = true;
                    break;
                default:throw new UnityException("This should never happen");
            }
            k = (k + 1) % activeBeats.Length;    
        }
        CircleSpawnerComponent.Init();
    }
    private int customRound(float number)
    {

        int intNumber = Mathf.RoundToInt(number);

        if (number < 0 || number >= beatMap.Length) { return 0; }
        else return intNumber;

    }

    public void AddSubscription(UnityAction a)
    {
        BeatChanged.AddListener(a);
    }
    public void RemoveSubscription(UnityAction a)
    {
        BeatChanged.RemoveListener(a);
    }
}
