using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathScreenScore : MonoBehaviour
{
    public TMP_Text finalScore;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.playerScore != null) 
        {
            print("Player Score: "+ GameManager.playerScore);
            finalScore.text = "Pointage final "+ GameManager.playerScore;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
