using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class JoystickReposition : MonoBehaviour
{
    private RectTransform outerJoystick;
    [SerializeField] private InputActionReference pointerActionRef;
    [SerializeField] private InputActionReference pressActionRef;

    private Vector2 defaultPos;

    private void OnEnable()
    {
        pointerActionRef.action.Enable();
        pressActionRef.action.Enable();

        pressActionRef.action.performed += OnPress;
        pressActionRef.action.canceled += OnRelease;
    }

    private void Awake()
    {
        outerJoystick = GetComponent<RectTransform>();

        defaultPos = outerJoystick.anchoredPosition;
    }

    private void OnDisable()
    {
        pressActionRef.action.performed -= OnPress;
        pressActionRef.action.canceled -= OnRelease;

        pointerActionRef.action.Disable();
        pressActionRef.action.Disable();
    }

    void OnPress(InputAction.CallbackContext _)
    {
        Vector2 screenPos = pointerActionRef.action.ReadValue<Vector2>();
        MoveOuterToScreen(screenPos);
    }

    void OnRelease(InputAction.CallbackContext _)
    {
        outerJoystick.anchoredPosition = defaultPos;
    }

    private void MoveOuterToScreen(Vector2 screenPos)
    {
        var canvasRectTransform = (RectTransform)outerJoystick.parent;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos, null, out var local))
        {
            outerJoystick.anchoredPosition = local;
        }
    }
}
