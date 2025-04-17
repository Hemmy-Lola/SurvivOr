using UnityEngine;

public class TouchShooter : MonoBehaviour
{
    public float portee = 100f;
    public LayerMask cibleLayer;

    void Update()
    {
        // V�rifie s�il y a au moins un doigt sur l��cran
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // On ne tire que au d�but du toucher
            if (touch.phase == TouchPhase.Began)
            {
                Tirer(touch.position);
            }
        }
    }

    void Tirer(Vector2 positionEcran)
    {
        Ray ray = Camera.main.ScreenPointToRay(positionEcran);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, portee, cibleLayer))
        {
            Debug.Log($"Touch�: {hit.collider.name}");

            var cible = hit.collider.GetComponent<RegenerationSystem>();
            if (cible != null)
            {
                cible.RecevoirDegats(20f); // ou ce que tu veux
            }
        }
        else
        {
            Debug.Log("Tir rat� !");
        }
    }
}
