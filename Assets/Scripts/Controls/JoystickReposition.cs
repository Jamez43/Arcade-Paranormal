using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class JoystickReposition : MonoBehaviour
{
    private RectTransform outerJoystick;
    private CanvasGroup canvasGroup;
    [SerializeField] private InputActionReference pointerActionRef;
    [SerializeField] private InputActionReference pressActionRef;

    private Vector2 defaultPos;

    private void OnEnable()
    {
        pointerActionRef.action.Enable();
        pressActionRef.action.Enable();

        pressActionRef.action.performed += OnPress;
        pressActionRef.action.canceled += OnRelease;

        SetJoystickVisible(false);
    }

    private void Awake()
    {
        outerJoystick = GetComponent<RectTransform>();
        canvasGroup = outerJoystick.parent.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = outerJoystick.parent.gameObject.AddComponent<CanvasGroup>();
        }

        defaultPos = outerJoystick.anchoredPosition;
        SetJoystickVisible(false);
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
        SetJoystickVisible(true);
    }

    void OnRelease(InputAction.CallbackContext _)
    {
        outerJoystick.anchoredPosition = defaultPos;
        SetJoystickVisible(false);
    }

    private void MoveOuterToScreen(Vector2 screenPos)
    {
        var canvasRectTransform = (RectTransform)outerJoystick.parent;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos, null, out var local))
        {
            outerJoystick.anchoredPosition = local;
        }
    }

    private void SetJoystickVisible(bool isVisible)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = isVisible ? 1f : 0f;
    }
}
