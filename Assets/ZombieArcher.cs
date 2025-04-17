// Archer.cs
using Unity.VisualScripting;
using UnityEngine;

public class ZombieArcher : PersonnageBase
{
    protected override void InitialiserStats()
    {
        float pointsDeVieMax = 90f;
        float pointsDeVie = pointsDeVieMax;
        float degats = 18f;
        float vitesseDeplacement = 0.8f;
        float vitesseAttaque = 1.5f;
        float armure = 5f;
        float tempsRespawn = 3f; // mettre en time 
    }
}