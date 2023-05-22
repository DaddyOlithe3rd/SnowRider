using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class SkierController : MonoBehaviour
{
    Rigidbody2D rb;
    public Status status;
    public Properties properties;

    public CapsuleCollider2D capsuleCollider;
    public Transform head;
    public Transform feet;
    public Transform bottomPointObject;


    public float angle;//Angle between the collision vector and the horizontal(Vector2.right)
    public float currentRotationSpeed;
    public float timeAfterJump = 0;
    public float timeBeforeJump = 0;

    private Vector2 normal = Vector2.up;
    private Vector3 bottomPoint;
    public Vector2 lastNorm;
    public Vector3 initialScale;
    public Vector2 speedBeforeUnCrouching;
    public Vector2 lastSpeed;//Speed from previous fixed update

    //Initializing variables
    void Start()
    {
        //status = new Status();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        head = transform.Find("head");
        feet = transform.Find("feet");
        bottomPointObject = transform.Find("bottomPoint");
        bottomPoint = bottomPointObject.transform.position;
        lastNorm = Vector2.zero;
        initialScale = transform.localScale;
        speedBeforeUnCrouching = rb.velocity;
        currentRotationSpeed = properties.rotationSpeed;
        properties.initialRadius = properties.initialRadius * Mathf.Abs((head.position - feet.position).magnitude) / 2;
    }

    public void CustomUpdate()
    {
        bottomPoint = bottomPointObject.transform.position;//Finding the bottom point of the skier for the raycast and the skier's rotation

        ContactPoint2D[] arrayContacts = new ContactPoint2D[16];
        int nbContacts = rb.GetContacts(arrayContacts);

        /*This raycast is used to detect the ground. If the ground is close enough according to the properties.rayCastLength, the 
         isGrounded variable will stay true*/
        RaycastHit2D groundRay = Physics2D.Raycast(bottomPoint, -properties.rayCastLength * (transform.position - bottomPoint).normalized, properties.rayCastLength, properties.layerMask);
        Debug.DrawRay(bottomPoint, -properties.rayCastLength * (transform.position - bottomPoint).normalized, Color.red);

        if (nbContacts != 0)
        {
            status.hasJumped = false;
            status.isFrontFlipping = false;
            status.isBackFlipping = false;
            timeAfterJump = properties.timeBeforeFlipping;
        }
        if (nbContacts == 0 && !groundRay) status.isGrounded = false;
        else
        {
            status.isGrounded = true;
            List<ContactPoint2D> contacts = new List<ContactPoint2D>();
            for (int i = 0; i < 16; i++)
            {
                if (arrayContacts[i].collider != null)
                {
                    contacts.Add(arrayContacts[i]);
                    if (contacts.ElementAt(i).collider.gameObject.name != "Segment(Clone)" && contacts.ElementAt(i).collider.gameObject.name != "line")
                    {
                        status.isDead = true;
                    }
                }
            }
            normal = groundRay.normal;
            if (contacts.Count != 0)
            {
                normal = contacts.ElementAt(contacts.Count - 1).normal;
            }

            /*If the skier is still on the same slope angle, there is no need to update its rotation, if it isn't, the
             rotation of the skier is adjusted to match the normal of the slope*/
            if (lastNorm != normal)
            {
                angle = (Vector2.SignedAngle(Vector2.up, normal));
                transform.RotateAround(bottomPoint, Vector3.forward, (angle - transform.eulerAngles.z));
                lastNorm = normal;
            }
        }

        //Player input
        if (status.tryJump)
        {
            jump();
        }
        if (status.tryCrouch)
        {
            crouch();
        }
        if (status.tryUnCrouch && status.isCrouched)
        {
            speedBeforeUnCrouching = rb.velocity;
            unCrouch();
        }
        if ((status.tryFrontFlip && timeAfterJump > 0) || status.isFrontFlipping)
        {
            rotateClockwise();
        }
        if ((status.tryBackFlip && timeAfterJump > 0) || status.isBackFlipping)
        {
            rotateAntiClockwise();
        }

        if (status.isUnCrouching)
        {
            unCrouch();
        }
        if (status.hasJumped)
        {
            timeAfterJump--;
        }
        if (status.isCrouched)
        {
            rb.AddForce(-lastSpeed.normalized * properties.crouchingForce, ForceMode2D.Force);
            rb.AddForce(rb.velocity.normalized * properties.crouchingForce, ForceMode2D.Force);
        }

        //Making sure the skier always has a minimum positive x axis speed
        if (rb.velocity.magnitude < properties.minimumSpeed)
        {
            rb.velocity = Vector2.right * (properties.minimumSpeed + 0.1f);
        }

        lastSpeed = rb.velocity;
    }

    // Update is called once per frame
    void FixedUpdate() 
    {
        
    }

    //Making the skier jump
    public void jump()
    {
        if (status.isGrounded && !status.hasJumped)
        {
            rb.AddForce(Vector2.up * properties.jumpForce, ForceMode2D.Impulse);
            transform.position += new Vector3(0.1f, 0.1f, 0f);
            status.hasJumped = true;
            print(gameObject.name + " jumped");
        }
    }

    //Making the skier crouch
    public void crouch()
    {
        print("Crouch");
        if (!status.isCrouched && !status.isUnCrouching)
        {
            rb.AddForce(rb.velocity.normalized * properties.crouchingForce, ForceMode2D.Force);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.6f, 0f);
            float radius = Mathf.Abs((head.position - feet.position).magnitude) / 2;
            currentRotationSpeed = (Mathf.Pow(properties.initialRadius, 2) / Mathf.Pow(radius, 2)) * properties.rotationSpeed;
            status.isCrouched = true;
        }
    }

    //Making the skier unCrouch
    public void unCrouch()
    {
        print("UnCrouch");
        status.isUnCrouching = true;
        if(transform.localScale.y < initialScale.y)
        {
            transform.localScale += new Vector3(0f, properties.unCrouchingSpeed * Time.fixedDeltaTime * initialScale.y, 0f);
        }
        if (transform.localScale.y >= initialScale.y)
        {
            currentRotationSpeed = properties.rotationSpeed;
            transform.localScale = initialScale;
            status.isUnCrouching = false;
            status.isCrouched = false;
            if (status.isGrounded)
            {
                rb.velocity = speedBeforeUnCrouching;
                jump();
            }
            rb.AddForce(-1 * rb.velocity.normalized * properties.crouchingForce, ForceMode2D.Force);
        }
    }

    //Making the skier turn clockWise according to the predefined rotation speed
    public void rotateClockwise()
    {
        status.isFrontFlipping = true;
        transform.rotation = eulerToQuaternion(transform.rotation.eulerAngles.z - currentRotationSpeed);
    }

    //Making the skier turn antiClockWise according to the predefined rotation speed
    public void rotateAntiClockwise()
    {
        status.isBackFlipping = true;
        transform.rotation = eulerToQuaternion(transform.rotation.eulerAngles.z + currentRotationSpeed);
    }

    //Converting an angle to a quaternion, this angle must ba an angle of rotation relative to the z axis
    Quaternion eulerToQuaternion(float angle)
    {
        /*The quaternion use here is simplified because the rotation is exactly arounf the Z axis, which means that
         the first two compenent representing the rotation around the x and y axis  are zero. */
        float angleRad = Mathf.Deg2Rad * angle;
        Quaternion rotation = new Quaternion(0f, 0f, Mathf.Sin((angleRad) / 2), Mathf.Cos((angleRad) / 2));
        return rotation;
    }
}