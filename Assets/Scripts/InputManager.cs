using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    public InputSystem_Actions.PlayerActions playerActions;

    private PlayerMovement _playerMovement;
    private PlayerLook _playerLook;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _inputActions = new InputSystem_Actions();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerLook = GetComponent<PlayerLook>();

        playerActions = _inputActions.Player;

        playerActions.Jump.performed += ctx => _playerMovement.Jump();
        playerActions.Crouch.performed += ctx => _playerMovement.Crouch();
        playerActions.Sprint.performed += ctx => _playerMovement.Sprint();
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
