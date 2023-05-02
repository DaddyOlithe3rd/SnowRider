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
    private List<SkierController> controller;
    public TMP_Text[] scoreText;

    void Start()
    {
        controller = new List<SkierController>();
        for (int i = 0; i < skier.Length; i++)
        {
            controller.Add(skier[i].GetComponent<SkierController>());   
        }
        for (int i = 0;i < scoreText.Length; i++)
        {
            scoreText[i].text = controller.ElementAt(i).score.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < skier.Length; i++)
        {
            if (controller[i].isDead)
            {
                controller[i].score = 0f;
                controller[i].isDead = false;
            }
            scoreText[i].text = Mathf.Round(controller[i].score).ToString();
        }
    }
}
