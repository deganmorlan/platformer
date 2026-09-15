using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header ("move settings")]
    [SerializeField] private float runSpeed = 5.0f;

    [SerializeField] private float runAcceleration = 30f;

    [SerializeField] private float runDeceleration = 40f;

    [SerializeField] private float jumpSpeed = 5.0f;

    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] private LayerMask groundlayer;

    InputAction moveAction;
    InputAction jumpAction;
    public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPressedThisFrame();

    public Vector2 MoveInput { get; private set; }

    Rigidbody2D playerCharacter;

    Animator playerAnimator;

    public LayerMask GroundLayer => groundlayer.value != 0 ? groundlayer : LayerMask.GetMask("ground");

    BoxCollider2D playerfeetcolider;

    // Initializes its contents before the game begins
    void Awake()
    {
        playerCharacter = GetComponent<Rigidbody2D>();

        playerAnimator = GetComponentInChildren<Animator>();

        playerfeetcolider = GetComponent<BoxCollider2D>();

        InputActionMap playerMap = inputActions.FindActionMap("Player", true);

        jumpAction = playerMap.FindAction("Jump", true);

        moveAction = playerMap.FindAction("Move", true);



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

        jump();
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
    }

    private void flipsprite()
    {
        bool hmovement = Mathf.Abs(playerCharacter.linearVelocity.x) > Mathf.Epsilon;

        if (hmovement)
        {
        
            transform.localScale = new Vector2(Mathf.Sign(playerCharacter.linearVelocity.x), 1f);
        
        }


    }

    private void jump()
    {
        bool isgrounded = playerfeetcolider.IsTouchingLayers(GroundLayer);

        if(JumpPressedThisFrame&&isgrounded)
        {
            playerCharacter.linearVelocity = new Vector2(playerCharacter.linearVelocity.x, jumpSpeed);
        }
    }


}