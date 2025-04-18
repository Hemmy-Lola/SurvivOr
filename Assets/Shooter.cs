// Shooter.cs
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Références")]
    public Camera cam;

    [Header("Tir")]
    public float maxDistance = 50f;
    public LayerMask hitMask = ~0;
    public GameObject shotEffectPrefab;

    void Start()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) Debug.LogError("[Shooter] Pas de MainCamera taggée !");
    }

    void Update()
    {
        if (cam == null) return;

        bool click = Input.GetMouseButtonDown(0);
        bool tap = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;

        if (click || tap)
            Shoot(new Vector2(Screen.width / 2f, Screen.height / 2f));
    }

    void Shoot(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 1f);

        if (Physics.Raycast(ray, out var hit, maxDistance, hitMask))
        {
            if (hit.collider.TryGetComponent<Enemy>(out var enemy))
                enemy.Die();

            if (shotEffectPrefab != null)
                Instantiate(shotEffectPrefab, hit.point, Quaternion.identity);
        }
    }
}
