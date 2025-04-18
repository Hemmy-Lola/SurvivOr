using System.Collections;
using UnityEngine;

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
        // Ajoute ici animations / d�sactivation / effets si besoin
    }
}
