// Barbare.cs
using UnityEngine;

public class ZombieNormal : PersonnageBase
{
    protected override void InitialiserStats()
    {
        float pointsDeVieMax = 150f;
        float pointsDeVie = pointsDeVieMax;
        float degats = 15f;
        float vitesseDeplacement = 0.8f;
        float vitesseAttaque = 0.8f;
        float armure = 10f;
        float tempsRespawn = 2f; // mettre en time 
    }
}