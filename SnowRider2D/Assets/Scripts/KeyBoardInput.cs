using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInput : MonoBehaviour
{
    SkierController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<SkierController>();
    }

    void FixedUpdate()
    {
        controller.status.tryJump = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        controller.status.tryCrouch = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        controller.status.tryUnCrouch = Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        controller.status.tryRotateColckWise = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        controller.status.tryRotateAntiColckWise = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
    }
}
