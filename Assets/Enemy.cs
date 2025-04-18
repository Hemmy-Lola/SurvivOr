// Enemy.cs
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1f;
    public event Action OnDeath;
    private Transform camTarget;

    void Start()
    {
        camTarget = Camera.main?.transform;
        if (camTarget == null)
        {
            Debug.LogError("[Enemy] Pas de MainCamera taggée !");
            enabled = false;
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

    public void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
