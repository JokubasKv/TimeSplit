using UnityEditor.Rendering.LookDev;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;
using Unity.VisualScripting;
using UnityEngine.InputSystem.XR;

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
    public float gravity = -9.81f;                   // gravity / fall rate
    public float jumpHeight = 2.5f;                  // jump height



    void Start()
    {
        controller = GetComponent<CharacterController>();

        _speed = baseSpeed;
    }

    void Update()
    {
        //Debug.Log(controller.isGrounded);
        ProcessCrouch();
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
        Debug.Log("Jump");
        Debug.Log(playerVelocity.y);
        if (controller.isGrounded && playerVelocity.y <= 0)
        {
            playerVelocity.y = jumpHeight * Time.deltaTime;
        }
        Debug.Log(playerVelocity.y);
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
