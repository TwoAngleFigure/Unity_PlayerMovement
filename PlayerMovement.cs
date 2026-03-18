using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody playerRigi;

    [Header("Input Action")]
    private PlayerFieldControl actions;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    [Header("Animation")]
    [SerializeField] Animator animator;

    [Header("Movement")]
    [SerializeField] Vector2 moveVactor;
    [SerializeField] float moveAcceleration = 50f;
    [SerializeField] float warkMaxSpeed = 2f;
    [SerializeField] float sprintMaxSpeed = 5f;
    [SerializeField] float rotationSpeed = 720f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] bool isGround = true;

    [SerializeField] Transform cameraTransform;

    private void Awake()
    {
        if (playerRigi == null)
            playerRigi = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponent<Animator>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        actions = new PlayerFieldControl();
        moveAction = actions.Player.Movement;
        jumpAction = actions.Player.Jump;
        sprintAction = actions.Player.Sprint;
    }

    public void OnEnable()
    {
        actions.Enable();
        jumpAction.performed += OnJump;
    }

    public void OnDisable()
    {
        jumpAction.performed -= OnJump;
        actions.Disable();
    }

    void FixedUpdate()
    {
        Vector2 _input = moveAction.ReadValue<Vector2>();

        Vector3 _camForward = cameraTransform.forward;
        Vector3 _camRight = cameraTransform.right;
        _camForward.y = 0;
        _camRight.y = 0;
        _camForward.Normalize();
        _camRight.Normalize();

        Vector3 _moveDir = (_camForward * _input.y) + (_camRight * _input.x);
        moveVactor = new Vector2(_input.x, _input.y) * moveAcceleration;

        float _currentMaxSpeed = sprintAction.IsPressed() ? sprintMaxSpeed : warkMaxSpeed;
        float _playerSpeed = playerRigi.linearVelocity.magnitude;
        animator.SetFloat("Speed", _playerSpeed);

        if (_moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion _targetRotation = Quaternion.LookRotation(_moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotationSpeed * Time.fixedDeltaTime);

            if (_playerSpeed < _currentMaxSpeed)
            {
                playerRigi.AddForce(_moveDir * moveAcceleration, ForceMode.Acceleration);
            }
        }
    }

    void OnJump(InputAction.CallbackContext _context)
    {
        if (isGround)
        {
            playerRigi.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("isJump", true);
        }
    }

    void OnTriggerStay(Collider _other)
    {
        if (_other.CompareTag("Floor"))
        {
            isGround = true;
        }
    }

    void OnTriggerEnter(Collider _other)
    {
        if (_other.CompareTag("Floor"))
        {
            animator.SetBool("isJump", false);
        }
    }

    void OnTriggerExit(Collider _other)
    {
        if (_other.CompareTag("Floor"))
            isGround = false;
    }
}