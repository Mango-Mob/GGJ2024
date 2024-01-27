using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;

    // Smooth damp movement
    private Vector2 currentMove;
    private Vector2 moveVelocity;

    [Header("UI")]
    public JuiceMashUI juiceMashUI;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

        juiceMashUI.ToggleVisibility(playerMovement.playerAnimator.GetBool("IsTickling"));
    }
}
