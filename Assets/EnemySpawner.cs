using System.Collections;
using System.Collections.Generic;
using System.Linq;                        // ← indispensable pour ToList()
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab & limites")]
    public GameObject enemyPrefab;
    public int maxEnemies = 10;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public bool spawnOnlyOnPlanes = true;
    public float spawnDistance = 3f;  // Distance à laquelle les ennemis apparaissent

    [Header("Références AR")]
    public ARPlaneManager planeManager;     // ← Assigne-le dans l'Inspector

    // liste interne des mobs vivants
    private List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        if (spawnOnlyOnPlanes && planeManager == null)
            Debug.LogWarning("[Spawner] spawnOnlyOnPlanes activé mais pas de planeManager !");
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (enemies.Count < maxEnemies)
                SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnOne()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("[Spawner] Pas de enemyPrefab assigné !", this);
            return;
        }

        var cam = Camera.main;
        if (cam == null) return;

        // Position de base à la hauteur de la caméra
        Vector3 cameraPosition = cam.transform.position;
        
        // Direction aléatoire autour du joueur
        float randomAngle = Random.Range(0f, 360f);
        Vector3 spawnDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
        
        // Position finale du spawn
        Vector3 spawnPos = cameraPosition + (spawnDirection * spawnDistance);

        // Si on utilise les planes AR, on ajuste la hauteur
        if (spawnOnlyOnPlanes && planeManager != null && planeManager.trackables.count > 0)
        {
            var planes = new List<ARPlane>();
            foreach (var p in planeManager.trackables)
                planes.Add(p);
            
            // Trouver le plane le plus proche
            ARPlane nearestPlane = null;
            float shortestDistance = float.MaxValue;
            
            foreach (var plane in planes)
            {
                float distance = Vector3.Distance(spawnPos, plane.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestPlane = plane;
                }
            }

            if (nearestPlane != null)
            {
                // Ajuster la hauteur selon le plane le plus proche
                spawnPos.y = nearestPlane.transform.position.y;
            }
        }

        // Créer l'ennemi et le faire regarder vers le joueur
        var e = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        e.transform.LookAt(cameraPosition);
        
        var script = e.GetComponent<Enemy>();
        if (script == null)
        {
            Debug.LogError("[Spawner] Prefab sans script Enemy !", e);
            Destroy(e);
            return;
        }

        enemies.Add(e);
        script.OnDeath += () => enemies.Remove(e);
    }
}
