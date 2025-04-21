using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab & limites")]
    public GameObject enemyPrefab;
    public int maxEnemies = 10;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float spawnDistance = 3f;
    public float minSpawnDistance = 2f;  // Distance minimum du joueur
    public bool spawnOnlyOnPlanes = true;

    [Header("Références AR")]
    public ARPlaneManager planeManager;

    private List<GameObject> enemies = new List<GameObject>();
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null) Debug.LogError("[Spawner] Pas de caméra principale !");
        
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

    private bool IsPositionOnPlane(Vector3 position, out Vector3 planePosition)
    {
        planePosition = position;
        
        if (!spawnOnlyOnPlanes || planeManager == null || planeManager.trackables.count == 0)
            return true;

        foreach (var plane in planeManager.trackables)
        {
            // Projeter le point sur le plan
            Vector3 projectedPoint = plane.transform.position;
            projectedPoint.y = plane.transform.position.y;

            // Vérifier si le point est dans les limites du plane
            Vector2 planeSize = plane.size * 0.5f;
            Vector3 localPoint = plane.transform.InverseTransformPoint(position);

            if (Mathf.Abs(localPoint.x) <= planeSize.x && 
                Mathf.Abs(localPoint.z) <= planeSize.y)
            {
                planePosition = projectedPoint;
                return true;
            }
        }

        return false;
    }

    private void SpawnOne()
    {
        if (enemyPrefab == null || mainCamera == null) return;

        for (int attempts = 0; attempts < 5; attempts++)  // Plusieurs tentatives pour trouver une position valide
        {
            // Position de base à la hauteur de la caméra
            Vector3 cameraPosition = mainCamera.transform.position;
            
            // Direction aléatoire autour du joueur
            float randomAngle = Random.Range(0f, 360f);
            Vector3 spawnDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
            
            // Position finale du spawn
            float randomDistance = Random.Range(minSpawnDistance, spawnDistance);
            Vector3 spawnPos = cameraPosition + (spawnDirection * randomDistance);

            // Vérifier si la position est valide sur un plane
            Vector3 validPosition;
            if (IsPositionOnPlane(spawnPos, out validPosition))
            {
                // Créer l'ennemi
                var e = Instantiate(enemyPrefab, validPosition, Quaternion.identity);
                e.transform.LookAt(new Vector3(cameraPosition.x, validPosition.y, cameraPosition.z));
                
                var script = e.GetComponent<Enemy>();
                if (script != null)
                {
                    enemies.Add(e);
                    script.OnDeath += () => enemies.Remove(e);
                }
                else
                {
                    Debug.LogError("[Spawner] Prefab sans script Enemy !", e);
                    Destroy(e);
                }
                
                break;  // Sortir de la boucle si on a réussi à spawn
            }
        }
    }
}
