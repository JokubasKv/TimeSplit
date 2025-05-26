using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PickupableInteractableTests
{
    private GameObject testObject;
    private PickupableInteractable pickupable;
    private Renderer renderer;

    [SetUp]
    public void SetUp()
    {
        testObject = new GameObject();
        testObject.AddComponent<BoxCollider>();
        renderer = testObject.AddComponent<MeshRenderer>();
        pickupable = testObject.AddComponent<PickupableInteractable>();
        pickupable.fadesWhenPickedUp = true;
        pickupable.throwDamage = 15f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testObject);
    }

    [Test]
    public void Interact_SetsIsPickedUpTrue_AndIsThrownFalse()
    {
        pickupable.isPickedUp = false;
        pickupable.isThrown = true;

        // Call protected Interact via reflection
        var method = typeof(PickupableInteractable).GetMethod("Interact", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Invoke(pickupable, null);

        Assert.IsTrue(pickupable.isPickedUp);
        Assert.IsFalse(pickupable.isThrown);
    }

    [Test]
    public void Throw_SetsIsPickedUpFalse_AndIsThrownTrue()
    {
        pickupable.isPickedUp = true;
        pickupable.isThrown = false;

        pickupable.Throw();

        Assert.IsFalse(pickupable.isPickedUp);
        Assert.IsTrue(pickupable.isThrown);
    }

    [Test]
    public void Throw_SetsGoalAlphaTo1()
    {
        pickupable.Throw();
        var goalAlphaField = typeof(PickupableInteractable).GetField("goalAlpha", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        float goalAlpha = (float)goalAlphaField.GetValue(pickupable);
        Assert.AreEqual(1f, goalAlpha, 0.001f);
    }

    [Test]
    public void Interact_SetsGoalAlphaTo02()
    {
        // Call protected Interact via reflection
        var method = typeof(PickupableInteractable).GetMethod("Interact", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Invoke(pickupable, null);

        var goalAlphaField = typeof(PickupableInteractable).GetField("goalAlpha", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        float goalAlpha = (float)goalAlphaField.GetValue(pickupable);
        Assert.AreEqual(0.2f, goalAlpha, 0.001f);
    }

    [UnityTest]
    public System.Collections.IEnumerator ProcessFade_DecreasesAlpha_WhenGoalAlphaIsLower()
    {
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = new Color(1, 1, 1, 1f);

        var goalAlphaField = typeof(PickupableInteractable).GetField("goalAlpha", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        goalAlphaField.SetValue(pickupable, 0.5f);

        float initialAlpha = renderer.material.color.a;
        pickupable.fadesWhenPickedUp = true;

        // Simulate FixedUpdate
        pickupable.Invoke("FixedUpdate", 0f);
        yield return null;

        float newAlpha = renderer.material.color.a;
        Assert.Less(newAlpha, initialAlpha);
    }
}
