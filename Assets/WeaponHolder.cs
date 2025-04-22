using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponModel;
    public Vector3 weaponPosition = new Vector3(0.2f, -0.15f, 0.3f);  // Position FPS plus proche
    public Vector3 weaponRotation = new Vector3(0, 100, 1);  // Rotation par défaut pour FPS

    [Header("Render Settings")]
    public bool disableShadows = true;  // Désactiver les ombres par défaut
    public int renderQueue = 3000;      // Mettre l'arme au-dessus des objets AR

    [Header("FPS Settings")]
    [Range(0.1f, 1f)]
    public float smoothing = 0.1f;  // Lissage des mouvements
    public float swayAmount = 0.02f;  // Amplitude du balancement
    public float maxSway = 0.06f;  // Balancement maximum
    public float swaySmoothness = 3f;  // Lissage du balancement

    private Camera mainCamera;
    private Transform weaponTransform;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("[WeaponHolder] Pas de caméra principale trouvée!");
            return;
        }

        if (weaponModel != null)
        {
            GameObject weapon = Instantiate(weaponModel, mainCamera.transform);
            weaponTransform = weapon.transform;
            initialPosition = weaponPosition;
            
            // Configuration du rendu pour l'AR
            ConfigureWeaponForAR(weapon);
            
            UpdateWeaponTransform();
        }
        else
        {
            Debug.LogError("[WeaponHolder] Pas de modèle d'arme assigné!");
        }
    }

    void ConfigureWeaponForAR(GameObject weapon)
    {
        // Configurer tous les Renderers (modèle principal et enfants)
        Renderer[] renderers = weapon.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // Désactiver les ombres si demandé
            if (disableShadows)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            // Ajuster la queue de rendu pour tous les matériaux
            foreach (Material mat in renderer.materials)
            {
                mat.renderQueue = renderQueue;
                
                // S'assurer que le shader est configuré pour l'AR
                if (mat.shader.name.Contains("Standard"))
                {
                    mat.SetInt("_ZWrite", 1);  // Activer l'écriture de profondeur
                    mat.SetInt("_ZTest", 4);   // LEqual - rendu normal de profondeur
                }
            }
        }
    }

    void Update()
    {
        if (weaponTransform == null) return;

        float moveX = -Input.GetAxis("Mouse X") * swayAmount;
        float moveY = -Input.GetAxis("Mouse Y") * swayAmount;

        moveX = Mathf.Clamp(moveX, -maxSway, maxSway);
        moveY = Mathf.Clamp(moveY, -maxSway, maxSway);

        Vector3 swayPosition = new Vector3(moveX, moveY, 0);
        targetPosition = initialPosition + swayPosition;

        weaponTransform.localPosition = Vector3.Lerp(
            weaponTransform.localPosition, 
            targetPosition, 
            Time.deltaTime * swaySmoothness
        );

        weaponTransform.localRotation = Quaternion.Euler(weaponRotation);
    }

    void UpdateWeaponTransform()
    {
        if (weaponTransform != null)
        {
            weaponTransform.localPosition = weaponPosition;
            weaponTransform.localRotation = Quaternion.Euler(weaponRotation);
            initialPosition = weaponPosition;
        }
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateWeaponTransform();
        }
    }
}
