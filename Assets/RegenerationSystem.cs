using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegenerationSystem : MonoBehaviour
{
    [Header("Stats")]
    public float pointsDeVie = 100f;
    public float pointsDeVieMax = 100f;
    public float armure = 0f;
    public bool estMort = false;

    [Header("Régénération")]
    public float regenParSeconde = 20f;
    public float delaiAvantRegen = 5f;

    [Header("Dégâts des ennemis")]
    public float rayonDetection = 1.5f; // Rayon équivalent à un hexagone
    public float degatsParEnnemi = 10f; // Dégâts infligés par ennemi proche
    public GameObject[] prefabsEnnemis; // Liste des prefabs d'ennemis

    private Coroutine regenCoroutine;
    private Coroutine attenteAvantRegenCoroutine;

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
        while (pointsDeVie < pointsDeVieMax)
        {
            pointsDeVie += regenParSeconde * Time.deltaTime;
            pointsDeVie = Mathf.Min(pointsDeVie, pointsDeVieMax);
            Debug.Log($"{gameObject.name} régénère... PV: {pointsDeVie}");
            yield return null;
        }

        regenCoroutine = null;
    }

    protected virtual void Mourir()
    {
        estMort = true;
        Debug.Log($"{gameObject.name} est mort !");

        // Afficher le menu de score
        AfficherMenuScore();
    }

    private void AfficherMenuScore()
    {
        // Charger le Canvas du menu de score
        GameObject menuScore = GameObject.Find("MenuScoreCanvas");
        if (menuScore == null)
        {
            Debug.LogWarning("MenuScoreCanvas introuvable !");
            return;
        }
        Debug.Log("MenuScoreCanvas trouvé et activé.");
        menuScore.SetActive(true);

        // Mettre à jour les informations de score
        var texteScore = menuScore.transform.Find("TexteScore")?.GetComponent<UnityEngine.UI.Text>();
        var boutonRetour = menuScore.transform.Find("BoutonRetour")?.GetComponent<UnityEngine.UI.Button>();

        if (texteScore != null)
        {
            string detailsScore = "Score :\n";
            foreach (var entry in ScoreManager.ObtenirZombiesTuesParType())
            {
                detailsScore += $"{entry.Key} : {entry.Value} tués\n";
            }

            detailsScore += $"\nTotal Zombies : {ScoreManager.ObtenirNombreTotalZombies()}";
            detailsScore += $"\nScore Total : {ScoreManager.ObtenirScoreTotal()}";
            texteScore.text = detailsScore;
            Debug.Log("Score mis à jour : " + detailsScore);
        }
        else
        {
            Debug.LogWarning("TexteScore introuvable ou son composant Text est manquant !");
        }

        if (boutonRetour != null)
        {
            boutonRetour.onClick.RemoveAllListeners();
            boutonRetour.onClick.AddListener(() =>
            {
                RetourAuMenu();
            });
        }
        else
        {
            Debug.LogWarning("BoutonRetour introuvable ou son composant Button est manquant !");
        }
    }

    private void RetourAuMenu()
    {
        // Réinitialiser le score
        ScoreManager.ReinitialiserScore();

        // Retourner au menu de scan
        var scanManager = FindObjectOfType<ScanAndPlayManager>();
        if (scanManager != null)
        {
            scanManager.RestartScanning();
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