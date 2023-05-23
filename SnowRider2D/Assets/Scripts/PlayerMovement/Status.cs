using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Status
{
    // Start is called before the first frame update
    public bool isDead;
    public bool isGrounded;
    public bool hasJumped;
    public bool isCrouched;
    public bool isUnCrouching;
    public bool isAI;
    public bool isBackFlipping;
    public bool isFrontFlipping;

    public bool tryJump;
    public bool tryRotateColckWise;
    public bool tryRotateAntiColckWise;
    public bool tryCrouch;
    public bool tryUnCrouch;
    public bool tryBackFlip;
    public bool tryFrontFlip;
    public Status()
    {
        isDead = false;
        isGrounded = false;
        hasJumped = false;
        isCrouched = false;
        isUnCrouching = false;
        tryJump = false;
        tryRotateColckWise = false;
        tryRotateAntiColckWise = false;
        tryCrouch = false;
        tryUnCrouch = false;
    }

}
