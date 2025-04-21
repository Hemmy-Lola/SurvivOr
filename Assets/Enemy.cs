using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action OnDeath;

    [Header("Audio")]
    public AudioClip attackClip;
    public AudioClip deathClip;
    public AudioClip spawnClip;

    private AudioSource audioSource;

    [Header("Stats")]
    public float health = 100f;
    public float speed = 1.5f;
    public float attackRange = 1.2f;
    public float attackCooldownMin = 1.5f;
    public float attackCooldownMax = 3f;
    public int damage = 10;

    [Header("Composants")]
    private Animator animator;
    private Transform target;
    private bool isDead = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Joue un son au spawn
        if (spawnClip != null)
            audioSource.PlayOneShot(spawnClip);

        animator = GetComponent<Animator>();

        // Cible la main caméra
        if (Camera.main != null)
            target = Camera.main.transform;

        // attaques aléatoires
        StartCoroutine(RandomAttackRoutine());
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private IEnumerator RandomAttackRoutine()
    {
        while (!isDead)
        {
            float waitTime = UnityEngine.Random.Range(attackCooldownMin, attackCooldownMax);
            yield return new WaitForSeconds(waitTime);

            if (isDead || target == null) yield break;

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= attackRange)
            {
                animator.SetTrigger("attack");

                // Son attaque
                if (attackClip != null)
                    audioSource.PlayOneShot(attackClip);

                Debug.Log("[Zombie] Attaque !");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("die");

        // ✅ Son mort
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        OnDeath?.Invoke();
        Destroy(gameObject, 3f);
    }
}

