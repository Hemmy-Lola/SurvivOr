using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public float speed = 1f;
    public float damage = 10f; // Dégâts infligés
    public float health = 100f; // Points de vie
    public event Action OnDeath;
    private Transform camTarget;
    public GameObject prefabsEnnemis; // Liste des prefabs d'ennemis

    void Start()
    {
        camTarget = Camera.main?.transform;
        if (camTarget == null)
        {
            Debug.LogError("[Enemy] Pas de MainCamera taggée !");
            enabled = false;
        }

        // Optionnel : Pour s'assurer que le Rigidbody réagit aux collisions en mode non-kinematic
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
            player.RecevoirDegats(damage); // Infligez les dégâts ici
        }
    }

    public void Die()
    {
        ScoreManager.AjouterZombieTue(this.GetType().Name, 10); // Exemple : 10 points par zombie
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}