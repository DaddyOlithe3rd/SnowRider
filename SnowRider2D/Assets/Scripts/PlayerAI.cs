using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    private Rigidbody2D rb;
    //private float MoveSpeed;
    //private float MoveHorizontal;
    [SerializeField] float obstacleRayDistance;
    

    bool closeToWall = false;
    bool closeToRock = false;
    bool jumpOn;

    public GameObject obstacleRayFeet;
    public GameObject obstacleRayFrontHead;
    public GameObject obstacleRayBackHead;
    public GameObject groundRayObject;

    public LayerMask obstacleLayer;


    void Start()
    {
        //lM = LayerMask.NameToLayer("Ground");
        jumpOn = false;
        rb = gameObject.GetComponent<Rigidbody2D>();
        //rb.AddForce(new Vector2(0.6f, 0f), ForceMode2D.Impulse);

    }

    void Update()
    {

        if (closeToRock == true && jumpOn == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, 5f);
            jumpOn = false;
        }
        else if (closeToWall == true && jumpOn == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, 8.5f);
            jumpOn = false;
        }
        else
        {
            rb.velocity = new Vector2(3f, rb.velocity.y);
        }


    }
    bool OnCollisionEnter2D(Collision2D other)
    {
        if (obstacleLayer == (obstacleLayer | (1 << other.gameObject.layer)))
        {
            return true;
        }
        else return false;
    }

    private void FixedUpdate()
    {


        RaycastHit2D hitFeet = Physics2D.Raycast(obstacleRayFeet.transform.position, Vector2.right, obstacleRayDistance * 0.6f);
        RaycastHit2D hitFront = Physics2D.Raycast(obstacleRayFrontHead.transform.position, new Vector2(0.8f, 0.4f), obstacleRayDistance);
        RaycastHit2D hitBack = Physics2D.Raycast(obstacleRayBackHead.transform.position, new Vector2(0.2f, 0.5f), obstacleRayDistance);
        RaycastHit2D hitGround = Physics2D.Raycast(groundRayObject.transform.position, -Vector2.up);

        Debug.DrawRay(groundRayObject.transform.position, -Vector2.up * hitGround.distance, Color.red);
        Debug.DrawRay(obstacleRayBackHead.transform.position, new Vector2(0.2f, 0.5f) * obstacleRayDistance, Color.green);

        if (hitGround.collider != null)
        {
            if (hitGround.distance <= 0.03f)
            {
                jumpOn = true;
            }
            else
            {
                jumpOn = false;
            }
        }


        if (hitFeet.collider != null && hitFront.collider != null && jumpOn == true)
        {
            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.red);
            Debug.DrawRay(obstacleRayFrontHead.transform.position, new Vector2(0.8f, 0.4f) * obstacleRayDistance, Color.red);
            closeToWall = true;
        }
        else if (hitFeet.collider != null && hitFront.collider == null && jumpOn == true)
        {
            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.red);
            closeToRock = true;
        }
        else if (hitFront.collider != null && hitFeet.collider == null)
        {
            Debug.DrawRay(obstacleRayFrontHead.transform.position, new Vector2(0.8f, 0.4f) * obstacleRayDistance, Color.blue);
            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.green);

            transform.localScale = new Vector3(1f, 0.5f, 1f);
        }

        else
        {
            if (hitBack.collider == null) transform.localScale = new Vector3(1f, 1f, 1f);
            closeToWall = false;
            closeToRock = false;
            Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.green);
            Debug.DrawRay(obstacleRayFrontHead.transform.position, new Vector2(0.8f, 0.4f) * obstacleRayDistance, Color.green);
        }


    }
}
