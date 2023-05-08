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


    private int mask1;
    private int mask2;

    //bool closeToWall;
    private bool closeToRock;
    private bool isGrounded;
    private bool isCrouched;
    private bool canRotate;

    //Les 4 Raycasts
    public GameObject obstacleRayFeet;
    public GameObject obstacleRayFrontHead;
    public GameObject obstacleRayBackHead;
    public GameObject groundRayObject;

    public SkierController controller;

    private Vector3 initialSize;


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();
        isGrounded = false;
        closeToRock = false;
        canRotate = false;
        //closeToWall = false;
        initialSize = transform.localScale;

        mask1 = 1 << LayerMask.NameToLayer("Ground");
        mask2 = 1 << LayerMask.NameToLayer("Obstacle");
        controller = GetComponentInParent<SkierController>();

    }

    void Update()
    {

        if (closeToRock)
        {
            controller.jump();
            closeToRock = false;
        }

        if (isCrouched) controller.crouch();

        else if (!isCrouched && transform.localScale != initialSize) controller.unCrouch();

        //Quand l'AI est dans les airs, il tourne pour faire un backflip.
        if (canRotate)
        {
            controller.rotateAntiClockwise();
        }
        //else transform.Rotate(0f, 0f, 0f);
    }
    private void FixedUpdate()
    {


        RaycastHit2D hitFeet = Physics2D.Raycast(obstacleRayFeet.transform.position, Vector2.right, obstacleRayDistance * 0.6f, mask2);
        RaycastHit2D hitFront = Physics2D.Raycast(obstacleRayFrontHead.transform.position, new Vector2(0.3f, 0.3f), obstacleRayDistance, mask2);
        RaycastHit2D hitBack = Physics2D.Raycast(obstacleRayBackHead.transform.position, new Vector2(0.15f, 0.4f), obstacleRayDistance, mask2);
        RaycastHit2D hitGround = Physics2D.Raycast(groundRayObject.transform.position, Vector2.down, 8f, mask1);


        Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.green);
        Debug.DrawRay(obstacleRayFrontHead.transform.position, new Vector2(0.3f, 0.3f) * obstacleRayDistance, Color.green);
        Debug.DrawRay(obstacleRayBackHead.transform.position, new Vector2(0.15f, 0.4f) * obstacleRayDistance, Color.green);
        Debug.DrawRay(groundRayObject.transform.position, Vector2.down * 8f, Color.green);


        if (hitGround.collider != null)
        {
            canRotate = false;
            Debug.DrawRay(groundRayObject.transform.position, Vector2.down * 6f, Color.red);

            if (hitGround.distance <= 0.5f)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        if (hitGround.collider == null)
        {
            canRotate = true;
            Debug.DrawRay(groundRayObject.transform.position, Vector2.down * 6f, Color.black);
        }


        //S'il y a une roche devant l'AI.
        if (hitFeet.collider != null && hitFront.collider == null && isGrounded)
        {
            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.red);

            closeToRock = true;
        }

        //S'il y a un arbre devant l'AI, son scale devient plus petit, comme s'il "s'accroupissait".
        else if (hitFront.collider != null && hitFeet.collider == null) isCrouched = true;

        else if (hitBack.collider == null) isCrouched = false;


    }
}

