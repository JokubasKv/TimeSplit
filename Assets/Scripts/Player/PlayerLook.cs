using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform cameraPivot;
    public new Camera camera;
    public float xRotation = 0f;

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    public void ProcessLook(Vector2 input)
    {
        xRotation -= (input.y * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.Rotate(Vector3.up * (input.x * Time.deltaTime) * xSensitivity);
    }
}
