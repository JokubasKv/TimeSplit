using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera _camera;
    private PlayerUI _playerUI;
    private InputManager _inputManager;

    [SerializeField] private float _rayDistance = 3f;
    [SerializeField] private LayerMask _mask;


    void Start()
    {
        _camera = GetComponent<PlayerLook>().camera;
        _playerUI = GetComponent<PlayerUI>();
        _inputManager = GetComponent<InputManager>();
    }


    void Update()
    {
        _playerUI.UpdateText(string.Empty);

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * _rayDistance);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, _rayDistance, _mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                _playerUI.UpdateText(interactable.promptMessage);

                if (_inputManager.playerActions.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
