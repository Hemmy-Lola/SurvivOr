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

    [Header("R�g�n�ration")]
    public float regenParSeconde = 20f;
    public float delaiAvantRegen = 5f;

    [Header("D�g�ts des ennemis")]
    public float rayonDetection = 1.5f; // Rayon �quivalent � un hexagone
    public float degatsParEnnemi = 10f; // D�g�ts inflig�s par ennemi proche
    public GameObject[] prefabsEnnemis; // Liste des prefabs d'ennemis

    private Coroutine regenCoroutine;
    private Coroutine attenteAvantRegenCoroutine;

    public virtual void RecevoirDegats(float degatsRecus)
    {
        float degatsEffectifs = degatsRecus * (100f / (100f + armure));
        pointsDeVie -= degatsEffectifs;
        pointsDeVie = Mathf.Max(0, pointsDeVie);

        Debug.Log($"{gameObject.name} a re�u {degatsEffectifs} d�g�ts, PV restants: {pointsDeVie}");

        // Stopper la r�g�n�ration si en cours
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }

        if (attenteAvantRegenCoroutine != null)
        {
            StopCoroutine(attenteAvantRegenCoroutine);
        }

        // Red�marrer le timer de r�g�n�ration
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
            Debug.Log($"{gameObject.name} r�g�n�re... PV: {pointsDeVie}");
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
        Debug.Log("MenuScoreCanvas trouv� et activ�.");
        menuScore.SetActive(true);

        // Mettre � jour les informations de score
        var texteScore = menuScore.transform.Find("TexteScore")?.GetComponent<UnityEngine.UI.Text>();
        var boutonRetour = menuScore.transform.Find("BoutonRetour")?.GetComponent<UnityEngine.UI.Button>();

        if (texteScore != null)
        {
            string detailsScore = "Score :\n";
            foreach (var entry in ScoreManager.ObtenirZombiesTuesParType())
            {
                detailsScore += $"{entry.Key} : {entry.Value} tu�s\n";
            }

            detailsScore += $"\nTotal Zombies : {ScoreManager.ObtenirNombreTotalZombies()}";
            detailsScore += $"\nScore Total : {ScoreManager.ObtenirScoreTotal()}";
            texteScore.text = detailsScore;
            Debug.Log("Score mis � jour : " + detailsScore);
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
        // R�initialiser le score
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
            // Trouver toutes les instances de ce prefab dans la sc�ne
            var ennemis = GameObject.FindGameObjectsWithTag(prefabEnnemi.tag);

            foreach (var ennemi in ennemis)
            {
                float distance = Vector3.Distance(transform.position, ennemi.transform.position);
                if (distance <= rayonDetection)
                {
                    Debug.Log($"[RegenerationSystem] Ennemi d�tect� : {ennemi.name} � une distance de {distance}");
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
        // Visualiser le rayon de d�tection dans l'�diteur
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rayonDetection);
    }
}