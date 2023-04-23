using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.XR;

public class SkierController : MonoBehaviour
{
    // Start is called before the first frame update
    Vector2 normal = Vector2.up;
    float angle;//Angle entre la normal de la collision et Vector2.right

    Rigidbody2D rb;
    public  CapsuleCollider2D capsuleCollider;

    public bool isGrounded;
    public bool isCrouched;
    public float jumpSpeed;

    private Vector3 bottomPoint;
    public Vector2 lastNorm;
    public Vector3 initialSize;
    public Vector2 lastSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        isGrounded = false;
        isCrouched = false;
        lastNorm = Vector2.zero;
        initialSize = transform.localScale;
        lastSpeed = rb.velocity;

        //Trouver le point bottomPoint de la capsule
        Vector3 center = transform.position;
        float halfHeight = capsuleCollider.size.y / 2;
        float x = center.x + halfHeight * Mathf.Sin((transform.eulerAngles.z * Mathf.PI) / 180);
        float y = center.y - halfHeight * Mathf.Cos((transform.eulerAngles.z * Mathf.PI) / 180);
        bottomPoint = new Vector3(x, y, 0f);

        print("2 BottomPoint: " + bottomPoint);
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
        if (nbContacts == 0) isGrounded = false;
        else 
        {
            isGrounded = true;
            normal = contacts[nbContacts - 1].normal;

            //Si on est encore sur la même pente
            if (lastNorm != normal)
            {
                angle = (Vector2.SignedAngle(Vector2.up, normal));
                //transform.Rotate(0f, 0f, angle - transform.eulerAngles.z);
                transform.RotateAround(bottomPoint, Vector3.forward, (angle - transform.eulerAngles.z));
                print("New norm");
                //Vector3 contactPoint = new Vector3(contacts[0].point.x, contacts[0].point.y, 0f);
                //Quaternion rotation = Quaternion.LookRotation(Vector3.forward, normal);
                //transform.rotation = rotation;

                lastNorm = normal;
            }
        }

        //Jumping
        if (Input.GetAxisRaw("Vertical") == 1 && isGrounded)
        {
            rb.velocity += Vector2.up * jumpSpeed;
        }
        //Crouching
        if (Input.GetAxisRaw("Vertical") == -1 && !isCrouched)
        {
            lastSpeed = rb.velocity;
            if (isGrounded)
            {
                rb.velocity += rb.velocity.normalized * rb.gravityScale;
            }
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.6f, 0f);
            isCrouched = true;
        }
        if((Input.GetKeyUp(KeyCode.DownArrow) || Input.GetAxisRaw("Vertical") == 0) && isCrouched)
        {
            transform.localScale = initialSize;
            rb.velocity = lastSpeed;
            isCrouched = false;
        }
        //Rotating
        if (Input.GetAxisRaw("Horizontal") == 1 && !isGrounded)
        {
            transform.Rotate(0f, 0f, -1.5f);
        }
        if (Input.GetAxisRaw("Horizontal") == -1 && !isGrounded)
        {
            transform.Rotate(0f, 0f, 1.5f);
        }
        //Constant speed
        if(rb.velocity.magnitude < 3f) 
        {
            rb.velocity = Vector2.right * 3.1f;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Vector2 averageNormal = Vector2.zero;
        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    averageNormal += contact.normal;
        //}
        //averageNormal /= collision.contacts.Length;
        //normal = averageNormal;
    }
}
