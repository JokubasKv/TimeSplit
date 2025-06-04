using UnityEngine;

public class ShootableObject : MonoBehaviour
{
    [Header("Required References")]
    public Transform attackPoint;
    public GameObject bullet;

    [Header("Shooting Settings")]
    public float shootForce;
    public float timeBetweenShooting;
    public float spread;

    [SerializeField]
    public int bulletsLeft;

    bool readyToShoot = true;
    bool allowInvoke = true;

    public void Shoot(Vector3 target)
    {
        if (!readyToShoot)
        {
            return;
        }
        readyToShoot = false;

        Vector3 directionWithoutSpread = target - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, bullet.transform.rotation);
        currentBullet.transform.forward = directionWithSpread.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * shootForce, ForceMode.Impulse);

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
}
