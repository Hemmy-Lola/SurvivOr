using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1f;
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

    public void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
