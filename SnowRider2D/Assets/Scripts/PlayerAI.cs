using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class PlayerAI : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    [SerializeField] float obstacleRayDistance;


    public float jumpSpeed;
    public float angle;  //Angle entre la normal de la collision et Vector2.right

    public int mask1;
    public int mask2;

    //bool closeToWall;
    public bool closeToRock;
    public bool isGrounded;
    public bool isDead;
    public bool canRotate;

    //Les 4 Raycasts
    public GameObject obstacleRayFeet;
    public GameObject obstacleRayFrontHead;
    public GameObject obstacleRayBackHead;
    public GameObject groundRayObject;

    private Vector3 bottomPoint;
    public Vector3 InitialSize;
    public Vector2 lastNormal;
    public Vector2 normal = Vector2.up;
    public Vector2 lastSpeed;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();
        isGrounded = false;
        closeToRock = false;
        isDead = false;
        canRotate = false;
        lastNormal = Vector2.zero;
        lastSpeed = rb.velocity;
        //closeToWall = false;
        InitialSize = transform.localScale;

        mask1 = 1 << LayerMask.NameToLayer("Ground");
        mask2 = 1 << LayerMask.NameToLayer("Obstacle");
    }

    void Update()
    {
        if (closeToRock)
        {
            rb.velocity += Vector2.up;
        }

        //Avoir une vitesse constante
        if (rb.velocity.magnitude < 3f)
        {
            rb.velocity = Vector2.right * 3.2f;
        }

        //Quand l'AI est dans les airs, il tourne pour faire un backflip.
        if (canRotate)
        {
            transform.Rotate(0f, 0f, 1.2f);
        }
        //else transform.Rotate(0f, 0f, 0f);
    }
    private void FixedUpdate()
    {
        //Trouver le point bottomPoint de la capsule
        Vector3 center = transform.position;
        float halfHeight = capsuleCollider.size.y / 2;
        float x = center.x + halfHeight * Mathf.Sin((transform.eulerAngles.z * Mathf.PI) / 180);
        float y = center.y - halfHeight * Mathf.Cos((transform.eulerAngles.z * Mathf.PI) / 180);
        bottomPoint = new Vector3(x, y, 0f);


        RaycastHit2D hitFeet = Physics2D.Raycast(obstacleRayFeet.transform.position, Vector2.right, obstacleRayDistance * 0.6f, mask2);
        RaycastHit2D hitFront = Physics2D.Raycast(obstacleRayFrontHead.transform.position, new Vector2(0.8f, 0.4f), obstacleRayDistance, mask2);
        RaycastHit2D hitBack = Physics2D.Raycast(obstacleRayBackHead.transform.position, new Vector2(0.2f, 0.5f), obstacleRayDistance, mask2);
        RaycastHit2D hitGround = Physics2D.Raycast(groundRayObject.transform.position, Vector2.down, 6f, mask1);


        if (hitGround.collider != null)
        {
            canRotate = false;
            Debug.DrawRay(groundRayObject.transform.position, Vector2.down * 6f, Color.red);

            if (hitGround.distance <= 0.5f)
            {
                isGrounded = true;
                //Debug.Log("Collide avec dequoi");
            }
            else
            {
                isGrounded = false;
                //Debug.Log("Collide, mais trop loin");
            }
        }
        if (hitGround.collider == null)
        {
            canRotate = true;
            //Debug.Log("Collide PAS");
            Debug.DrawRay(groundRayObject.transform.position, Vector2.down * 6f, Color.black);
        }


        ContactPoint2D[] contacts = new ContactPoint2D[10];
        int nbContacts = rb.GetContacts(contacts);

        if (nbContacts != 0)
        {
            normal = contacts[nbContacts - 1].normal;
            foreach (ContactPoint2D contact in contacts)
            {
                //if (contact.collider.gameObject)
                //{
                //    isDead = true;
                //    print("Dead");
                //}
            }

            //Si on est encore sur la même pente
            if (lastNormal != normal)
            {
                angle = (Vector2.SignedAngle(Vector2.up, normal));
                transform.RotateAround(bottomPoint, Vector3.forward, (angle - transform.eulerAngles.z));

                lastNormal = normal;
            }
        }


        //Si il y a un gros obstacle devant l'AI.
        /*if (hitFeet.collider != null && hitFront.collider != null && isGrounded == true)
        {
            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.red);
            Debug.DrawRay(obstacleRayFrontHead.transform.position, new Vector2(0.8f, 0.4f) * obstacleRayDistance, Color.red);
            closeToWall = true;
        }*/

        //S'il y a une roche devant l'AI.
        if (hitFeet.collider != null && hitFront.collider == null && isGrounded)
        {
            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.red);

            closeToRock = true;
        }

        //S'il y a un arbre devant l'AI, son scale devient plus petit, comme s'il "s'accroupissait".
        else if (hitFront.collider != null && hitFeet.collider == null)
        {
            Debug.DrawRay(obstacleRayFrontHead.transform.position, new Vector2(0.8f, 0.4f) * obstacleRayDistance, Color.blue);
            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.green);

            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.6f, 0f);
        }

        else
        {
            if (hitBack.collider == null) transform.localScale = InitialSize;

            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.green);
            Debug.DrawRay(obstacleRayFrontHead.transform.position, new Vector2(0.8f, 0.4f) * obstacleRayDistance, Color.green);

            Debug.DrawRay(obstacleRayBackHead.transform.position, new Vector2(0.2f, 0.5f) * obstacleRayDistance, Color.green);
        }


    }
}

