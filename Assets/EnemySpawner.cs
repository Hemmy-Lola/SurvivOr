using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab & limites")]
    public GameObject enemyPrefab;    
    public int maxEnemies = 10;

    [Header("Paramètres de spawn")]
    public float spawnRadius = 4f;
    public float spawnInterval = 2f;

    private Transform camTransform;
    private List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        if (Camera.main == null)
        {
            Debug.LogError("❌ Pas de MainCamera dans la scène ! Tague ta camera AR en 'MainCamera'.");
            enabled = false;
            return;
        }
        camTransform = Camera.main.transform;

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
            Debug.LogError("❌ enemyPrefab non assigné dans l’Inspector !", this);
            return;
        }

        Vector3 dir = Random.onUnitSphere;
        dir.y = 0;
        Vector3 pos = camTransform.position + dir.normalized * spawnRadius;

        GameObject e = Instantiate(enemyPrefab, pos, Quaternion.identity);
        Enemy enemyScript = e.GetComponent<Enemy>();
        if (enemyScript == null)
        {
            Debug.LogError("❌ Ton prefab Enemy n'a pas le script Enemy !", e);
            Destroy(e);
            return;
        }

        enemies.Add(e);
        enemyScript.OnDeath += () => enemies.Remove(e);
    }
}
