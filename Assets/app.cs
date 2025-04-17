using UnityEngine;

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

    protected abstract void InitialiserStats();
    private Vector3 GenererPositionAleatoire(Vector3[] limites)
    {
        float x = Random.Range(limites[0].x, limites[2].x);
        float z = Random.Range(limites[0].z, limites[2].z);
        return new Vector3(x, limites[0].y, z);
    }
    private GameObject SpawnerZombie(ConfigSpawn config, Vector3[] limites)
    {
        Vector3 position = GenererPositionAleatoire(limites);
        GameObject zombie = Instantiate(config.prefabZombie, position, Quaternion.identity);
        return zombie;
    }
 }
public class AireDeJeu : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject marqueurPrefab;
    private Vector3[] limites = new Vector3[4];
    private int indexLimite = 0;

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (indexLimite < 4)
            {
                Vector2 positionEcran = Input.GetTouch(0).position;
                Ray ray = Camera.main.ScreenPointToRay(positionEcran);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    limites[indexLimite] = hit.point;
                    Instantiate(marqueurPrefab, hit.point, Quaternion.identity);
                    indexLimite++;
                }
            }
        }
    }

    public Vector3[] ObtenirLimites()
    {
        return limites;
    }
}
