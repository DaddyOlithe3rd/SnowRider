using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update
    public float score;
    public TMP_Text scoreText;

    void Start()
    {
        score = 0;
        scoreText.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        SkierController controller = GetComponentInParent<SkierController>();
        if (SkierController.isDead)
        {
            score = 0;
        }
        score = score + Time.fixedDeltaTime * 10;
        scoreText.text = Mathf.Round(score).ToString();
        
    }
}
