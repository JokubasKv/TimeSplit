using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class StationaryEnemy : Enemy
{
    public GameObject TurretPart;

    void Start()
    {
        _stateMachine.Initialize(new StationaryState());
    }

    public override void LookAt(Vector3 position)
    {
        TurretPart.transform.LookAt(position);
    }

    public override bool CanSeePlayer()
    {
        if (_player == null) return false;

        if (Vector3.Distance(TurretPart.transform.position, _player.transform.position) < sightDistance)
        {
            Vector3 targetDirecion = _player.transform.position - TurretPart.transform.position - (Vector3.up * eyeHeight);
            float angleToPlayer = Vector3.Angle(targetDirecion, TurretPart.transform.forward);

            if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
            {
                Ray ray = new Ray(TurretPart.transform.position + (Vector3.up * eyeHeight), targetDirecion);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, sightDistance))
                {
                    if (hitInfo.transform.gameObject == _player)
                    {
                        Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                        return true;
                    }
                }
            }
        }

        return false;
    }
}