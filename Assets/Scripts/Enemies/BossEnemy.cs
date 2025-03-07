public class BossEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        stats = new CharacterStats(200, 30, 1f);
    }
}