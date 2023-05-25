using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCharacter : Character
{
    [Range(0, 100)][SerializeField] private int _health = 100;
    [Range(0.5f, 10.0f)][SerializeField] private float _movingSpeed = 8.0f;
    [SerializeField] private UIview ui;
    [SerializeField] private float _acceleration = 3.0f;
    private const float _gravity = -9.8f;
    private CharacterController _characterController;
    private MouseLook _mouseLook;
    private HealthManager _healthManager;

    //private Vector3 _currentVelocity;
    protected override FireAction _fireAction { get; set; }
    
    
    protected override void Initiate()
    {
        base.Initiate();
        _fireAction = gameObject.AddComponent<RayShooter>();
        _fireAction.Reloading();
        _characterController = GetComponentInChildren<CharacterController>();
        _characterController ??= gameObject.AddComponent<CharacterController>();
        _mouseLook = GetComponentInChildren<MouseLook>();
        _mouseLook ??= gameObject.AddComponent<MouseLook>();

        _healthManager = GetComponentInChildren<HealthManager>();
        _healthManager??= gameObject.AddComponent<HealthManager>();
        _healthManager.InitializeHealth(_health);

        OnUpdateAction += UpdateUI;
    }

    public override void Movement()
    {
        if (_mouseLook != null && _mouseLook.PlayerCamera != null)
        {
            _mouseLook.PlayerCamera.enabled = hasAuthority;
        }
        if (hasAuthority)
        {
            var moveX = Input.GetAxis("Horizontal") * _movingSpeed;
            var moveZ = Input.GetAxis("Vertical") * _movingSpeed;
            var movement = new Vector3(moveX, 0, moveZ);
            movement = Vector3.ClampMagnitude(movement, _movingSpeed);
            movement *= Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                movement *= _acceleration;
            }
            movement.y = _gravity;
            movement = transform.TransformDirection(movement);
            _characterController.Move(movement);
            _mouseLook.Rotation();
            //CmdUpdatePosition(transform.position);
        }
        /*
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, serverPosition, ref _currentVelocity, _movingSpeed * Time.deltaTime);
        }
        */
    }

    private void Start()
    {
        Initiate();
    }

    private void UpdateUI()
    {
        ui.hp.text = $"Health: {_healthManager.Health}";
        ui.bullets.text = $"Clip: {_fireAction.BulletCount}";
    }

    /*
    private void OnGUI()
    {
        if (Camera.main == null)
        {
            return;
        }
        var info = $"Health: {_healthManager.Health}\nClip: {_fireAction.BulletCount}";
        var size = 12;
        var bulletCountSize = 50;
        var posX = _mouseLook.PlayerCamera.pixelWidth / 2 - size / 4;
        var posY = _mouseLook.PlayerCamera.pixelHeight / 2 - size / 2;
        var posXBul = _mouseLook.PlayerCamera.pixelWidth - bulletCountSize * 2;
        var posYBul = _mouseLook.PlayerCamera.pixelHeight - bulletCountSize;
        GUI.Label(new Rect(posX, posY, size, size), "+");
        GUI.Label(new Rect(posXBul, posYBul, bulletCountSize * 2,
        bulletCountSize * 2), info);
    }
    */
    private void OnDestroy()
    {
        OnUpdateAction -= UpdateUI;

    }
}
