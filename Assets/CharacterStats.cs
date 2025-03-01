[System.Serializable]
public struct CharacterStats
{
    public float health;
    public float damage;
    public float speed;

    public CharacterStats(float health, float damage, float speed)
    {
        this.health = health;
        this.damage = damage;
        this.speed = speed;
    }
}