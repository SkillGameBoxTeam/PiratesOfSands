﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaMineShotModelView : MonoBehaviour
{
    #region Fields
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    [SerializeField] private float force;
    [SerializeField] private float radius;

    private Collider[] affectedColliders;
    private HashSet<GameObject> affectedObjects;

    private bool isFloating;

    private RaycastHit startHeight;
    private RaycastHit currentHeight;

    #endregion

    #region Accessors

    public Rigidbody Rigidbody { get => rb; }

    public float Damage
    {
        get => damage;
        set
        {
            if (damage != value)
            {
                damage = value;
            }
        }
    }

    public float Speed
    {
        get => speed;
        set
        {
            if (speed != value)
            {
                speed = value;
            }
        }
    }

    public float Lifetime
    {
        get => lifetime;
        set
        {
            if (lifetime != value)
            {
                lifetime = value;
            }
        }
    }

    public float Force
    {
        get => force;
        set
        {
            if (force != value)
            {
                force = value;
            }
        }
    }

    public float Radius
    {
        get => radius;
        set
        {
            if (radius != value)
            {
                radius = value;
            }
        }
    }

    #endregion

    private void Start()
    {
        affectedObjects = new HashSet<GameObject>();

        LaunchProjectile();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radius);
    }

    private void Update()
    {
        affectedColliders = Physics.OverlapSphere(transform.position, radius);

        if (isFloating)
        {
            Physics.Raycast(transform.position, Vector3.down, out currentHeight, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground"));

            if (currentHeight.distance != startHeight.distance)
                rb.MovePosition(new Vector3(rb.transform.position.x, rb.transform.position.y - currentHeight.distance + startHeight.distance, rb.transform.position.z));
        }
    }

    private void LaunchProjectile()
    {
        isFloating = true;
        rb.useGravity = false;

        Physics.Raycast(transform.position, Vector3.down, out startHeight);

        rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);
        rb.AddRelativeTorque(Vector3.right * speed, ForceMode.Impulse);

        StartCoroutine(DelayedDestroy(lifetime));
    }


    private void Explode()
    {
        if (affectedColliders != null)
        {
            foreach (Collider collider in affectedColliders)
            {
                if (collider && affectedObjects.Contains(collider.gameObject) == false)
                {
                    affectedObjects.Add(collider.gameObject);
                }
            }

            foreach (GameObject obj in affectedObjects)
            {
                if (obj.TryGetComponent<MonoBehaviour>(out MonoBehaviour mb))
                {
                    if (mb is IDamageable)
                    {
                        mb.TryGetComponent<Rigidbody>(out Rigidbody rb);
                        rb.AddExplosionForce(force, transform.position, radius, 10f, ForceMode.Impulse);

                        Vector3 closestPoint = rb.ClosestPointOnBounds(transform.position);
                        float distance = Vector3.Distance(closestPoint, transform.position);

                        float calculatedDamage = 1.0f - Mathf.Clamp01(distance / radius);
                        calculatedDamage *= damage;

                        ((IDamageable)mb).RecieveDamage(Mathf.Round(calculatedDamage));
                    }
                }
            }
        }

        ParticleFactory.CreateBigExplosion(transform);
        Destroy(gameObject);
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);

        isFloating = false;
        rb.useGravity = true;

        yield return new WaitForSeconds(delay);

        Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<MonoBehaviour>(out MonoBehaviour mb))
        {
            if (mb is IDamageable)
            {
                if (mb.tag.Equals("Ship"))
                {
                    ParticleFactory.CreateShipCollision(mb.transform);
                    Explode();
                }
            }
        }

        else if (collision.collider.tag.Equals("Ground"))
            ParticleFactory.CreateSandExplosion(transform);

        isFloating = false;
        rb.useGravity = true;
        //rb.velocity /= 2f;
    }
}
