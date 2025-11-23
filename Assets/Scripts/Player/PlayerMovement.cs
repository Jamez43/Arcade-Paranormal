using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;
    private Vector2 moveAmount;
    private float currentSpeed;
    private Collider2D playerCollider;
    private PlayerRuntimeStats stats;

    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.FindActionMap("Gameplay").Enable();
        }
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.FindActionMap("Gameplay").Disable();
        }
    }

    public void SwitchInputMap(string oldMap, string newMap)
    {
        if (inputActions != null)
        {
            inputActions.FindActionMap(oldMap).Disable();
            inputActions.FindActionMap(newMap).Enable();
        }
    }

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("Input Actions not assigned in PlayerMovement Inspector!");
            return;
        }

        moveAction = inputActions.FindAction("Move");
        if (moveAction == null)
        {
            Debug.LogError("Move action not found in Input Actions!");
            return;
        }

        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogError("Collider2D component not found on Player!");
            return;
        }

        var playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController component not found on Player!");
            return;
        }

        stats = playerController.Stats;
        if (stats == null)
        {
            Debug.LogError("PlayerController.Stats is null!");
            return;
        }

        currentSpeed = stats.Speed;
    }

    private void Update()
    {
        if (moveAction == null) return;

        moveAmount = moveAction.ReadValue<Vector2>();
        if (playerCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            currentSpeed = stats.Speed * 0.5f;
        }
        else
        {
            currentSpeed = stats.Speed;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        transform.position += new Vector3(moveAmount.x, moveAmount.y, 0) * currentSpeed * Time.fixedDeltaTime;
    }

}
