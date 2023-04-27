using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform[] skiers;
    public Transform furthestTransform;

    private void Start()
    {
        furthestTransform = skiers[0];
    }

    void FixedUpdate()
    {
        for (int i = 0; i < skiers.Length; i++)
        {
            if (skiers[i].position.x > furthestTransform.position.x)
            {
                furthestTransform = skiers[i];
            }
        }
        transform.position = furthestTransform.position - new Vector3(0f,0f,10f);
    }
}
