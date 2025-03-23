using benjohnson;
using UnityEngine;

public class PlayerStats : Singleton<PlayerStats>
{
    public bool win;
    public int stage;
    public int time;
    public int enemiesDefeated;
    public int bonusEnemiesDefeated; //For bonus rewards
    public int artifactsDiscovered;
    public int artifactsTriggered;
    public int damageDealt;
    public int damageTaken;
    public int healthHealed;
    public int coinsCollected;

    public void ResetStats()
    {
        win = false;
        stage = 0;
        time = 0;
        enemiesDefeated = 0;
        bonusEnemiesDefeated = 0;
        artifactsDiscovered = 0;
        artifactsTriggered = 0;
        damageDealt = 0;
        damageTaken = 0;
        healthHealed = 0;
        coinsCollected = 0;
    }
}
