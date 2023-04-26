using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        SkierController.isDead = true;
        print("Mort");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        SkierController.isDead = false;
    }
}
