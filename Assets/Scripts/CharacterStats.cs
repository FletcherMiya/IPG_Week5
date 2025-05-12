[System.Serializable]
public class CharacterStats
{
    public float maxHealth;
    public float health;
    public float speed;

    public CharacterStats(float maxHealth, float damage, float speed)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.speed = speed;
    }
}