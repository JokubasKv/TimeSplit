using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera _camera;
    private UIManager _uIManager;
    private PlayerHold _playerHold;

    [SerializeField] private float _rayDistance = 3f;
    [SerializeField] private LayerMask _mask;

    private GameObject _currentLookingObj;

    void Start()
    {
        _camera = GetComponent<PlayerLook>().camera;
        _uIManager = UIManager.instance;
        _playerHold = GetComponent<PlayerHold>();
    }


    void Update()
    {
        _uIManager.SetPromptText(string.Empty);

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * _rayDistance);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, _rayDistance, _mask))
        {
            if (hitInfo.collider.GetComponent<AbstractInteractable>() != null)
            {
                AbstractInteractable interactable = hitInfo.collider.GetComponent<AbstractInteractable>();
                _uIManager.SetPromptText(interactable.promptMessage);
            }

            _currentLookingObj = hitInfo.collider.gameObject;
        }
        else
        {
            _currentLookingObj = null;
        }

    }

    public void Interact()
    {
        if (_currentLookingObj == null)
        {
            return;
        }

        AbstractInteractable interactable = _currentLookingObj.GetComponent<AbstractInteractable>();
        if (interactable != null)
        {
            interactable.BaseInteract();
            if (interactable is PickupableInteractable)
            {
                _playerHold.PickUpObject(_currentLookingObj);
            }
        }
    }
}
