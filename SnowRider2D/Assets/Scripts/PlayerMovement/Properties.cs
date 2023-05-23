using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Properties
{
    public float initialRadius;
    public float rotationSpeed;
    public float jumpForce;
    public float minimumSpeed;
    public float unCrouchingSpeed;//Speed at which the skier uncrouches
    public float crouchingForce;
    public float rayCastLength;
    public float score;
    public LayerMask layerMask;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
