using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public float minDistanceToPlayer = 0.5f;  // Distance minimale avant d'attaquer
    public float damageAmount = 10f;          // Dégâts infligés au joueur
    public float attackCooldown = 1f;         // Temps entre chaque attaque

    private Transform target;
    private float lastAttackTime;
    public event Action OnDeath;

    void Start()
    {
        // Trouver la caméra du joueur comme cible
        var cam = Camera.main;
        if (cam != null)
            target = cam.transform;
        else
            Debug.LogError("[Enemy] Pas de caméra principale trouvée!");
    }

    void Update()
    {
        if (target == null) return;

        // Calculer la distance au joueur
        Vector3 directionToPlayer = target.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Regarder vers le joueur
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        // Si on est plus loin que la distance minimale, on avance
        if (distanceToPlayer > minDistanceToPlayer)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        // Sinon, on attaque si le cooldown est passé
        else if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        // TODO: Implémenter les dégâts au joueur
        lastAttackTime = Time.time;
        Debug.Log($"[Enemy] Attaque le joueur! Dégâts: {damageAmount}");
    }

    public void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
