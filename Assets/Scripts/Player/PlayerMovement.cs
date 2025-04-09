using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;

    public static float baseSpeed = 5f;
    public static float sprintSpeed = 8f;
    public static float gravity = -9.8f;
    public static float jumpHeight = 3f;

    private float _speed = baseSpeed;

    private bool _crouching = false;
    private bool _lerpCroucing = false;
    private float _crouchTimer = 0f;


    private bool _sprinting = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        ProcessCrouch();
    }

    public void ProcesMove(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        controller.Move(transform.TransformDirection(moveDirection) * _speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (controller.isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
        }
    }

    public void ProcessCrouch()
    {
        if (_lerpCroucing)
        {
            _crouchTimer += Time.deltaTime;
            Debug.Log(_crouchTimer + " crouch");
            float p = _crouchTimer / 1;
            Debug.Log(p + " p");
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
