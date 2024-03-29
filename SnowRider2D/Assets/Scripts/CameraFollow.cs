using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform[] skiers;
    private Transform furthestTransform;
    public Transform toFollow;

    public bool followSpecificSkier;

    private void Start()
    {
        furthestTransform = skiers[0];
        if (!Settings.AI1)
        {
            skiers[1].gameObject.SetActive(false);
        }
        if (!Settings.AI2)
        {
            skiers[2].gameObject.SetActive(false);
        }
        if (Settings.perlin)
        {
            GameObject.Find("Perlin").SetActive(true);
            GameObject.Find("Controller").SetActive(false);
        }
        else if (Settings.bezier)
        {
            GameObject.Find("Perlin").SetActive(false);
            GameObject.Find("Controller").SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (followSpecificSkier)
        {
            transform.position = toFollow.position - new Vector3(0f, 0f, 10f);
        }
        else
        {
            for (int i = 0; i < skiers.Length; i++)
            {
                if (skiers[i].position.x > furthestTransform.position.x)
                {
                    furthestTransform = skiers[i];
                }
            }
            transform.position = furthestTransform.position - new Vector3(0f, 0f, 10f);
        }
        
    }
}
