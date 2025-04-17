using UnityEngine.XR.ARFoundation;
using UnityEngine;

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
