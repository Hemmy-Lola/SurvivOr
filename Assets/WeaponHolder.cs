using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponModel;
    public Vector3 weaponPosition = new Vector3(0.2f, -0.15f, 0.3f);  // Position FPS plus proche
    public Vector3 weaponRotation = new Vector3(0, 100, 1);  // Rotation par défaut pour FPS

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
            UpdateWeaponTransform();
        }
        else
        {
            Debug.LogError("[WeaponHolder] Pas de modèle d'arme assigné!");
        }
    }

    void Update()
    {
        if (weaponTransform == null) return;

        // Calcul du mouvement de l'arme basé sur le mouvement de la caméra
        float moveX = -Input.GetAxis("Mouse X") * swayAmount;
        float moveY = -Input.GetAxis("Mouse Y") * swayAmount;

        // Limiter le mouvement
        moveX = Mathf.Clamp(moveX, -maxSway, maxSway);
        moveY = Mathf.Clamp(moveY, -maxSway, maxSway);

        // Position cible avec le balancement
        Vector3 swayPosition = new Vector3(moveX, moveY, 0);
        targetPosition = initialPosition + swayPosition;

        // Appliquer le mouvement avec lissage
        weaponTransform.localPosition = Vector3.Lerp(
            weaponTransform.localPosition, 
            targetPosition, 
            Time.deltaTime * swaySmoothness
        );

        // Maintenir la rotation
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
