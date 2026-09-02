using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [SerializeField]private float runSpeed= 5.0f;





    [SerializeField] private InputActionAsset inputActions;
    InputAction moveAction;
    public Vector2 MoveIntput;
    Rigidbody2D playerCharacter;
    public Vector2 MoveInput { get; private set; }

    void Awake()


    {
        playerCharacter = GetComponent<Rigidbody2D>();



        //initalizas content befor the game begins
        InputActionMap playerMap = inputActions.FindActionMap("Player",true);

        moveAction = playerMap.FindAction("Move",true);
       
        playerMap.Enable();




    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveIntput = moveAction.ReadValue<Vector2>();

        run();
    }
    private void run()
    {
        float hMovement = MoveIntput.x;

        playerCharacter.linearVelocity = new Vector2(hMovement * runSpeed,playerCharacter.linearVelocity.y);
    }
}
