using UnityEngine;

public interface IUnitKilledPropertyChanged : IReactiveEventInterface
{
    public delegate void EnemyKilled(UnitScriptable unit, Vector3 position);
    public delegate void FriendKilled(UnitScriptable unit, Vector3 position);

    public event EnemyKilled OnEnemyKilled;
    public event FriendKilled OnFriendKilled;
}