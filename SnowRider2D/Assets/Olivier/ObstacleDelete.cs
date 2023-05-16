using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDelete : MonoBehaviour
{
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        //Targer correspond à la caméra principale qui suit le joueur le plus rapide
        target = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        //Si un obstacle sort du champ de la caméra, il sera supprimé
        if (transform.position.x < target.transform.position.x - 20)
        {
            Destroy(gameObject);
        }
    }
}
