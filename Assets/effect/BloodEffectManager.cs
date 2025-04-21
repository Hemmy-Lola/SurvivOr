using UnityEngine;

public class BloodEffectManager : MonoBehaviour
{
    public static BloodEffectManager instance;

    public GameObject bloodEffectPrefab;

    void Awake()
    {
        instance = this;
    }

    public void SpawnBlood(Vector3 position)
    {
        if (bloodEffectPrefab != null)
        {
            GameObject splash = Instantiate(bloodEffectPrefab, position, Quaternion.identity);
            Destroy(splash, 2f);
        }
        else
        {
            Debug.LogWarning("Aucun prefab de sang assigné !");
        }
    }

    // Appel statique pratique depuis d'autres scripts
    public static void SpawnBloodStatic(Vector3 position)
    {
        if (instance != null)
            instance.SpawnBlood(position);
    }
}