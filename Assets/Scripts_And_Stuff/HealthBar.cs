using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject carrotPrefab;
    public CustomGameManager gameManager;
    public GameObject[] carrots;
    public float gap;
    private int playerHealthCache;
    private Color defaultColor;
    private Color otherColor;
    // Start is called before the first frame update
    void Start()
    {
        otherColor = Color.black;
        defaultColor = Color.white;
        playerHealthCache= gameManager.playerHealth;
        carrots = new GameObject[gameManager.playerHealth];
        for (int i = 0; i <gameManager.playerHealth; i++)
        {
            carrots[i] = Instantiate(carrotPrefab,transform.position-new Vector3(gap * i*Screen.width/500, 0, 0 ),Quaternion.identity,this.transform);
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.currentPlayerHealth!=playerHealthCache)
        {
            playerHealthCache = gameManager.currentPlayerHealth;
            for(int i = 0;i < gameManager.playerHealth; i++)
            {
                if (i < gameManager.currentPlayerHealth) { carrots[i].GetComponent<CanvasRenderer>().SetColor(defaultColor); } else { carrots[i].GetComponent<CanvasRenderer>().SetColor(otherColor); }
            }

        }
        
    }

   
}
