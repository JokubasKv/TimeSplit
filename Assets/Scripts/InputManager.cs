using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    public InputSystem_Actions.PlayerActions playerActions;

    private PlayerMovement _playerMovement;
    private PlayerLook _playerLook;
    private PlayerInteract _playerInteract;
    private PlayerHold _playerHold;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _inputActions = new InputSystem_Actions();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerLook = GetComponent<PlayerLook>();
        _playerInteract = GetComponent<PlayerInteract>();
        _playerHold = GetComponent<PlayerHold>();

        playerActions = _inputActions.Player;

        playerActions.Jump.performed += ctx => _playerMovement.Jump();
        playerActions.Crouch.performed += ctx => _playerMovement.Crouch();
        playerActions.Sprint.performed += ctx => _playerMovement.Sprint();
        playerActions.Interact.performed += ctx => _playerInteract.Interact();
        playerActions.Throw.performed += ctx => _playerHold.ThrowObject();

        playerActions.Attack.started += ctx => _playerHold.AttackPressed();
        playerActions.Attack.canceled += ctx => _playerHold.AttackReleased();
    }

    private void Update()
    {
        _playerMovement.ProcesMove(playerActions.Move.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        _playerLook.ProcessLook(playerActions.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        playerActions.Enable();
    }

    private void OnDisable()
    {
        playerActions.Disable();
    }
}
