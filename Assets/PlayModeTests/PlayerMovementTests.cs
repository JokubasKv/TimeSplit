using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovementTests
{
    private GameObject playerObj;
    private PlayerMovement playerMovement;
    private CharacterController controller;
    private GameObject groundObj;

    [SetUp]
    public void SetUp()
    {
        // Create ground
        groundObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        groundObj.transform.position = Vector3.zero;

        // Create player
        playerObj = new GameObject("Player");
        controller = playerObj.AddComponent<CharacterController>();
        playerMovement = playerObj.AddComponent<PlayerMovement>();
        controller.height = 2f;
        playerObj.transform.position = new Vector3(0, 2f, 0);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(groundObj);
    }

    [Test]
    public void Start_SetsSpeedToBaseSpeed()
    {
        playerMovement.baseSpeed = 7f;
        playerMovement.SendMessage("Start");
        var speedField = typeof(PlayerMovement).GetField("_speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.AreEqual(7f, (float)speedField.GetValue(playerMovement));
    }

    [Test]
    public void Crouch_TogglesCrouchingAndSpeed()
    {
        playerMovement.baseSpeed = 5f;
        playerMovement.crouchSpeed = 2f;
        playerMovement.SendMessage("Start");

        playerMovement.Crouch();
        var speedField = typeof(PlayerMovement).GetField("_speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.AreEqual(2f, (float)speedField.GetValue(playerMovement));

        playerMovement.Crouch();
        Assert.AreEqual(5f, (float)speedField.GetValue(playerMovement));
    }

    [Test]
    public void Sprint_TogglesSprintingAndSpeed()
    {
        playerMovement.baseSpeed = 5f;
        playerMovement.sprintSpeed = 10f;
        playerMovement.SendMessage("Start");

        playerMovement.Sprint();
        var speedField = typeof(PlayerMovement).GetField("_speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.AreEqual(10f, (float)speedField.GetValue(playerMovement));

        playerMovement.Sprint();
        Assert.AreEqual(5f, (float)speedField.GetValue(playerMovement));
    }

    [Test]
    public void Dash_SetsIsDashingAndDashDirection()
    {
        playerMovement.SendMessage("Start");
        Vector2 input = new Vector2(1, 0);
        playerMovement.Dash(input);

        var isDashing = typeof(PlayerMovement).GetField("_isDashing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsTrue((bool)isDashing.GetValue(playerMovement));

        var dashDir = typeof(PlayerMovement).GetField("_dashDirection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Vector3 dir = (Vector3)dashDir.GetValue(playerMovement);
        Assert.AreNotEqual(Vector3.zero, dir);
    }

    [Test]
    public void Dash_DoesNotDashIfCooldown()
    {
        playerMovement.SendMessage("Start");
        typeof(PlayerMovement).GetField("_dashCooldownTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(playerMovement, 1f);
        playerMovement.Dash(Vector2.one);
        var isDashing = typeof(PlayerMovement).GetField("_isDashing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Assert.IsFalse((bool)isDashing.GetValue(playerMovement));
    }

    [Test]
    public void ProcessCrouch_LerpsHeight()
    {
        playerMovement.SendMessage("Start");
        playerMovement.Crouch();
        typeof(PlayerMovement).GetField("_lerpCroucing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(playerMovement, true);
        typeof(PlayerMovement).GetField("_crouchTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(playerMovement, 0.5f);
        playerMovement.ProcessCrouch();
        Assert.Less(controller.height, 2f);
    }
}
