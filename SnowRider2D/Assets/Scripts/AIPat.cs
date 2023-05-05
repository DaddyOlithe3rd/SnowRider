using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPat : MonoBehaviour
{
    float moveInput;
    [SerializeField] float speed;
    [SerializeField] float jumpforce;
    Rigidbody2D rb;

    public GameObject closetoground;
    public GameObject obstacleRayObject;
    public GameObject obstacleFeetRayObject;
    public GameObject obstacleUncrouchRayObject;
    public GameObject obstacleUncrouchFrontRayObject;
    public Vector3 scaleChangetoCrouch;
    public Vector3 scaleChangetoStand;

    float obstacleRayDistance; //formula for distance that we want the AI to jump from
    float obstacleFeetRayDistance; //formula for distance that we want the AI to jump from
    float obstacleUncrouchRayDistance;

    public LayerMask layerMask;
    public LayerMask layerground;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        obstacleUncrouchRayDistance = 0.85f;
    }

    void FixedUpdate()
    {
    }

    private void Update()
    {
        //print(rb.velocity.magnitude);

        SkierController controller = GetComponentInParent<SkierController>();

        //this is the distance of the raycast that will be at the feet and the one thats going to be at the head
        obstacleRayDistance = obstacleFeetRayDistance = rb.velocity.magnitude/2;

        //Raycast to see if the player is close to the ground (used for seeing when to rotate)
        RaycastHit2D hitclosetoground = Physics2D.Raycast(closetoground.transform.position, new Vector2(0, -1), transform.localScale.x * 2, layerground);
        Debug.DrawRay(closetoground.transform.position, transform.localScale.x * 2 * new Vector2(0, -1), Color.blue);

        //Raycast at the level of the head (used to detect when to crouch)
        RaycastHit2D hitObstacleHead = Physics2D.Raycast(obstacleRayObject.transform.position, rb.velocity.normalized, obstacleRayDistance, layerMask);
        Debug.DrawRay(obstacleRayObject.transform.position, obstacleRayDistance * rb.velocity.normalized, Color.green);

        //Raycast at the level of the feet (used to detect when to jump)
        RaycastHit2D hitObstacleFeet = Physics2D.Raycast(obstacleFeetRayObject.transform.position, rb.velocity.normalized, obstacleFeetRayDistance, layerMask);
        Debug.DrawRay(obstacleFeetRayObject.transform.position, obstacleFeetRayDistance * rb.velocity.normalized, Color.green);

        //Raycast higher than the head (used to detect if an object is still above the AI when he is crouched)
        RaycastHit2D hitObstacleUncrouch = Physics2D.Raycast(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), obstacleUncrouchRayDistance, layerMask);
        Debug.DrawRay(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), Color.green);

        //Raycast higher than the head (used to detect if an object is still above the AI when he is crouched)
        RaycastHit2D hitObstacleUncrouchFront = Physics2D.Raycast(obstacleUncrouchFrontRayObject.transform.position, rb.velocity.normalized, obstacleRayDistance + 1.3f, layerMask);
        Debug.DrawRay(obstacleUncrouchFrontRayObject.transform.position, (obstacleRayDistance + 1.3f) * rb.velocity.normalized, Color.green);

        if (rb.velocity.magnitude >= 30)
        {
            if (controller.isCrouched == true)
            {
                controller.unCrouch();
            }
        }
        else if (rb.velocity.magnitude <30)
        {
            controller.crouch();
        }



        if (hitObstacleHead.collider != null)
        {
            Debug.DrawRay(obstacleRayObject.transform.position, obstacleRayDistance * rb.velocity.normalized, Color.red);
            controller.crouch();
        }
        else if (hitObstacleHead.collider == null && hitObstacleUncrouch.collider == null && hitObstacleUncrouchFront.collider == null)
        {
            if(controller.isCrouched == true && rb.velocity.magnitude >=30)
            {
                controller.unCrouch();
            }
        }
        else if (hitObstacleFeet.collider != null)
        {
            Debug.DrawRay(obstacleFeetRayObject.transform.position, obstacleFeetRayDistance * rb.velocity.normalized, Color.red);
            controller.jump();
        }


        //color of the uncrouch rays
        if (hitObstacleUncrouch.collider != null)
        {
            Debug.DrawRay(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), Color.red);
        }
        if (hitObstacleUncrouchFront.collider != null)
        {
            Debug.DrawRay(obstacleUncrouchFrontRayObject.transform.position, rb.velocity.normalized * (obstacleRayDistance + 1.3f), Color.red);
        }
        

        //rotation
        if (transform.eulerAngles != new Vector3(0,0,0) && hitclosetoground.collider == null)
        {
            print("should rotate");
            if ((transform.eulerAngles.z > 0 && transform.eulerAngles.z <= 180) || (transform.eulerAngles.z < -180 && transform.eulerAngles.z > -360))
            {
                controller.rotateClockwise();
            }
            else if ((transform.eulerAngles.z > 180 && transform.eulerAngles.z < 360)||(transform.eulerAngles.z < 0 && transform.eulerAngles.z > -180))
            {
                controller.rotateAntiClockwise();
            }
        }
    }
}
