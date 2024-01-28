using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public MultiAudioAgent audioAgent { private set; get; }

    // Smooth damp movement
    private Vector2 currentMove;
    private Vector2 moveVelocity;

    public LiquidQuantity[] glassQuantities = new LiquidQuantity[3]; 

    [Header("UI")]
    [SerializeField] private LiquidProgressControllerUI[] glasses;
    public JuiceMashUI juiceMashUI;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        for (int i = 0; i < 3; i++)
        {
            glassQuantities[i] = new LiquidQuantity();
        }

        audioAgent = GetComponent<MultiAudioAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update glasses
        for (int i = 0; i < 3; i++)
        {
            glasses[i].SetValues(glassQuantities[i]);
        }

        bool gamepadMode = InputManager.Instance.isInGamepadMode;

        Vector2 move = Vector2.zero;
        if (gamepadMode) // Gamepad movement
        {
            move = InputManager.Instance.GetGamepadStick(StickType.LEFT, 0);
        }
        else // Keyboard movement
        {
            move.x += InputManager.Instance.IsKeyPressed(KeyType.D) ? 1.0f : 0.0f;
            move.x += InputManager.Instance.IsKeyPressed(KeyType.A) ? -1.0f : 0.0f;
            move.y += InputManager.Instance.IsKeyPressed(KeyType.W) ? 1.0f : 0.0f;
            move.y += InputManager.Instance.IsKeyPressed(KeyType.S) ? -1.0f : 0.0f;

            move.Normalize();
        }

        if (move.magnitude >= 0.8f)
        {
            playerMovement.ReleaseGrab();
        }

        playerMovement.playerAnimator.SetBool("IsMoving", move.magnitude > 0.0f);

        move = Vector2.SmoothDamp(currentMove, move, ref moveVelocity, 0.1f);
        currentMove = move;

        playerMovement.playerAnimator.SetFloat("RunSpeed", move.magnitude);
        playerMovement.Move(move);

        // Diving control
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.SOUTH, 0) || InputManager.Instance.GetMouseDown(MouseButton.LEFT))
        {
            if (playerMovement.playerAnimator.GetBool("IsTickling"))
            {
                playerMovement.JuiceTarget();
                Fruit grabbedFruit = playerMovement.GetGrabbedFruit();

                if (grabbedFruit)
                    juiceMashUI.SetJuice((int)grabbedFruit.my_type);
                juiceMashUI.SetValue(grabbedFruit ? playerMovement.GetGrabbedFruit().juiceAmount : 0.0f);
            }
            else
            {
                playerMovement.Dive();
            }
        }

        juiceMashUI.ToggleVisibility(playerMovement.playerAnimator.GetBool("IsTickling"), playerMovement.GetGrabbedFruit() ? playerMovement.GetGrabbedFruit().juiceAmount : 0.0f);

        if(GameManager.Instance.canSell && !glassQuantities[1].isEmpty)
        {
            if (InputManager.Instance.IsGamepadButtonDown(ButtonType.LEFT, 0) || InputManager.Instance.IsKeyDown(KeyType.ALP_ONE))
            {
                GameManager.Instance.SellTo( 0, glassQuantities[1]);
                glassQuantities[1] = new LiquidQuantity();
            }
            else if (InputManager.Instance.IsGamepadButtonDown(ButtonType.UP, 0) || InputManager.Instance.IsKeyDown(KeyType.ALP_TWO))
            {
                GameManager.Instance.SellTo(1, glassQuantities[1]);
                glassQuantities[1] = new LiquidQuantity();
            }
            else if (InputManager.Instance.IsGamepadButtonDown(ButtonType.DOWN, 0) || InputManager.Instance.IsKeyDown(KeyType.ALP_THREE))
            {
                GameManager.Instance.SellTo(2, glassQuantities[1]);
                glassQuantities[1] = new LiquidQuantity();
            }
            else if (InputManager.Instance.IsGamepadButtonDown(ButtonType.RIGHT, 0) || InputManager.Instance.IsKeyDown(KeyType.ALP_FOUR))
            {
                GameManager.Instance.SellTo(3, glassQuantities[1]);
                glassQuantities[1] = new LiquidQuantity();
            }
        }
        

        // 
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.RB, 0) || InputManager.Instance.IsKeyDown(KeyType.E))
        {
            ChangeGlass(true);
        }
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.LB, 0) || InputManager.Instance.IsKeyDown(KeyType.Q))
        {
            ChangeGlass(false);
        }

        // Dump glass
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.NORTH, 0) || InputManager.Instance.IsKeyDown(KeyType.F))
        {
            glassQuantities[1] = new LiquidQuantity();
        }
    }

    public void ChangeGlass(bool _forward)
    {
        audioAgent.Play("splash swapping drinks");


        if (_forward)
        {
            LiquidQuantity tempLiq = glassQuantities[1];
            glassQuantities[1] = glassQuantities[2];
            glassQuantities[2] = tempLiq;


            //LiquidQuantity tempLiq = glassQuantities[0];
            //glassQuantities[0] = glassQuantities[2];
            //glassQuantities[2] = glassQuantities[1];
            //glassQuantities[1] = tempLiq;
        }
        else
        {
            LiquidQuantity tempLiq = glassQuantities[1];
            glassQuantities[1] = glassQuantities[0];
            glassQuantities[0] = tempLiq;

            //LiquidQuantity tempLiq = glassQuantities[0];
            //glassQuantities[0] = glassQuantities[1];
            //glassQuantities[1] = glassQuantities[2];
            //glassQuantities[2] = tempLiq;
        }
    }
    public void GetHit()
    {
        playerMovement.Hit();
    }
}
