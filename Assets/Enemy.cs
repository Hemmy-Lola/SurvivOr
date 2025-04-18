using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1f;
    public float health = 100f; // Points de vie
    public event Action OnDeath;
    private Transform targetCam;

    void Start()
    {
        targetCam = Camera.main.transform;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetCam.position,
            speed * Time.deltaTime
        );
    }

    public void ReceiveDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} a reçu {damage} dégâts. PV restants : {health}");

        if (health <= 0)
            Die();
    }

    public void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}