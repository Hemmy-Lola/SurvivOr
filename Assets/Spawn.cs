using UnityEngine;
using System.Collections.Generic;

public class SpawnerZombies : MonoBehaviour
{
    [System.Serializable]
    public class ConfigSpawn
    {
        public string typeZombie;
        public GameObject prefabZombie;
        public int limiteMax = 5;          // Nombre maximum de ce type
        public float tempsEntreSpawns = 3f; // Temps entre chaque spawn
        public Transform[] pointsDeSpawn;  // Plusieurs points de spawn possibles
        [HideInInspector] public float timer = 0f;
        [HideInInspector] public int nombreActuel = 0;
    }

    [Header("Configuration des spawns")]
    public List<ConfigSpawn> configurationsSpawn = new List<ConfigSpawn>();

    [Header("Paramètres de difficulté")]
    public float multiplicateurVitesseSpawn = 1f; 
    public float tempsAvantProchaineVague = 60f;
    private float timerVague = 0f;
    public int vagueActuelle = 0;

    void Update()
    {
        timerVague += Time.deltaTime;
        if (timerVague >= tempsAvantProchaineVague)
        {
            NouvelleVague();
        }

        foreach (var config in configurationsSpawn)
        {
            if (config.nombreActuel < config.limiteMax)
            {
                config.timer += Time.deltaTime;

                float tempsAjuste = config.tempsEntreSpawns / multiplicateurVitesseSpawn;

                if (config.timer >= tempsAjuste)
                {
                    SpawnerZombie(config);
                    config.nombreActuel++;
                    config.timer = 0f;
                    Debug.Log($"{config.typeZombie} créé. Total: {config.nombreActuel}/{config.limiteMax}");
                }
            }
        }
    }

    private void NouvelleVague()
    {
        vagueActuelle++;
        timerVague = 0f;

        // Augmenter la difficulté
        multiplicateurVitesseSpawn += 0.2f;

        // Augmenter les limites de zombies
        foreach (var config in configurationsSpawn)
        {
            config.limiteMax += 2;
        }

        Debug.Log($"Vague: {vagueActuelle}! Plus de zombies arrivent...");
    }

    private GameObject SpawnerZombie(ConfigSpawn config)
    {
        // Choisir un point de spawn aléatoire parmi ceux disponibles
        Transform pointSpawn = null;
        if (config.pointsDeSpawn != null && config.pointsDeSpawn.Length > 0)
        {
            int indexAleatoire = Random.Range(0, config.pointsDeSpawn.Length);
            pointSpawn = config.pointsDeSpawn[indexAleatoire];
        }

        // Déterminer la position de spawn
        Vector3 position = pointSpawn != null ? pointSpawn.position : transform.position;
        Quaternion rotation = pointSpawn != null ? pointSpawn.rotation : Quaternion.identity;

        // Ajouter un peu de variation aléatoire à la position
        position += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));

        // Instancier le zombie
        GameObject zombie = Instantiate(config.prefabZombie, position, rotation);

        return zombie;
    }

    // Méthode appelée quand un zombie est tué
    public void NotifierMortZombie(GameObject zombie)
    {
        // Trouver la configuration correspondante et décrémenter le compteur
        foreach (var config in configurationsSpawn)
        {
            if ((zombie.GetComponent<ZombieNormal>() != null && config.typeZombie == "Normal") ||
                (zombie.GetComponent<ZombieArcher>() != null && config.typeZombie == "Archer"))
                //(zombie.GetComponent<ZombieRapide>() != null && config.typeZombie == "Rapide"))
            {
                config.nombreActuel--;
                Debug.Log($"{config.typeZombie} éliminé. Total: {config.nombreActuel}/{config.limiteMax}");
                break;
            }
        }
    }
}