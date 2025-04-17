using UnityEngine;
using static SpawnerZombies;

public abstract class PersonnageBase : MonoBehaviour
{
    [Header("Statistiques")]
    [SerializeField] protected float pointsDeVieMax;
    [SerializeField] protected float pointsDeVie;
    [SerializeField] protected float degats;
    [SerializeField] protected float vitesseDeplacement;
    [SerializeField] protected float vitesseAttaque;
    [SerializeField] protected float armure;
    [SerializeField] protected float tempsRespawn;

    protected bool estMort = false;

    protected virtual void Awake()
    {
        InitialiserStats();
    }

    // Méthode abstraite pour initialiser les statistiques spécifiques au personnage
    protected abstract void InitialiserStats();

    // Génère une position aléatoire dans les limites spécifiées
    private Vector3 GenererPositionAleatoire(Vector3[] limites)
    {
        if (limites == null || limites.Length < 3)
        {
            Debug.LogError("Les limites fournies sont invalides pour générer une position aléatoire.");
            return Vector3.zero;
        }

        float x = Random.Range(limites[0].x, limites[2].x);
        float z = Random.Range(limites[0].z, limites[2].z);
        return new Vector3(x, limites[0].y, z);
    }

    // Spawne un zombie à une position aléatoire dans les limites spécifiées
    private GameObject SpawnerZombie(ConfigSpawn config, Vector3[] limites)
    {
        if (config == null || config.prefabZombie == null)
        {
            Debug.LogError("Configuration de spawn ou prefabZombie invalide.");
            return null;
        }

        Vector3 position = GenererPositionAleatoire(limites);
        GameObject zombie = Instantiate(config.prefabZombie, position, Quaternion.identity);
        return zombie;
    }
}