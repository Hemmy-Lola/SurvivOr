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

    [Header("Références AR")]
    public ARPlaneManager planeManager;     // ← Assigne-le dans l’Inspector

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
            Debug.LogError("[Spawner] Pas de enemyPrefab assigné !", this);
            return;
        }

        Vector3 spawnPos;

        if (spawnOnlyOnPlanes
            && planeManager != null
            && planeManager.trackables.count > 0)
        {
            var planes = new List<ARPlane>();
            foreach (var p in planeManager.trackables)
                planes.Add(p);
            var plane = planes[UnityEngine.Random.Range(0, planes.Count)];

            Vector2 halfSize = plane.size * 0.5f;

            float localX = UnityEngine.Random.Range(-halfSize.x, halfSize.x);
            float localZ = UnityEngine.Random.Range(-halfSize.y, halfSize.y);
            Vector3 localOffset = new Vector3(localX, 0f, localZ);

            spawnPos = plane.transform.TransformPoint(localOffset);
        }
        else
        {
            var cam = Camera.main;
            spawnPos = (cam != null)
                ? cam.transform.position + cam.transform.forward * 1f
                : Vector3.zero;
        }

        var e = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        var script = e.GetComponent<Enemy>();
        if (script == null)
        {
            Debug.LogError("[Spawner] Prefab sans script Enemy !", e);
            Destroy(e);
            return;
        }

        enemies.Add(e);
        script.OnDeath += () => enemies.Remove(e);
    }
}
