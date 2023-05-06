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
        if (Score.playerScore != null) 
        {
            print("Player Score: "+Score.playerScore);
            finalScore.text = "Pointage final "+Score.playerScore;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
