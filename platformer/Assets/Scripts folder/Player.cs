using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{

    [SerializeField] private InputActionAsset inputActions;
    InputAction moveAction;

    void Awake()
    {
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
        
    }
}
