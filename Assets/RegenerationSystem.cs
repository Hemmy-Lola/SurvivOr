using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Ajoutez cette directive en haut du fichier

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
    public float rayonDetection = 1.5f; // Rayon équivalent à un hexagone
    public float degatsParEnnemi = 10f; // Dégâts infligés par ennemi proche
    public GameObject[] prefabsEnnemis; // Liste des prefabs d'ennemis

    private Coroutine regenCoroutine;
    private Coroutine attenteAvantRegenCoroutine;

    public GameObject menuScoreCanvas;

    public virtual void RecevoirDegats(float degatsRecus)
    {
        float degatsEffectifs = degatsRecus * (100f / (100f + armure));
        pointsDeVie -= degatsEffectifs;
        pointsDeVie = Mathf.Max(0, pointsDeVie);

        Debug.Log($"{gameObject.name} a reçu {degatsEffectifs} dégâts, PV restants: {pointsDeVie}");

        // Stopper la régénération si en cours
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        if (attenteAvantRegenCoroutine != null)
        {
            StopCoroutine(attenteAvantRegenCoroutine);
        }

        // Redémarrer le timer de régénération
        attenteAvantRegenCoroutine = StartCoroutine(AttenteAvantRegen());

        // Si mort
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
            // Ajouter les points fractionnaires accumulés
            accumulator += regenParSeconde * Time.deltaTime;

            // Convertir l'accumulateur en points entiers à ajouter
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
            // Assurez-vous que le MenuScoreCanvas est actif
            // mais qu'il est invisible grâce au CanvasGroup (alpha = 0)
            menuScore.SetActive(true);

            CanvasGroup cg = menuScore.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                Debug.LogWarning("Aucun CanvasGroup trouvé sur MenuScoreCanvas !");
            }
            else
            {
                // Rendre visible le CanvasGroup (instantanément ou en fondu)
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }

            // Mettre à jour les informations de score
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
                    // Masquer le menu en ramenant l'alpha à 0
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
        Time.timeScale = 1f; // Réactiver le temps

        // Réinitialiser les points de vie et l'état mort
        pointsDeVie = pointsDeVieMax;
        estMort = false;

        // Réinitialiser le score
        ScoreManager.ReinitialiserScore();

        // Détruire uniquement les clones d'Enemy (ceux dont le nom contient "(Clone)")
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            if (enemy.gameObject.name.Contains("(Clone)"))
            {
                Destroy(enemy.gameObject);
            }
        }

        // Retourner au menu de scan
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
            // Trouver toutes les instances de ce prefab dans la scène
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
        // Visualiser le rayon de détection dans l'éditeur
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rayonDetection);
    }
}