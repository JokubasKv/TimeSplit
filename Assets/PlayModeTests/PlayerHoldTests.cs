using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHoldTests
{
    private GameObject playerObj;
    private GameObject holdObj;
    private PlayerHold playerHold;
    private Camera camera;
    private GameObject pickupObj;
    private Rigidbody pickupRb;
    private Collider pickupCollider;
    private PickupableInteractable pickupableInteractable;
    private ShootableObject shootableObject;

    [SetUp]
    public void SetUp()
    {
        playerObj = new GameObject("Player");
        camera = playerObj.AddComponent<Camera>();
        playerObj.AddComponent<CharacterController>();
        var playerLook = playerObj.AddComponent<PlayerLook>();
        playerLook.camera = camera;
        playerHold = playerObj.AddComponent<PlayerHold>();
        playerHold.player = playerObj;
        playerHold.holdPos = new GameObject("HoldPos").transform;

        pickupObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pickupRb = pickupObj.AddComponent<Rigidbody>();
        pickupCollider = pickupObj.GetComponent<BoxCollider>();
        pickupableInteractable = pickupObj.AddComponent<PickupableInteractable>();
        shootableObject = pickupObj.AddComponent<ShootableObject>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(pickupObj);
    }

    [Test]
    public void PickUpObject_SetsHeldObjectAndParent()
    {
        playerHold.PickUpObject(pickupObj);
        Assert.AreEqual(pickupObj, GetPrivateField<GameObject>(playerHold, "heldObj"));
        Assert.AreEqual(playerHold.holdPos, pickupObj.transform.parent);
        Assert.IsTrue(pickupRb.isKinematic);
    }

    [Test]
    public void DropObject_ClearsHeldObjectAndUnparents()
    {
        playerHold.PickUpObject(pickupObj);
        playerHold.DropObject();
        Assert.IsNull(GetPrivateField<GameObject>(playerHold, "heldObj"));
        Assert.IsNull(pickupObj.transform.parent);
        Assert.IsFalse(pickupRb.isKinematic);
    }

    [UnityTest]
    public IEnumerator ThrowObject_AddsForceAndClearsHeldObject()
    {
        playerHold.PickUpObject(pickupObj);
        pickupableInteractable.isThrown = false;
        playerHold.ThrowObject();
        yield return new WaitForFixedUpdate();
        Assert.IsNull(GetPrivateField<GameObject>(playerHold, "heldObj"));
        Assert.IsTrue(pickupRb.linearVelocity.magnitude > 0);
        Assert.IsTrue(pickupableInteractable.isThrown);
    }

    [Test]
    public void AttackPressed_SetsAttackPressedTrue()
    {
        playerHold.AttackPressed();
        Assert.IsTrue(GetPrivateField<bool>(playerHold, "_attackPressed"));
    }

    [Test]
    public void AttackReleased_SetsAttackPressedFalse()
    {
        playerHold.AttackPressed();
        playerHold.AttackReleased();
        Assert.IsFalse(GetPrivateField<bool>(playerHold, "_attackPressed"));
    }

    [Test]
    public void RemoveObjectReference_ClearsReferences()
    {
        playerHold.PickUpObject(pickupObj);
        playerHold.RemoveObjectReference();
        Assert.IsNull(GetPrivateField<GameObject>(playerHold, "heldObj"));
        Assert.IsNull(GetPrivateField<ShootableObject>(playerHold, "_shootableObject"));
    }

    // Helper to get private fields
    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (T)field.GetValue(obj);
    }
}