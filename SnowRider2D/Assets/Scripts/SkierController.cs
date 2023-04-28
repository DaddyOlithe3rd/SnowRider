using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class SkierController : MonoBehaviour
{
    // Start is called before the first frame update
    Vector2 normal = Vector2.up;
    float angle;//Angle entre la normal de la collision et Vector2.right

    Rigidbody2D rb;
    public  CapsuleCollider2D capsuleCollider;

    public bool isGrounded;
    public bool isCrouched;
    public bool isUncrouching;
    public bool isAI;
    public bool isDead;
    public float rotationSpeed;
    public float jumpSpeed;
    public float minimumSpeed;
    public float unCrouchingSpeed;//Speed at which the skier uncrouches
    public float crouchedDrag;//Speed incrementation when crouching
    public float standingDrag;
    public float maxRaycastDistance;

    private Vector3 bottomPoint;
    public Vector2 lastNorm;
    public Vector3 initialScale;
    public Vector2 lastSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        isGrounded = false;
        isCrouched = false;
        isUncrouching = false;
        isDead = false;
        lastNorm = Vector2.zero;
        initialScale = transform.localScale;
        lastSpeed = rb.velocity;

        //Trouver le point bottomPoint de la capsule
        Vector3 center = transform.position;
        float halfHeight = capsuleCollider.size.y / 2;
        float x = center.x + halfHeight * Mathf.Sin((transform.eulerAngles.z * Mathf.PI) / 180);
        float y = center.y - halfHeight * Mathf.Cos((transform.eulerAngles.z * Mathf.PI) / 180);
        bottomPoint = new Vector3(x, y, 0f);
        //rb.drag = standingDrag;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Trouver le point bottomPoint de la capsule
        Vector3 center = transform.position;
        float halfHeight = capsuleCollider.size.y / 2;
        float x = center.x + halfHeight * Mathf.Sin((transform.eulerAngles.z * Mathf.PI) / 180);
        float y = center.y - halfHeight * Mathf.Cos((transform.eulerAngles.z * Mathf.PI) / 180);
        bottomPoint = new Vector3(x, y, 0f);

        ContactPoint2D[] contacts = new ContactPoint2D[10];
        int nbContacts = rb.GetContacts(contacts);

        Debug.DrawRay(bottomPoint, -1 * normal.normalized * maxRaycastDistance, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(bottomPoint, -1 * normal, maxRaycastDistance);
        if (nbContacts == 0 && !hit)
        {
            isGrounded = false;
        }
        else if (nbContacts != 0)
        {
            isGrounded = true;
            if(nbContacts != 0)
            {
                normal = contacts[nbContacts - 1].normal;
            }
            else if (hit)
            {
                normal = hit.normal;
            }
            

            //Si on est encore sur la même pente
            if (lastNorm != normal)
            {
                angle = (Vector2.SignedAngle(Vector2.up, normal));
                transform.RotateAround(bottomPoint, Vector3.forward, (angle - transform.eulerAngles.z));
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
                lastSpeed = rb.velocity;
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

        //Constant speed
        if(rb.velocity.magnitude < minimumSpeed) 
        {
            rb.velocity = Vector2.right * (minimumSpeed + 0.1f);
        }
    }
    public void jump()
    {
        if (isGrounded)
        {
            rb.velocity = rb.velocity + Vector2.up * jumpSpeed;
            isGrounded = false;
        }
    }

    public void crouch()
    {
        if (!isCrouched)
        {
            //rb.drag = crouchedDrag;
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.6f, 0f);
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
            transform.localScale = initialScale;
            isUncrouching = false;
            isCrouched = false;
            if (isGrounded)
            {
                rb.velocity = lastSpeed + Vector2.up * jumpSpeed;
            }
            //rb.drag = standingDrag;
        }
    }

    public void rotateClockwise()
    {
        if (!isGrounded)
        {
            transform.Rotate(0f, 0f, -rotationSpeed);
        }
    }

    public void rotateAntiClockwise()
    {
        if (!isGrounded)
        {
            transform.Rotate(0f, 0f, rotationSpeed);
        }
    }
    public bool getDeath()
    {
        return isDead;
    }

}
