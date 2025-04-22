using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 1f;
    public float damage = 10f;
    public float health = 100f;
    public float minDistanceToPlayer = 0.5f;
    public float attackCooldown = 1f;

    [Header("Références")]
    public GameObject prefabsEnnemis;
    private Transform camTarget;
    private float lastAttackTime;

    public event Action OnDeath;

    void Start()
    {
        camTarget = Camera.main?.transform;
        if (camTarget == null)
        {
            Debug.LogError("[Enemy] Pas de MainCamera taggée !");
            enabled = false;
            return;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    void Update()
    {
        if (camTarget == null) return;

        // Regarder vers la caméra
        Vector3 lookAtTarget = new Vector3(camTarget.position.x, transform.position.y, camTarget.position.z);
        transform.LookAt(lookAtTarget);

        float distance = Vector3.Distance(transform.position, camTarget.position);

        if (distance > minDistanceToPlayer)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                camTarget.position,
                speed * Time.deltaTime
            );
        }
        else
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
            }
        }
    }


    public void ReceiveDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} a reçu {damage} dégâts. PV restants : {health}");

        if (health <= 0)
            Die();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<RegenerationSystem>(out var player))
        {
            Debug.Log($"[Enemy] Collision avec {player.gameObject.name}");
            player.RecevoirDegats(damage);
        }
    }

    public void Die()
    {
        ScoreManager.AjouterZombieTue(this.GetType().Name, 10);
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
