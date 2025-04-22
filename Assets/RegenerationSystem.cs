using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RegenerationSystem : MonoBehaviour
{
    [Header("Stats")]
    public float pointsDeVie = 100f;
    public float pointsDeVieMax = 100f;
    public float armure = 0f;
    public bool estMort = false;

    [Header("Régénération")]
    public float regenParSeconde = 20.00f;
    public float delaiAvantRegen = 5f;

    [Header("Dégâts des ennemis")]
    public float rayonDetection = 1.5f;
    public float degatsParEnnemi = 10f;
    public GameObject[] prefabsEnnemis;

    private Coroutine regenCoroutine;
    private Coroutine attenteAvantRegenCoroutine;

    public GameObject menuScoreCanvas;

    public virtual void RecevoirDegats(float degatsRecus)
    {
        float degatsEffectifs = degatsRecus * (100f / (100f + armure));
        pointsDeVie -= degatsEffectifs;
        pointsDeVie = Mathf.Max(0, pointsDeVie);

        Debug.Log($"{gameObject.name} a reçu {degatsEffectifs} dégâts, PV restants: {pointsDeVie}");

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        if (attenteAvantRegenCoroutine != null)
        {
            StopCoroutine(attenteAvantRegenCoroutine);
        }

        attenteAvantRegenCoroutine = StartCoroutine(AttenteAvantRegen());

        if (pointsDeVie <= 0 && !estMort)
        {
            Mourir();
        }
    }

    void Update()
    {
        VerifierEnnemisProches();
    }

    private IEnumerator AttenteAvantRegen()
    {
        yield return new WaitForSeconds(delaiAvantRegen);
        regenCoroutine = StartCoroutine(Regeneration());
    }

    private IEnumerator Regeneration()
    {
        float accumulator = 0f;
        while (pointsDeVie < pointsDeVieMax)
        {
            accumulator += regenParSeconde * Time.deltaTime;

            int healPoints = Mathf.FloorToInt(accumulator);
            if (healPoints > 0)
            {
                pointsDeVie = Mathf.Min(pointsDeVie + healPoints, pointsDeVieMax);
                accumulator -= healPoints;
                Debug.Log($"{gameObject.name} régénère... PV: {pointsDeVie}");
            }

            yield return null;
        }

        regenCoroutine = null;
    }

    protected virtual void Mourir()
    {
        estMort = true;
        Debug.Log($"{gameObject.name} est mort !");

        Time.timeScale = 0f;

        BloodEffectManager.SpawnBloodStatic(this.transform.position);


        AfficherMenuScore();
    }

    private void AfficherMenuScore()
    {
        try
        {
            GameObject menuScore = GameObject.Find("MenuScoreCanvas");
            if (menuScore == null)
            {
                Debug.LogWarning("MenuScoreCanvas introuvable !");
                return;
            }

            menuScore.SetActive(true);

            CanvasGroup cg = menuScore.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                Debug.LogWarning("Aucun CanvasGroup trouvé sur MenuScoreCanvas !");
            }
            else
            {
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }

            var texteScore = menuScore.transform.Find("TexteScore")?.GetComponent<TextMeshProUGUI>();
            var boutonRetour = menuScore.transform.Find("BoutonRetour")?.GetComponent<UnityEngine.UI.Button>();

            if (texteScore != null)
            {
                string detailsScore = "Details fin de partie:\n";
                detailsScore += $"\nTotal Zombies : {ScoreManager.ObtenirNombreTotalZombies()}";
                detailsScore += $"\nScore Total : {ScoreManager.ObtenirScoreTotal()}";
                texteScore.text = detailsScore;
                Debug.Log("Score mis à jour : " + detailsScore);
            }
            else
            {
                Debug.LogWarning("TexteScore introuvable ou son composant TextMeshProUGUI est manquant !");
            }

            if (boutonRetour != null)
            {
                boutonRetour.onClick.RemoveAllListeners();
                boutonRetour.onClick.AddListener(() =>
                {
                    Debug.Log("BoutonRetour a été cliqué.");
                    if (cg != null)
                    {
                        cg.alpha = 0f;
                        cg.interactable = false;
                        cg.blocksRaycasts = false;
                    }
                    RetourAuMenu();
                });
            }
            else
            {
                Debug.LogWarning("BoutonRetour introuvable ou son composant Button est manquant !");
            }

            if (texteScore != null && boutonRetour != null)
            {
                RectTransform texteRect = texteScore.GetComponent<RectTransform>();
                RectTransform boutonRect = boutonRetour.GetComponent<RectTransform>();

                if (texteRect != null && boutonRect != null)
                {
                    Vector3 textePosition = texteRect.localPosition;
                    boutonRect.localPosition = new Vector3(
                        textePosition.x,
                        textePosition.y - texteRect.rect.height - 20f,
                        textePosition.z
                    );
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erreur lors du chargement de la scène MenuScore : " + e.Message);
            return;
        }
    }

    private void RetourAuMenu()
    {
        Time.timeScale = 1f; 

        pointsDeVie = pointsDeVieMax;
        estMort = false;

        ScoreManager.ReinitialiserScore();

        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            if (enemy.gameObject.name.Contains("(Clone)"))
            {
                Destroy(enemy.gameObject);
            }
        }

        var scanManager = Object.FindFirstObjectByType<ScanAndPlayManager>();
        if (scanManager != null)
        {
            Debug.Log("Retour au menu de scan.");
            scanManager.RestartScanning();
        }
        else 
        {
            Debug.LogWarning("ScanAndPlayManager introuvable !");
        }
    }

    private void VerifierEnnemisProches()
    {
        foreach (var prefabEnnemi in prefabsEnnemis)
        {
            var ennemis = GameObject.FindGameObjectsWithTag(prefabEnnemi.tag);

            foreach (var ennemi in ennemis)
            {
                float distance = Vector3.Distance(transform.position, ennemi.transform.position);
                if (distance <= rayonDetection)
                {
                    Debug.Log($"[RegenerationSystem] Ennemi détecté : {ennemi.name} à une distance de {distance}");
                    if (ennemi.TryGetComponent<Enemy>(out var enemyScript))
                    {
                        RecevoirDegats(degatsParEnnemi);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rayonDetection);
    }
}