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
        //obstacleRayDistance = Mathf.Sqrt((rb.velocity.x)* (rb.velocity.x) + (rb.velocity.y)* (rb.velocity.y))*100;
        //obstacleFeetRayDistance = Mathf.Sqrt((rb.velocity.x) * (rb.velocity.x) + (rb.velocity.y) * (rb.velocity.y))*100;
        
        obstacleUncrouchRayDistance = 0.85f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 down = transform.TransformDirection(-Vector3.up);
    }

    private void Update()
    {
        obstacleRayDistance = obstacleFeetRayDistance = rb.velocity.magnitude/2;
        //print(rb.velocity);
        SkierController controller = GetComponentInParent<SkierController>();

        RaycastHit2D hitclosetoground = Physics2D.Raycast(closetoground.transform.position, new Vector2(0, -1), transform.localScale.x * 2, layerground);
        Debug.DrawRay(closetoground.transform.position, transform.localScale.x * 2 * new Vector2(0, -1), Color.blue);

        RaycastHit2D hitObstacleHead = Physics2D.Raycast(obstacleRayObject.transform.position, rb.velocity.normalized, obstacleRayDistance, layerMask);
        RaycastHit2D hitObstacleFeet = Physics2D.Raycast(obstacleFeetRayObject.transform.position, rb.velocity.normalized, obstacleFeetRayDistance, layerMask);
        RaycastHit2D hitObstacleUncrouch = Physics2D.Raycast(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), obstacleUncrouchRayDistance, layerMask);
        RaycastHit2D hitObstacleUncrouchFront = Physics2D.Raycast(obstacleUncrouchFrontRayObject.transform.position, rb.velocity.normalized, obstacleRayDistance + 1.3f, layerMask);

        Debug.DrawRay(obstacleRayObject.transform.position, obstacleRayDistance * rb.velocity.normalized, Color.green);
        Debug.DrawRay(obstacleFeetRayObject.transform.position, obstacleFeetRayDistance * rb.velocity.normalized, Color.green);
        Debug.DrawRay(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), Color.green);
        Debug.DrawRay(obstacleUncrouchFrontRayObject.transform.position, (obstacleRayDistance + 1.3f) * rb.velocity.normalized, Color.green);
        if (hitObstacleHead.collider != null)
        {
            Debug.DrawRay(obstacleRayObject.transform.position, obstacleRayDistance * rb.velocity.normalized, Color.red);
            //transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.6f, 0f);
            controller.crouch();
        }
        else if (hitObstacleHead.collider == null && hitObstacleUncrouch.collider == null && hitObstacleUncrouchFront.collider == null)
        {
            if(controller.isCrouched == true)
            {
                controller.unCrouch();
            }
        }
        else if (hitObstacleFeet.collider != null)
        {
            Debug.DrawRay(obstacleFeetRayObject.transform.position, obstacleFeetRayDistance * rb.velocity.normalized, Color.red);
            controller.jump();
        }
        if (hitObstacleUncrouch.collider != null)
        {
            Debug.DrawRay(obstacleUncrouchRayObject.transform.position, new Vector2(-1, 1), Color.red);
        }
        if (hitObstacleUncrouchFront.collider != null)
        {
            Debug.DrawRay(obstacleUncrouchFrontRayObject.transform.position, rb.velocity.normalized * (obstacleRayDistance + 1.3f), Color.red);
        }
        
        if (transform.eulerAngles != new Vector3(0,0,0) && hitclosetoground.collider == null)
        {
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
