using benjohnson;
using UnityEngine;
using UnityEngine.Events;

public class EC_Health : MonoBehaviour
{
    public int maxHealth;
    int currentHealth;

    [Header("Events")]
    public UnityEvent deathEvent;
    public UnityEvent damageEvent;

    // Components
    EC_Animator anim;
    [SerializeField] Counter counter;

    void Awake()
    {
        anim = GetComponentInChildren<EC_Animator>();

        currentHealth = maxHealth;
        UpdateCounter();
    }

    public void Damage(int value)
    {
        PlayerStats.instance.damageDealt += value;
        damageEvent.Invoke();
        DamagePopup.CreatePopup(transform.position, value);
        SoundManager.instance.PlaySound("Enemy Hurt");
        ArtifactManager.instance.TriggerDealDamage();
        currentHealth -= value;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Kill();
        }
        UpdateCounter();
        anim?.Squash(1.5f, 0.75f);
    }

    public void Heal(int value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UpdateCounter();
    }

    void UpdateCounter()
    {
        if (counter == null) return;
        counter.SetText(currentHealth.ToString(), 0);
    }

    public void Kill()
    {
        PlayerStats.instance.enemiesDefeated++;
        PlayerStats.instance.bonusEnemiesDefeated++;
        if(PlayerStats.instance.bonusEnemiesDefeated == 10)
        {
            PlayerStats.instance.bonusEnemiesDefeated = 0;
            Player.instance.bonusCoins.DropCoins();
        }

        deathEvent.Invoke();
        ArtifactManager.instance.TriggerKillEnemy();

        GetComponent<EC_Entity>().Remove();
    }
}
