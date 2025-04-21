using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{

    private static Dictionary<string, int> zombiesTuesParType = new Dictionary<string, int>();
    private static int scoreTotal = 0;

    public static void AjouterZombieTue(string typeZombie, int points)
    {
        if (!zombiesTuesParType.ContainsKey(typeZombie))
        {
            zombiesTuesParType[typeZombie] = 0;
        }

        zombiesTuesParType[typeZombie]++;
        scoreTotal += points;
    }

    public static Dictionary<string, int> ObtenirZombiesTuesParType()
    {
        return new Dictionary<string, int>(zombiesTuesParType);
    }

    public static int ObtenirScoreTotal()
    {
        return scoreTotal;
    }

    public static int ObtenirNombreTotalZombies()
    {
        int total = 0;
        foreach (var count in zombiesTuesParType.Values)
        {
            total += count;
        }
        return total;
    }

    public static void ReinitialiserScore()
    {
        zombiesTuesParType.Clear();
        scoreTotal = 0;
        Debug.Log("Score réinitialisé.");
    }
}