using Unity.VisualScripting;
using UnityEngine;

public class PlayerHold : MonoBehaviour
{
    private Camera _camera;

    public GameObject player;
    public Transform holdPos;

    public float throwForce = 500f;
    public float pickUpRange = 5f;
    private GameObject heldObj;
    private Rigidbody heldObjRb;

    private bool _attackPressed = false;
    private ShootableObject _shootableObject;

    void Start()
    {
        _camera = GetComponent<PlayerLook>().camera;
    }

    void Update()
    {
        if (heldObj != null)
        {
            MoveObject();

            if (_attackPressed && _shootableObject != null)
            {
                Vector3 target = GetPointInfrontOfCamera();
                _shootableObject.Shoot(target);
            }
        }
    }

    public void PickUpObject(GameObject pickUpObj)
    {
        var rb = pickUpObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            heldObj = pickUpObj;

            var pickupableInteractable = pickUpObj.GetComponent<PickupableInteractable>();

            if (pickupableInteractable.staticPickupRotation)
            {
                heldObj.transform.localRotation = Quaternion.Euler(pickupableInteractable.pickupRotation);
            }

            heldObjRb = rb;
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform;
            //heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }

        var shootableObject = pickUpObj.GetComponent<ShootableObject>();
        if (shootableObject != null)
        {
            _shootableObject = shootableObject;
        }
    }
    public void DropObject()
    {
        StopClipping();

        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;

        RemoveObjectReference();
    }
    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }

    public Vector3 GetPointInfrontOfCamera()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        return targetPoint;
    }

    public void ThrowObject()
    {
        if (heldObj == null)
        {
            return;
        }

        StopClipping();

        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        //heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);

        PickupableInteractable pickupableInteractable = heldObj.GetComponent<PickupableInteractable>();
        pickupableInteractable.Throw();

        RemoveObjectReference();
    }

    public void RemoveObjectReference()
    {
        heldObj = null;
        _shootableObject = null;
    }

    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }

    public void AttackPressed()
    {
        _attackPressed = true;
    }

    public void AttackReleased()
    {
        _attackPressed = false;
    }
}
