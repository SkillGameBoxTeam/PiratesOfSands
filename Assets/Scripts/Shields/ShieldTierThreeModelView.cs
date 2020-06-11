﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTierThreeModelView : MonoBehaviour, IDamageable, IShield
{
    [SerializeField] private Animator animator;

    [SerializeField] private float duration;
    [SerializeField] private float durability;

    public float Duration
    {
        get => duration;
        set
        {
            if (duration != value)
            {
                duration = value;
            }
        }
    }

    public float Durability
    {
        get => durability;
        set
        {
            if (durability != value)
            {
                durability = value;
            }
        }
    }

    private float destroyTime;

    private void Start()
    {
        if(duration != 0)
            destroyTime = Time.time + duration;
    }

    private void Update()
    {
        if (destroyTime != 0)
        {
            if (Time.time > destroyTime)
            {
                animator.SetTrigger("Discharge");
                Destroy(gameObject, 1);
            }
        }
    }

    public void RecieveDamage(float amount)
    {
        durability -= amount;
        Debug.Log($"{gameObject.name} was damaged for {amount}. {durability} durability left");

        if (durability <= 0)
        {
            Debug.Log($"{gameObject.name} was destroyed!");
            animator.SetTrigger("Discharge");
            Destroy(gameObject, 1);
        }
    }
}