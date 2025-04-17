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

    // M�thode abstraite pour initialiser les statistiques sp�cifiques au personnage
    protected abstract void InitialiserStats();

    // G�n�re une position al�atoire dans les limites sp�cifi�es
    private Vector3 GenererPositionAleatoire(Vector3[] limites)
    {
        if (limites == null || limites.Length < 3)
        {
            Debug.LogError("Les limites fournies sont invalides pour g�n�rer une position al�atoire.");
            return Vector3.zero;
        }

        float x = Random.Range(limites[0].x, limites[2].x);
        float z = Random.Range(limites[0].z, limites[2].z);
        return new Vector3(x, limites[0].y, z);
    }

    // Spawne un zombie � une position al�atoire dans les limites sp�cifi�es
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