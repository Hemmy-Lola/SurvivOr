using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    public float speed = 1f;
    public float damage = 10f; 
    public float health = 100f; 
    public event Action OnDeath;
    private Transform camTarget;
    public GameObject prefabsEnnemis; 

    void Start()
    {
        camTarget = Camera.main?.transform;
        if (camTarget == null)
        {
            Debug.LogError("[Enemy] Pas de MainCamera taggée !");
            enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    void Update()
    {
        if (camTarget != null)
            transform.position = Vector3.MoveTowards(
                transform.position,
                camTarget.position,
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

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Enemy] Collision détectée avec {collision.gameObject.name}");

        if (collision.gameObject.TryGetComponent<RegenerationSystem>(out var player))
        {
            Debug.Log($"[Enemy] Collision détectée avec {player.gameObject.name}");
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