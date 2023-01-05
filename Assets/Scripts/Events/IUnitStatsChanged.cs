public interface IUnitStatsChanged : IReactiveEventInterface
{
    public delegate void EnemyKilled(TurnBasedUnit killed, TurnBasedUnit killedBy);
    public delegate void UnitLost(TurnBasedUnit killedBy);

    public event EnemyKilled OnEnemyKilled;
    public event UnitLost OnUnitLost;
}