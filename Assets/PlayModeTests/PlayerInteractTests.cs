using NUnit.Framework;
using UnityEngine;

public class PlayerInteractTests
{
    private GameObject _playerObj;
    private GameObject _uiManagerObj;
    private PlayerInteract _playerInteract;
    private PlayerLook _playerLook;
    private Camera _camera;
    private UIManager _uiManager;
    private PlayerHold _playerHold;

    [SetUp]
    public void SetUp()
    {
        _playerObj = new GameObject();
        _camera = _playerObj.AddComponent<Camera>();
        _playerLook = _playerObj.AddComponent<PlayerLook>();
        _playerLook.camera = _camera;

        _playerHold = _playerObj.AddComponent<PlayerHold>();
        _playerInteract = _playerObj.AddComponent<PlayerInteract>();

        _uiManagerObj = new GameObject();
        _uiManager = _uiManagerObj.AddComponent<UIManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_playerObj);
        Object.DestroyImmediate(_uiManagerObj);
    }

    [Test]
    public void Start_InitializesReferences()
    {
        _playerInteract.SendMessage("Start");
        Assert.AreEqual(_camera, typeof(PlayerInteract).GetField("_camera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_playerInteract));
        Assert.AreEqual(_uiManager, typeof(PlayerInteract).GetField("_uIManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_playerInteract));
        Assert.AreEqual(_playerHold, typeof(PlayerInteract).GetField("_playerHold", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_playerInteract));
    }

    // Helper test classes
    private class TestInteractable : AbstractInteractable
    {
        public bool BaseInteractCalled = false;
        protected override void Interact() { BaseInteractCalled = true; }
    }

    private class TestPickupableInteractable : PickupableInteractable
    {
        public bool BaseInteractCalled = false;
        protected override void Interact() { BaseInteractCalled = true; }
    }

    [Test]
    public void Interact_CallsBaseInteract_WhenInteractable()
    {
        var interactableObj = new GameObject();
        var interactable = interactableObj.AddComponent<TestInteractable>();
        typeof(PlayerInteract).GetField("_currentLookingObj", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_playerInteract, interactableObj);

        _playerInteract.Interact();

        Assert.IsTrue(interactable.BaseInteractCalled);
        Object.DestroyImmediate(interactableObj);
    }
}
