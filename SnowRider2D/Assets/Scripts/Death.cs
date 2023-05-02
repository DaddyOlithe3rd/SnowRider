using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    // Start is called before the first frame update
    public SkierController controller;
    void Start()
    {
        controller = GetComponentInParent<SkierController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        controller.isDead = true;
        if (!controller.isAI)
        {
            SceneManager.LoadScene("DeathScreen");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //controller.isDead = false;
    }
}
