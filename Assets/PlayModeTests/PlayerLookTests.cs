using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerLookTests
{
    private GameObject playerObject;
    private PlayerLook playerLook;
    private GameObject cameraPivotObject;
    private Camera cameraComponent;

    [SetUp]
    public void SetUp()
    {
        playerObject = new GameObject("Player");
        playerLook = playerObject.AddComponent<PlayerLook>();

        cameraPivotObject = new GameObject("CameraPivot");
        cameraPivotObject.transform.parent = playerObject.transform;
        playerLook.cameraPivot = cameraPivotObject.transform;

        var cameraGO = new GameObject("Camera");
        cameraComponent = cameraGO.AddComponent<Camera>();
        playerLook.camera = cameraComponent;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(cameraPivotObject);
        Object.DestroyImmediate(cameraComponent.gameObject);
    }

    [Test]
    public void ProcessLook_ClampsXRotation()
    {
        playerLook.xRotation = 0f;
        Vector2 input = new Vector2(0, 1000f); // Large Y input to exceed clamp
        float originalDeltaTime = Time.deltaTime;
        Time.timeScale = 1f;
        float fakeDeltaTime = 0.016f;
        typeof(Time).GetField("m_DeltaTime", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, fakeDeltaTime);

        playerLook.ProcessLook(input);

        Assert.That(playerLook.xRotation, Is.GreaterThanOrEqualTo(-80f));
        Assert.That(playerLook.xRotation, Is.LessThanOrEqualTo(80f));

        // Reset deltaTime
        typeof(Time).GetField("m_DeltaTime", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, originalDeltaTime);
    }

    [Test]
    public void ProcessLook_RotatesPlayerOnXAxis()
    {
        float initialYRotation = playerObject.transform.eulerAngles.y;
        Vector2 input = new Vector2(10f, 0f);
        float fakeDeltaTime = 0.016f;
        typeof(Time).GetField("m_DeltaTime", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, fakeDeltaTime);

        playerLook.ProcessLook(input);

        float newYRotation = playerObject.transform.eulerAngles.y;
        Assert.AreNotEqual(initialYRotation, newYRotation);
    }

    [Test]
    public void ProcessLook_RotatesCameraPivotOnXAxis()
    {
        Vector2 input = new Vector2(0f, 10f);
        float fakeDeltaTime = 0.016f;
        typeof(Time).GetField("m_DeltaTime", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, fakeDeltaTime);

        playerLook.ProcessLook(input);

        float xAngle = playerLook.cameraPivot.localRotation.eulerAngles.x;
        Assert.AreNotEqual(0f, xAngle);
    }
}
