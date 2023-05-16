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
    public  CapsuleCollider2D capsuleCollider;
    public Transform head;
    public Transform feet;
    public Transform bottomPointObject;

    public bool isGrounded;
    public bool isCrouched;
    public bool isUncrouching;
    public bool isAI;
    public bool isDead;
    public bool hitGround;
    float angle;//Angle between the collision vector and the horizontal(Vector2.right)
    public float initialRadius;
    public float rotationSpeed;
    public float currentRotationSpeed;
    public float jumpForce;
    public float minimumSpeed;
    public float unCrouchingSpeed;//Speed at which the skier uncrouches
    public float crouchingForce;
    public float rayCastLength;

    public LayerMask layerMask;

    private Vector2 normal = Vector2.up;
    private Vector3 bottomPoint;
    public Vector2 lastNorm;
    public Vector3 initialScale;
    public Vector2 speedBeforeUnCrouching;
    public Vector2 lastSpeed;//Speed from previous fixed update

    //Initializing variables
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        head = transform.Find("head");
        feet = transform.Find("feet");
        bottomPointObject = transform.Find("bottomPoint");
        bottomPoint = bottomPointObject.transform.position;
        isGrounded = false;
        isCrouched = false;
        isUncrouching = false;
        isDead = false;
        hitGround = true;
        lastNorm = Vector2.zero;
        initialScale = transform.localScale;
        speedBeforeUnCrouching = rb.velocity;
        currentRotationSpeed = rotationSpeed;
        initialRadius = initialRadius * Mathf.Abs((head.position - feet.position).magnitude) / 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bottomPoint = bottomPointObject.transform.position;//Finding the bottom point of the skier for the raycast and the skier's rotation

        ContactPoint2D[] arrayContacts = new ContactPoint2D[16];
        int nbContacts = rb.GetContacts(arrayContacts);

        /*This raycast is used to detect the ground. If the ground is close enough according to the rayCastLength, the 
         isGrounded variable will stay true*/
        RaycastHit2D ground = Physics2D.Raycast(bottomPoint, -rayCastLength * (transform.position - bottomPoint).normalized, rayCastLength, layerMask);
        Debug.DrawRay(bottomPoint, -rayCastLength * (transform.position - bottomPoint).normalized, Color.red);

        if (nbContacts != 0)
        {
            hitGround = true;
        }
        if (nbContacts == 0 && !ground) isGrounded = false;
        else 
        {
            List<ContactPoint2D> contacts = new List<ContactPoint2D>();
            for (int i = 0; i < 16; i++)
            {
                if (arrayContacts[i].collider != null)
                {
                    contacts.Add(arrayContacts[i]);
                    if (contacts.ElementAt(i).collider.gameObject.name != "Segment(Clone)" && contacts.ElementAt(i).collider.gameObject.name != "line")
                    {
                        isDead = true;
                    }
                }
            }
            isGrounded = true;
            normal = ground.normal;
            if (contacts.Count != 0)
            {
                normal = contacts.ElementAt(contacts.Count - 1).normal;
            }
           
            /*If the skier is still on the same slope angle, there is no need to update its rotation, if it isn't, the
             rotation of the skier is adjusted to match the normal of the slope*/
            if (lastNorm != normal )
            {
                angle = (Vector2.SignedAngle(Vector2.up, normal));
;               transform.RotateAround(bottomPoint, Vector3.forward, (angle - transform.eulerAngles.z));
                lastNorm = normal;
            }
        }

        //Player input
        if (!isAI)
        {
            if (Input.GetAxisRaw("Vertical") == 1)
            {
                jump();
            }
            if (Input.GetAxisRaw("Vertical") == -1)
            {
                crouch();
            }
            if (((Input.GetKeyUp(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") == 0) && isCrouched))
            {
                speedBeforeUnCrouching = rb.velocity;
                unCrouch();
            }

            if (Input.GetAxisRaw("Horizontal") == 1)
            {
                rotateClockwise();
            }
            if (Input.GetAxisRaw("Horizontal") == -1)
            {
                rotateAntiClockwise();
            }
        }
        if (isUncrouching)
        {
            unCrouch();
        }
        if (isCrouched)
        {
            rb.AddForce(-lastSpeed.normalized * crouchingForce, ForceMode2D.Force);
            rb.AddForce(rb.velocity.normalized * crouchingForce, ForceMode2D.Force);
        }

        //Making sure the skier always has a minimum positive x axis speed
        if(rb.velocity.magnitude < minimumSpeed) 
        {
            rb.velocity = Vector2.right * (minimumSpeed + 0.1f);
        }

        //In case the skier dies
        if (isDead)
        {
            if (isAI)
            {
                gameObject.SetActive(false);
            }
            else
            {
                SceneManager.LoadScene("DeathScreen");
            }
        }
        
        lastSpeed = rb.velocity;
    }

    //Making the skier jump
    public void jump()
    {
        if (isGrounded && hitGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            transform.position += new Vector3(0.1f, 0.1f, 0f);
            hitGround = false;
            isGrounded = false;
        }
    }

    //Making the skier crouch
    public void crouch()
    {
        if (!isCrouched && !isUncrouching)
        {
            rb.AddForce(rb.velocity.normalized * crouchingForce, ForceMode2D.Force);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.6f, 0f);
            float radius = Mathf.Abs((head.position - feet.position).magnitude) / 2;
            currentRotationSpeed = (Mathf.Pow(initialRadius, 2) / Mathf.Pow(radius, 2)) * rotationSpeed;
            isCrouched = true;
        }
    }

    //Making the skier unCrouch
    public void unCrouch()
    {
        isUncrouching = true;
        if(transform.localScale.y < initialScale.y)
        {
            transform.localScale += new Vector3(0f, unCrouchingSpeed * Time.fixedDeltaTime * initialScale.y, 0f);
        }
        if (transform.localScale.y >= initialScale.y)
        {
            currentRotationSpeed = rotationSpeed;
            transform.localScale = initialScale;
            isUncrouching = false;
            isCrouched = false;
            if (isGrounded)
            {
                rb.velocity = speedBeforeUnCrouching;
                jump();
            }
            rb.AddForce(-1 * rb.velocity.normalized * crouchingForce, ForceMode2D.Force);
        }
    }

    //Making the skier turn clockWise according to the predefined rotation speed
    public void rotateClockwise()
    {
        if (!isGrounded)
        {
            transform.rotation = eulerToQuaternion(transform.rotation.eulerAngles.z - currentRotationSpeed);
        }
    }

    //Making the skier turn antiClockWise according to the predefined rotation speed
    public void rotateAntiClockwise()
    {
        if (!isGrounded)
        {
            transform.rotation = eulerToQuaternion(transform.rotation.eulerAngles.z + currentRotationSpeed);
        }
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