using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] skiers;
    public List<SkierController> skierControllers;

    private List<float> score;
    public static string playerScore;
    public TMP_Text[] scoreText;

    void Start()
    {
        skiers[1].SetActive(Settings.AI1);
        skiers[2].SetActive(Settings.AI2);

        GameObject.Find("Perlin").SetActive(Settings.perlin);
        GameObject.Find("Controller").SetActive(Settings.bezier);

        skierControllers = new List<SkierController>();
        score =  new List<float>();
        for (int i = 0; i < skiers.Length; i++)
        {
            skierControllers.Add(skiers[i].GetComponent<SkierController>());
            score.Add(0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < skiers.Length; i++)
        {
            skierControllers[i].CustomUpdate();
            //score[i] = Mathf.Round(skiers[i].transform.position.x * 10);
            score[i] = skierControllers[i].properties.score;
            if (scoreText[i]!= null)
            {
                scoreText[i].text = score[i].ToString();
                playerScore = scoreText[0].text;
                print("score");
            }
            if (skierControllers[i].status.isDead)
            {
                if (skierControllers[i].status.isAI)
                {
                    skiers[i].SetActive(false);
                }
                else
                {
                    SceneManager.LoadScene("DeathScreen");
                    print(skierControllers[i].name);
                }
            }
        }
    }
}
