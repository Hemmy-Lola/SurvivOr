// Shooter.cs
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Références")]
    public Camera cam;

    [Header("Paramètres de tir")]
    public float maxDistance = 50f;
    public LayerMask hitMask = ~0; 
    public GameObject shotEffectPrefab = null; 
    public float damageAmount = 20f;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            Debug.LogError("[Shooter] Pas de caméra assignée ni taggée MainCamera !");
    }

    void Update()
    {
        if (cam == null)
            return;

        bool clicked = Input.GetMouseButtonDown(0);
        bool tapped = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (clicked || tapped)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Shoot(screenCenter);
        }
    }

    void Shoot(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitMask))
        {
            Debug.Log($"[Shooter] Touché : {hit.collider.name}");

            if (hit.collider.TryGetComponent<Enemy>(out var enemy))
                enemy.ReceiveDamage(damageAmount);

            if (shotEffectPrefab != null)
                Instantiate(shotEffectPrefab, hit.point, Quaternion.identity);
        }
    }
}