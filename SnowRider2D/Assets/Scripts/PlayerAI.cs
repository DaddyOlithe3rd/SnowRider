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


    [SerializeField] float obstacleRayDistance; //Longueur des raycasts (choisi par le programmeur).



    public float jumpSpeed;


    private int mask1; // Les couches que les raycast regardent.
    private int mask2;

    //Les bool qui deviennent vraies selon la collision des raycasts.
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

    private Vector3 initialSize; //Taille initiale du joueur.


    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();
        isGrounded = false;
        closeToRock = false;
        canRotate = false;
        initialSize = transform.localScale;

        mask1 = 1 << LayerMask.NameToLayer("Ground");
        mask2 = 1 << LayerMask.NameToLayer("Obstacle");
        controller = GetComponentInParent<SkierController>();

    }

    void Update()
    {
        //S'il est proche d'une roche, il saute.
        if (closeToRock)
        {
            controller.jump();
            closeToRock = false;
        }

        //S'il voit un arbre, il s'accroupit.
        if (isCrouched) controller.crouch();

        //S'il n'a plus besoin de s'accroupir, il se relève à sa taille initiale.
        else if (!isCrouched && transform.localScale != initialSize) controller.unCrouch();

        // Quand l'AI est dans les airs, il tourne pour faire un backflip.
        // J'ai décidé d'enlever cette partie du code, car cela posait problème avec la génération de terrain.
        // Au début de la partie, si les joueurs "spawnaient" trop haut dans les airs, mon AI tournait parce qu'il pensait
        // qu'il pouvait faire un backflip.

        /*if (canRotate)
        {
            controller.rotateAntiClockwise();
        }*/
    }
    private void FixedUpdate()
    {

        //Définition des 4 raycasts.
        RaycastHit2D hitFeet = Physics2D.Raycast(obstacleRayFeet.transform.position, Vector2.right, obstacleRayDistance * 0.6f, mask2);
        RaycastHit2D hitFront = Physics2D.Raycast(obstacleRayFrontHead.transform.position, new Vector2(0.3f, 0.3f), obstacleRayDistance, mask2);
        RaycastHit2D hitBack = Physics2D.Raycast(obstacleRayBackHead.transform.position, new Vector2(0.15f, 0.4f), obstacleRayDistance, mask2);
        RaycastHit2D hitGround = Physics2D.Raycast(groundRayObject.transform.position, Vector2.down, 8f, mask1);

        //Sert simplement à voir les raycasts dans l'éditeur.
        Debug.DrawRay(obstacleRayFeet.transform.position, Vector2.right * obstacleRayDistance * 0.6f, Color.green);
        Debug.DrawRay(obstacleRayFrontHead.transform.position, new Vector2(0.3f, 0.3f) * obstacleRayDistance, Color.green);
        Debug.DrawRay(obstacleRayBackHead.transform.position, new Vector2(0.15f, 0.4f) * obstacleRayDistance, Color.green);
        Debug.DrawRay(groundRayObject.transform.position, Vector2.down * 8f, Color.green);

        //Si le raycast du sol touche le sol.
        if (hitGround.collider != null)
        {
            canRotate = false;

            //S'il est assez proche du sol, il peut sauter.
            if (hitGround.distance <= 0.5f)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        //Si le raycast ne touche pas le sol, ça veut dire qu'il peut faire un backflip.
        // Mais j'ai enlevé le code plus haut, donc il ne fera rien.
        if (hitGround.collider == null)
        {
            canRotate = true;
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

