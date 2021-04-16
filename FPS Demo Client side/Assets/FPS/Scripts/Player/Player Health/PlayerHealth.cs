
public class PlayerHealth
{
    PlayerManager playerManager;

    float health;

    public PlayerHealth(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
        health = playerManager.maxHealth;      
    }

    public void SetHealth(float health)
    {
        this.health = health;
        if (this.health <= 0)
           playerManager.Die();
    }
    public float GetHealth() => health;
    public float GetHealthPercent() => (health / playerManager.maxHealth) * 100;
}
