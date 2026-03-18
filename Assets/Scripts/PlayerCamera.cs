using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;

    [Header("Input Action")]
    private PlayerFieldControl cameraActions;
    private InputAction viewAction;
    private InputAction rightClickAction;
    private InputAction scrollAction;

    [Header("View Control")]
    [SerializeField] float lookSensitivity = 0.5f;
    [SerializeField] float cameraDistance = 5f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float minDistance = 2f;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 1.5f, 0);
    
    public bool isMouseLookActive = false;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        if (target == null)
            target = transform;

        cameraActions = new();
        viewAction = cameraActions.Player.View;
        rightClickAction = cameraActions.Player.RightClick;
        scrollAction = cameraActions.Player.Scroll;
    }

    private void OnEnable()
    {
        cameraActions.Enable();
        rightClickAction.performed += _ => OnToggleMouseLook();
    }

    private void OnDisable()
    {
        rightClickAction.performed -= _ => OnToggleMouseLook();
        cameraActions.Disable();
    }

    private void LateUpdate()
    {
        float _scroll = scrollAction.ReadValue<Vector2>().y;
        if (_scroll != 0)
        {
            cameraDistance -= _scroll * zoomSpeed * Time.deltaTime;
            cameraDistance = Mathf.Clamp(cameraDistance, minDistance, maxDistance);
        }

        if (isMouseLookActive)
        {
            Vector2 _mouseDelta = viewAction.ReadValue<Vector2>() * lookSensitivity;
            yaw += _mouseDelta.x;

            pitch -= _mouseDelta.y;
            pitch = Mathf.Clamp(pitch, -70f, 70f);

            Quaternion _rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 _targetPos = target.position + cameraOffset;
            transform.position = _targetPos + (_rotation * Vector3.back * cameraDistance);
            transform.LookAt(_targetPos);
        }
    }

    private void OnToggleMouseLook()
    {
        isMouseLookActive = !isMouseLookActive;

        if (isMouseLookActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
