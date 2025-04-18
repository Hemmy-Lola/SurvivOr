using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab & limites")]
    public List<GameObject> enemyPrefabs; // Liste des différents types de zombies
    public int maxEnemies = 10;

    [Header("Paramètres de spawn")]
    public Transform[] spawnZones; // Zones de spawn prédéfinies
    public float spawnInterval = 2f;

    private List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        if (spawnZones == null || spawnZones.Length == 0)
        {
            Debug.LogError("❌ Pas de zones de spawn définies !");
            enabled = false;
            return;
        }

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

        // Choisir une zone de spawn aléatoire
        Transform spawnZone = spawnZones[Random.Range(0, spawnZones.Length)];
        Vector3 spawnPosition = spawnZone.position + new Vector3(
            Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)
        );

        // Choisir un type de zombie aléatoire
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Vérifier que l'ennemi a bien un script Enemy
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