using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;

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
            glasses[i].SetValues(glassQuantities[i].GetUIValues());
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

        playerMovement.playerAnimator.SetBool("IsMoving", move.magnitude > 0.0f);

        move = Vector2.SmoothDamp(currentMove, move, ref moveVelocity, 0.1f);
        currentMove = move;

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
        if (_forward)
        {
            LiquidQuantity tempLiq = glassQuantities[0];
            glassQuantities[0] = glassQuantities[2];
            glassQuantities[2] = glassQuantities[1];
            glassQuantities[1] = tempLiq;
        }
        else
        {
            LiquidQuantity tempLiq = glassQuantities[0];
            glassQuantities[0] = glassQuantities[1];
            glassQuantities[1] = glassQuantities[2];
            glassQuantities[2] = tempLiq;
        }
    }
}
