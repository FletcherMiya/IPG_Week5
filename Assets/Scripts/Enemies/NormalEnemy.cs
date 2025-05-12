public class NormalEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        stats = new CharacterStats(50, 1, 0.5f);
    }
}