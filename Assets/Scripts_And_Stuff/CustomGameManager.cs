using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class CustomGameManager : MonoBehaviour
{
    public int noteCount=0;
    public int playerHealth;
    public int playerEnergyCap;
    private int playerEnergyBottom = 35;
    public int currentPlayerEnergy;
    public int currentPlayerHealth=3;
    private TextMeshProUGUI noteCounter;
    private bool isDying;
    private GameObject GameOverScreen;
    public CircleSpawner CircleSpawnerComponent;
    public int multiplier;
    public GameObject EnergyOrb;
    private HeadphoneAbilityUI headphoneAbilityUI;

    // Start is called before the first frame update
    void Start()
    {
        headphoneAbilityUI = FindAnyObjectByType<HeadphoneAbilityUI>();
        multiplier = 1;
        currentPlayerEnergy = playerEnergyBottom;
        if(CircleSpawnerComponent == null)
        {
            throw new UnityException("CircleSpawnerComponent needs to be assigned to the gamemanager");
        }
        
     
        GameOverScreen = GameObject.FindWithTag("GameOverScreen");
        if (GameOverScreen == null) throw new UnityException("Game over screen needs to be assigned to gamemanager");
        isDying = false;
        if (PlayerPrefs.GetInt("Lives") == 0) { PlayerPrefs.SetInt("Lives", 5); }
        noteCounter= GameObject.FindGameObjectWithTag("NoteCounter").GetComponent<TextMeshProUGUI>();
        currentPlayerHealth = playerHealth;
        noteCount = 0;
      //  Application.targetFrameRate = 60;
    }
    
    // Update is called once per frame
    void Update()
    {
       // Debug.Log("NUMBER OF NOTES" + noteCount);
        noteCounter.text="X"+noteCount.ToString();
    }
    public void DecrementMultiplier() { if (multiplier > 0) { multiplier--; } }

    public void IncrementMultiplier() {if( multiplier < 16) { multiplier++; } }
    public void AddEnergyOrb(int number, Vector3 worldPos)
    {
        AddEnergyOrb(number,worldPos,true);
    }

    public void AddEnergy()
    {
        AddEnergy(1);
        }
    public void AddEnergy(int level)
    {
        int number = LevelToNumber(level);
        if (currentPlayerEnergy + number > playerEnergyCap) { currentPlayerEnergy = playerEnergyCap; } else currentPlayerEnergy += number;
        if (currentPlayerEnergy == playerEnergyCap) if (headphoneAbilityUI != null) headphoneAbilityUI.SetShow();

    }

    private int LevelToNumber(int level)
    {
        if(level<=0) return 0;
        switch (level) { case 1:return 1; case 2: return 5; case 3: return 10; case 4: return 25; case 5: return 50; }
        return 0;
    }
    public void AddEnergyOrb(int number, Vector3 worldPos, bool useMultiplier)
    {
        if (multiplier < 1 || !useMultiplier)
        {
            multiplier = 1;
        }
        if (number < 1) { number = 1; }
        int counter = multiplier * number;
        int increment = 1;
        for (int i = 0; i < counter; i = i+increment) {  EnergyOrb e = GameObject.Instantiate(EnergyOrb, worldPos, Quaternion.identity).GetComponent<EnergyOrb>(); e.SetLevel(CalculateOrbSize(i, counter, ref increment)); }

    }

    private int CalculateOrbSize(int i, int counter, ref int increment)
    {
        int gap = counter - i;
        if(gap <= 0) return 0;
        
        if(gap>=50) { increment = 50; return 5; }
        else if (gap >= 25) { increment = 25; return 4; }
        else if (gap >= 10) { increment = 10; return 3; }
        else if (gap >= 5) { increment = 5; return 2; }
        else
        {

            increment = 1; return 1;
        }

    }


    public void Stun(float seconds)
    {
        CircleSpawnerComponent.Stun(seconds);

    }
    public void AddNote()
    {
        noteCount++;

    }

    
    public void Death()
    {
        if(isDying) { return; }
        isDying = true;
        StartCoroutine(GameOver());


    }
    IEnumerator GameOver()
    {
        while (GameOverScreen.GetComponent<RectTransform>().anchoredPosition.y > -125) {
            GameOverScreen.GetComponent<RectTransform>().anchoredPosition = GameOverScreen.GetComponent<RectTransform>().anchoredPosition - new Vector2(0, 2000) * Time.deltaTime; yield return null;
        };
        GameOverScreen.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -225);
        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetInt("Lives", PlayerPrefs.GetInt("Lives")-1);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name,LoadSceneMode.Single);
    }

    public void DepleteEnergy()
    {
        currentPlayerEnergy= playerEnergyBottom;
    }
}
