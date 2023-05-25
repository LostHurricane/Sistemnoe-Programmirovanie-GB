using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthManager : NetworkBehaviour, IDamagible
{
    public event Action<UnityEngine.Object> OnDeath;

    [SyncVar] private int _health;
    public int Health { get => _health; }

    public void InitializeHealth(int maxHealth)
    {
        _health = maxHealth;
    }

    public void DealDamage(int amount)
    {
        _health -= Mathf.Abs(amount);

        if (_health < 0)
        {
            OnDeath?.Invoke(this);
        }
    }



}
