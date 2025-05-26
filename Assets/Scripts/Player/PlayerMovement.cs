using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;

    private float _speed;
    private bool _crouching = false;
    private bool _lerpCroucing = false;
    private float _crouchTimer = 0f;
    private bool _sprinting = false;

    [Header("Move Settings")]
    public float baseSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 2.5f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool _isDashing = false;
    private float _dashTimer = 0f;
    private float _dashCooldownTimer = 0f;
    private Vector3 _dashDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        _speed = baseSpeed;
    }

    void Update()
    {
        ProcessCrouch();
        ProcessDash();
    }

    public void Dash(Vector2 input)
    {
        if (_isDashing || _dashCooldownTimer > 0f)
        {
            return;
        }

        _isDashing = true;
        _dashTimer = dashDuration;
        _dashCooldownTimer = dashCooldown;

        Vector3 moveDir = new Vector3(input.x, 0, input.y);

        if (moveDir.sqrMagnitude < 0.01f)
        {
            moveDir = Vector3.forward;
        }

        _dashDirection = transform.TransformDirection(moveDir.normalized);
    }

    private void ProcessDash()
    {
        if (_dashCooldownTimer > 0f)
        {
            _dashCooldownTimer = Mathf.Max(0, _dashCooldownTimer - Time.deltaTime);
        }

        if (_isDashing)
        {
            controller.Move(_dashDirection * dashSpeed * Time.deltaTime);
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
            }
        }

        if (UIManager.instance)
        {
            UIManager.instance.SetDashBar(1 - (_dashCooldownTimer / dashCooldown));
        }
    }

    public void ProcesMove(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        controller.Move(transform.TransformDirection(moveDirection) * _speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;
        if (controller.isGrounded && playerVelocity.y < -0.05f)
        {
            playerVelocity.y = -0.05f;
        }

        controller.Move(playerVelocity);
    }

    public void Jump()
    {
        if (controller.isGrounded && playerVelocity.y <= 0)
        {
            playerVelocity.y = jumpHeight * Time.deltaTime;
        }
    }

    public void ProcessCrouch()
    {
        if (_lerpCroucing)
        {
            _crouchTimer += Time.deltaTime;
            float p = _crouchTimer / 1;
            p *= p;
            if (_crouching)
            {
                controller.height = Mathf.Lerp(controller.height, 1, p);
            }
            else
            {
                controller.height = Mathf.Lerp(controller.height, 2, p);
            }

            if (p > 1)
            {
                _lerpCroucing = false;
                _crouchTimer = 0;
            }
        }
    }

    public void Crouch()
    {
        _crouching = !_crouching;
        _speed = _crouching ? crouchSpeed : baseSpeed;
        _crouchTimer = 0;
        _lerpCroucing = true;
    }

    public void Sprint()
    {
        _sprinting = !_sprinting;
        if (_sprinting)
        {
            _speed = sprintSpeed;
        }
        else
        {
            _speed = baseSpeed;
        }
    }
}
