using UnityEngine;
using UnityEngine.UIElements;

public class PickupableInteractable : AbstractInteractable
{
    private Renderer render;

    public bool isPickedUp = false;
    public bool isThrown = false;

    [Header("Pickup Settings")]
    public bool staticPickupRotation = false;
    public Vector3 pickupRotation = Vector3.zero;

    [Header("Throw Settings")]
    [SerializeField]
    public float throwDamage = 10f;


    [Header("Fade Settings")]
    [SerializeField] public bool fadesWhenPickedUp = false;
    private float goalAlpha;

    void Start()
    {
        goalAlpha = 1f;
        render = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        if (fadesWhenPickedUp)
        {
            ProcessFade();
        }
    }

    private void ProcessFade()
    {
        var color = render.material.color;

        if (color.a > 0 && color.a >= goalAlpha)
        {
            color.a -= 0.01f;
        }

        if (color.a < 1f && color.a <= goalAlpha)
        {
            color.a += 0.01f;
        }

        render.material.color = color;
    }

    protected override void Interact()
    {
        isPickedUp = true;
        isThrown = false;

        goalAlpha = 0.2f;
    }

    public void Throw()
    {
        isPickedUp = false;
        isThrown = true;

        goalAlpha = 1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
        {
            Debug.Log(collision.gameObject.name);
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
                Debug.Log(enemy);
                enemy.TakeDamage(throwDamage);
            }
        }

        isThrown = false;
    }
}
