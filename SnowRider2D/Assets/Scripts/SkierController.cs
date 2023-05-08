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
    // Start is called before the first frame update
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
    float angle;//Angle entre la normal de la collision et Vector2.right
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
        lastNorm = Vector2.zero;
        initialScale = transform.localScale;
        speedBeforeUnCrouching = rb.velocity;
        currentRotationSpeed = rotationSpeed;
        initialRadius = initialRadius * Mathf.Abs((head.position - feet.position).magnitude) / 2;
        
        //Trouver le point bottomPoint de la capsule
        //Vector3 center = transform.position;
        //float halfHeight = capsuleCollider.size.y / 2;
        //float x = center.x + halfHeight * Mathf.Sin((transform.eulerAngles.z * Mathf.PI) / 180);
        //float y = center.y - halfHeight * Mathf.Cos((transform.eulerAngles.z * Mathf.PI) / 180);
        //bottomPoint = new Vector3(x, y, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Trouver le point bottomPoint de la capsule
        //Vector3 center = transform.position;
        //float halfHeight = capsuleCollider.size.y / 2;
        //float x = center.x + halfHeight * Mathf.Sin((transform.eulerAngles.z * Mathf.PI) / 180);
        //float y = center.y - halfHeight * Mathf.Cos((transform.eulerAngles.z * Mathf.PI) / 180);
        //bottomPoint = new Vector3(x, y, 0f);
        bottomPoint = bottomPointObject.transform.position;


        ContactPoint2D[] arrayContacts = new ContactPoint2D[16];
        int nbContacts = rb.GetContacts(arrayContacts);

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
           
            //Si on est encore sur la même pente
            if (lastNorm != normal )
            {
                angle = (Vector2.SignedAngle(Vector2.up, normal));
;               transform.RotateAround(bottomPoint, Vector3.forward, (angle - transform.eulerAngles.z));
                lastNorm = normal;
            }
        }

        //Jumping
        if (!isAI)
        {
            if (Input.GetAxisRaw("Vertical") == 1)
            {
                jump();
            }
            //Crouching
            if (Input.GetAxisRaw("Vertical") == -1)
            {
                crouch();
            }
            if (((Input.GetKeyUp(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") == 0) && isCrouched))
            {
                speedBeforeUnCrouching = rb.velocity;
                unCrouch();
            }

            //Rotating
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

        //Constant speed
        if(rb.velocity.magnitude < minimumSpeed) 
        {
            rb.velocity = Vector2.right * (minimumSpeed + 0.1f);
        }

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
    public void jump()
    {
        if (isGrounded /*&& hitGround*/)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            hitGround = false;
            isGrounded = false;
        }
    }

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
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            rb.AddForce(-1 * rb.velocity.normalized * crouchingForce, ForceMode2D.Force);
        }
    }

    public void rotateClockwise()
    {
        if (!isGrounded)
        {
            transform.Rotate(0f, 0f, -currentRotationSpeed);
            //print("should rotate clockwise");
        }
    }
    
    public void rotateAntiClockwise()
    {
        if (!isGrounded)
        {
            transform.Rotate(0f, 0f, currentRotationSpeed);
            //print("should rotate anticlockwise");
        }
    }
}
