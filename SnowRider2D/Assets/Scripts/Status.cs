using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    // Start is called before the first frame update
    public bool isDead;
    public bool isGrounded;
    public bool hasJumped;
    public bool isCrouched;
    public bool isUnCrouching;
    public bool isAI;

    public bool tryJump;
    public bool tryRotateColckWise;
    public bool tryRotateAntiColckWise;
    public bool tryCrouch;
    public bool tryUnCrouch;
    public Status()
    {
        isDead = false;
        isGrounded = false;
        hasJumped = false;
        isCrouched = false;
        isUnCrouching = false;
        isAI = false;
        tryJump = false;
        tryRotateColckWise = false;
        tryRotateAntiColckWise = false;
        tryCrouch = false;
        tryUnCrouch = false;
    }

}
