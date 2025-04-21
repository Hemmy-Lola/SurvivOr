using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab & limites")]
    public List<GameObject> enemyPrefabs;
    public int maxEnemies = 10;

    [Header("Paramètres de spawn")]
    public Transform[] spawnZones;
    public float spawnInterval = 2f;
    public bool spawnOnlyOnPlanes = true;

    [Header("Références AR")]
    public ARPlaneManager planeManager;

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
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError("❌ Aucun prefab d'ennemi assigné !");
            return;
        }

        Vector3 spawnPosition;

        if (spawnOnlyOnPlanes && planeManager != null && planeManager.trackables.count > 0)
        {
            var planes = new List<ARPlane>();
            foreach (var p in planeManager.trackables)
                planes.Add(p);
            var plane = planes[Random.Range(0, planes.Count)];

            Vector2 halfSize = plane.size * 0.5f;
            float localX = Random.Range(-halfSize.x, halfSize.x);
            float localZ = Random.Range(-halfSize.y, halfSize.y);
            Vector3 localOffset = new Vector3(localX, 0f, localZ);

            spawnPosition = plane.transform.TransformPoint(localOffset);
        }
        else
        {
            Transform spawnZone = spawnZones[Random.Range(0, spawnZones.Length)];
            spawnPosition = spawnZone.position + new Vector3(
                Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)
            );
        }

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript == null)
        {
            Debug.LogError("❌ Le prefab d'ennemi n'a pas de script Enemy !");
            Destroy(enemy);
            return;
        }

        enemies.Add(enemy);
        enemyScript.OnDeath += () => enemies.Remove(enemy);
    }
}