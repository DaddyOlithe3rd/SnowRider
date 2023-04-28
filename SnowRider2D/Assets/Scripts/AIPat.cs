using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPat : MonoBehaviour
{
    float moveInput;
    [SerializeField] float speed;
    [SerializeField] float jumpforce;
    Rigidbody2D rb;

    public GameObject groundRayObject;
    public GameObject obstacleRayObject;
    public GameObject obstacleFeetRayObject;
    public GameObject obstacleUncrouchRayObject;
    public GameObject obstacleUncrouchFrontRayObject;
    public Vector3 scaleChangetoCrouch;
    public Vector3 scaleChangetoStand;

    float characterDirection;
    float obstacleRayDistance; //formula for distance that we want the AI to jump from
    float obstacleFeetRayDistance; //formula for distance that we want the AI to jump from
    float obstacleUncrouchRayDistance;
    float obstacleUncrouchFrontRayDistance;

    bool JumpOn;

    public LayerMask layerMask;


    void Start()
    {
        JumpOn = false;
        rb = GetComponent<Rigidbody2D>();
        characterDirection = 0f;
        //obstacleRayDistance = Mathf.Sqrt((rb.velocity.x)* (rb.velocity.x) + (rb.velocity.y)* (rb.velocity.y))*100;
        //obstacleFeetRayDistance = Mathf.Sqrt((rb.velocity.x) * (rb.velocity.x) + (rb.velocity.y) * (rb.velocity.y))*100;
        obstacleRayDistance = 10;
        obstacleFeetRayDistance = 10;
        obstacleUncrouchRayDistance = 0.85f;
        obstacleUncrouchFrontRayDistance = 0.85f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 down = transform.TransformDirection(-Vector3.up);
        RaycastHit2D hitGround = Physics2D.Raycast(groundRayObject.transform.position, down);
        Debug.DrawRay(groundRayObject.transform.position, down * hitGround.distance, Color.green);


        if (hitGround.collider != null)
        {
            if (hitGround.distance <= 1.1)
            {
                JumpOn = true;
            }
            else
            {
                JumpOn = false;
            }
        }
    }

    private void Update()
    {
        SkierController controller = GetComponentInParent<SkierController>();
        characterDirection = 1f;
  
        RaycastHit2D hitObstacleHead = Physics2D.Raycast(obstacleRayObject.transform.position, Vector2.right * new Vector2(characterDirection, 0f), obstacleRayDistance, layerMask);
        RaycastHit2D hitObstacleFeet = Physics2D.Raycast(obstacleFeetRayObject.transform.position, Vector2.right * new Vector2(characterDirection, 0f), obstacleFeetRayDistance, layerMask);
        RaycastHit2D hitObstacleUncrouch = Physics2D.Raycast(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), obstacleUncrouchRayDistance, layerMask);
        RaycastHit2D hitObstacleUncrouchFront = Physics2D.Raycast(obstacleUncrouchFrontRayObject.transform.position, Vector2.right * new Vector2(characterDirection, 0f), obstacleRayDistance+1.3f, layerMask);

        Debug.DrawRay(obstacleRayObject.transform.position, Vector2.right * obstacleRayDistance * new Vector2(characterDirection, 0f), Color.green);
        Debug.DrawRay(obstacleFeetRayObject.transform.position, Vector2.right * obstacleRayDistance * new Vector2(characterDirection, 0f), Color.green);
        Debug.DrawRay(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), Color.green);
        Debug.DrawRay(obstacleUncrouchFrontRayObject.transform.position, Vector2.right * (obstacleRayDistance + 1.3f), Color.green);
        if (hitObstacleHead.collider != null)
        {
            print("hi");
            Debug.DrawRay(obstacleRayObject.transform.position, Vector2.right * hitObstacleHead.distance * new Vector2(characterDirection, 0f), Color.red);
            //transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.6f, 0f);
            controller.crouch();
        }
        else if (hitObstacleHead.collider == null && hitObstacleUncrouch.collider == null && hitObstacleUncrouchFront.collider == null)
        {
            controller.unCrouch();
        }
        else if (hitObstacleFeet.collider != null)
        {
            Debug.DrawRay(obstacleFeetRayObject.transform.position, Vector2.right * hitObstacleFeet.distance * new Vector2(characterDirection, 0f), Color.red);
            controller.jump();
            print("jump");
        }
        //if (hitObstacleHead.collider == null && hitObstacleUncrouch.collider == null && hitObstacleUncrouchFront.collider == null)
        //{
        //    transform.localScale = scaleChangetoStand;
        //}
        if (hitObstacleUncrouch.collider != null)
        {
            Debug.DrawRay(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), Color.red);
        }
        if (hitObstacleUncrouchFront.collider != null)
        {
            Debug.DrawRay(obstacleUncrouchFrontRayObject.transform.position, Vector2.right * Vector2.right * (obstacleRayDistance + 1.3f), Color.red);
        }
    }
}
