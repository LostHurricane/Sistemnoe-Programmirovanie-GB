using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RayShooter : FireAction
{
    private Camera _camera;
    protected override void Start()
    {
        base.Start();
        _camera = GetComponentInChildren<Camera>();
    }
    private void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shooting();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reloading();
            }
            if (Input.anyKey && !Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

    }

    protected override void Shooting()
    {
        base.Shooting();
        if (bullets.Count > 0)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        if (reloading)
        {
            yield break;
        }
        CmdCalculateRaycastHit();
        var shoot = bullets.Dequeue();
        bulletCount = bullets.Count.ToString();
        ammunition.Enqueue(shoot);
        yield return new WaitForSeconds(2.0f);

    }

    [Command]
    private void CmdCalculateRaycastHit()
    {
        var point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
        var ray = _camera.ScreenPointToRay(point);
        if (!Physics.Raycast(ray, out var hit))
        {
            return;
        }

        Debug.Log(hit.collider.gameObject.name);
        var hp = hit.collider.GetComponentInParent<IDamagible>();
        if (hp != null)
        {
            hp.DealDamage(1);
            Debug.Log($"found i damagible and now target have {hp.Health}");

        }
    }

}
