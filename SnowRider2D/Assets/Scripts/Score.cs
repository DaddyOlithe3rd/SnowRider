using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] skier;
    private List<float> score;
    public TMP_Text[] scoreText;

    void Start()
    {
        score =  new List<float>();
        for (int i = 0; i < skier.Length; i++)
        {
            score.Add(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < skier.Length; i++)
        {
            score[i] = Mathf.Round(skier[i].transform.position.x * 10);
            if (scoreText[i]!= null)
            {
                scoreText[i].text = score[i].ToString();
            }
        }
    }
}
