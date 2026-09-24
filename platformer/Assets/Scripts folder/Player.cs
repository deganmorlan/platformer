using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    [Header ("move settings")]
    [SerializeField] private float runSpeed = 5.0f;

    [SerializeField] private float runAcceleration = 30f;

    [SerializeField] private float runDeceleration = 40f;
    [Header("Jump Settings")]
    [SerializeField] private float jumpSpeed = 5.0f;
    [SerializeField][Range(0.1f, 1f)] private float  jumpCutMultiplyer = 0.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float fallGravityMulti = 2.0f;

    [SerializeField] private float climbSpeed = 5.0f;

    [SerializeField] private float ladderJumpTime = 0.1f;
    float jumpedOffLadderTimer;
    

    float lastGroundTime;
    float jumpBufferTimer;
    float gravityScaleAtStart;
   

    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] private LayerMask groundlayer;

    LayerMask climbingLayer;

    InputAction moveAction;
    InputAction jumpAction;
    public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPressedThisFrame();

    public Vector2 MoveInput { get; private set; }

    Rigidbody2D playerCharacter;

    Animator playerAnimator;

    public LayerMask GroundLayer => groundlayer.value != 0 ? groundlayer : LayerMask.GetMask("ground");

    BoxCollider2D playerFeetColider;

    CapsuleCollider2D playerBodyCollider;

    // Initializes its contents before the game begins
    void Awake()
    {
        playerBodyCollider = GetComponent<CapsuleCollider2D>();

        playerCharacter = GetComponent<Rigidbody2D>();

        playerAnimator = GetComponentInChildren<Animator>();

        playerFeetColider = GetComponent<BoxCollider2D>();

        gravityScaleAtStart = playerCharacter.gravityScale;

        InputActionMap playerMap = inputActions.FindActionMap("Player", true);

        jumpAction = playerMap.FindAction("Jump", true);

        moveAction = playerMap.FindAction("Move", true);

        climbingLayer = LayerMask.GetMask("Climbing");


        playerMap.Enable();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        flipsprite();

        MoveInput = moveAction.ReadValue<Vector2>();

        Run();

        Jump();
        Climb();
        BetterGravity();
    }

    private void Run()
    {
        float hMovement = MoveInput.x;

        float targetSpeed = MoveInput.x * runSpeed;

        float speedChange;

        if (Mathf.Abs(targetSpeed) > Mathf.Epsilon)
        {
            speedChange = runAcceleration;
        }
        else
        {
            speedChange = runDeceleration;
        }

        float newSpeed = Mathf.MoveTowards(playerCharacter.linearVelocity.x, targetSpeed, speedChange * Time.deltaTime);

        playerCharacter.linearVelocity = new Vector2(newSpeed, playerCharacter.linearVelocity.y);

        bool hSpeed = Mathf.Abs(playerCharacter.linearVelocity.x) > Mathf.Epsilon;

        // playerAnimator.SetBool("run", hSpeed);
       // playerAnimator.SetBool("Run", hSpeed && !playerBodyCollider.IsTouchingLayers(climbingLayer));
    }

    private void flipsprite()
    {
        bool hmovement = Mathf.Abs(playerCharacter.linearVelocity.x) > Mathf.Epsilon;

        if (hmovement)
        {
        
            transform.localScale = new Vector2(Mathf.Sign(playerCharacter.linearVelocity.x), 1f);
        
        }


    }

    private void Jump()
    {
        
        if(jumpAction.WasReleasedThisFrame()&& playerCharacter.linearVelocity.y >0)
        {
            playerCharacter.linearVelocity = new Vector2(playerCharacter.linearVelocity.x, playerCharacter.linearVelocity.y * jumpCutMultiplyer);
        }


        bool isgrounded = playerFeetColider.IsTouchingLayers(GroundLayer) || playerBodyCollider.IsTouchingLayers(climbingLayer);



        if (isgrounded)
        {
            //Remember a breafe window after leaving the ground
            lastGroundTime = coyoteTime;
        }
        else
        {
            lastGroundTime -= Time.deltaTime;
        }

        if (JumpPressedThisFrame)
        {
            //Remember a jump press for landing
            jumpBufferTimer = jumpBufferTime;

        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        if (lastGroundTime <= 0 || jumpBufferTimer <= 0)
        {
            return;
        }
        playerCharacter.linearVelocity = new Vector2(playerCharacter.linearVelocity.x, jumpSpeed);
        lastGroundTime = 0;
        jumpBufferTimer = 0;

        // FORGOT THIS
        jumpedOffLadderTimer = ladderJumpTime;
    }
    private void BetterGravity()
    {
        //Optional turn off function when climbing
        if(playerBodyCollider.IsTouchingLayers(climbingLayer))
        {
            return;
        }

        //use stronger gravity when falling then cap fall speed. 
        float gravityMultiplyer = playerCharacter.linearVelocity.y < 0 ? fallGravityMulti : 1f;

        playerCharacter.gravityScale = gravityScaleAtStart * gravityMultiplyer;

       

        if(playerCharacter.linearVelocity.y < -jumpSpeed * fallGravityMulti)
        {
            playerCharacter.linearVelocity = new Vector2(playerCharacter.linearVelocity.x,-jumpSpeed*fallGravityMulti);
        }
    }



    private void Climb()
    {
        jumpedOffLadderTimer -= Time.deltaTime;

        // ADD THESE
        bool onLadder = playerBodyCollider.IsTouchingLayers(climbingLayer);
        //bool wasClimbing = playerAnimator.GetBool("climb");
        // playerAnimator
        if (jumpedOffLadderTimer > 0 || !onLadder)
        {
            //playerAnimator.SetBool("climb", false);

            playerCharacter.gravityScale = gravityScaleAtStart;

            // No launch off climbing layer top
            //if (!onLadder && wasClimbing && jumpedOffLadderTimer <= 0)
            //{
            //    playerCharacter.linearVelocity = new Vector2(playerCharacter.linearVelocity.x, 0f);
            //}

            return;
        }

        float vMovement = MoveInput.y;

        Vector2 climbingVelocity = new Vector2(MoveInput.x * runSpeed, vMovement * climbSpeed);

        playerCharacter.linearVelocity = climbingVelocity;

        bool vSpeed = Mathf.Abs(playerCharacter.linearVelocity.y) > Mathf.Epsilon;

        // UPDATE THIS
       // playerAnimator.SetBool("climb", vSpeed);

        playerCharacter.gravityScale = 0.0f;
    }






}