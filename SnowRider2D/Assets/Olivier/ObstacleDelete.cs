using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDelete : MonoBehaviour
{
    public GameObject target;


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < target.transform.position.x - 20)
        {
            Destroy(gameObject);
            Debug.Log("Destroy Tree");
        }
    }
}
